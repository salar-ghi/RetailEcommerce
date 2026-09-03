namespace Application.Services;

public class BasketService : IBasketService
{
    private static readonly TimeSpan BasketCacheDuration = TimeSpan.FromDays(1);
    private const string BasketCacheKeyPrefix = "basket:";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IRedisCacheService _cacheService;
    private readonly IMapper _mapper;

    public BasketService(IUnitOfWork unitOfWork, IRedisCacheService cacheService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<BasketDto> GetBasketAsync(string userId)
    {
        var cachedBasket = await _cacheService.GetCachedDataAsync<BasketDto>(GetBasketCacheKey(userId));
        if (cachedBasket != null)
        {
            return cachedBasket;
        }

        var basket = await GetOrCreateBasketFromDbAsync(userId);
        return await CacheAndMapBasketAsync(basket);
    }

    public async Task AddItemToBasketAsync(string userId, int productId, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }

        var basket = await GetOrCreateBasketFromDbAsync(userId);
        var product = await _unitOfWork.Products.GetByIdAsync(productId,
            include: q => q.Include(p => p.Batches));
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {productId} not found.");

        decimal unitPrice = product.Batches.Any()
                        ? product.Batches.First().SellingPrice
                        : 0;

        var existingItem = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            existingItem.UnitPrice = unitPrice;
            existingItem.Product = product;
        }
        else
        {
            basket.Items.Add(new BasketItem
            {
                ProductId = productId,
                Product = product,
                Quantity = quantity,
                UnitPrice = unitPrice,
                BasketId = basket.Id
            });
        }

        basket.ModifiedTime = DateTime.UtcNow;
        await _unitOfWork.Baskets.UpdateAsync(basket);
        await _unitOfWork.SaveChangesAsync();
        await CacheAndMapBasketAsync(basket);
    }

    public async Task UpdateItemQuantityAsync(string userId, int productId, int quantity)
    {
        if (quantity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity cannot be negative.");
        }

        var basket = await GetOrCreateBasketFromDbAsync(userId);
        var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
        {
            throw new KeyNotFoundException($"Item with Product ID {productId} not found.");
        }

        if (quantity == 0)
        {
            basket.Items.Remove(item);
            // Do not rely on orphan detection here.  A basket item must be physically
            // removed so that it cannot reappear on a later read.
            await _unitOfWork.BasketItems.DeleteAsync(item);
        }
        else
        {
            item.Quantity = quantity;
            await _unitOfWork.BasketItems.UpdateAsync(item);
        }

        basket.ModifiedTime = DateTime.UtcNow;
        await _unitOfWork.Baskets.UpdateAsync(basket);
        await _unitOfWork.SaveChangesAsync();
        await CacheAndMapBasketAsync(basket);
    }

    public async Task RemoveItemFromBasketAsync(string userId, int productId)
    {
        var basket = await GetOrCreateBasketFromDbAsync(userId);
        var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            basket.Items.Remove(item);
            await _unitOfWork.BasketItems.DeleteAsync(item);
            basket.ModifiedTime = DateTime.UtcNow;
            await _unitOfWork.Baskets.UpdateAsync(basket);
            await _unitOfWork.SaveChangesAsync();
            await CacheAndMapBasketAsync(basket);
        }
    }

    public async Task ClearBasketAsync(string userId)
    {
        var basket = await _unitOfWork.Baskets.GetByUserIdAsync(userId, userId);
        if (basket != null)
        {
            await _unitOfWork.Baskets.DeleteAsync(basket);
            await _unitOfWork.SaveChangesAsync();
            await _cacheService.RemoveCachedDataAsync(GetBasketCacheKey(userId));
        }
    }


    public async Task<IEnumerable<BasketDto>> ListBasketsAsync(string? status = null, string? userId = null)
    {
        var baskets = await _unitOfWork.Baskets.GetAllAsync(q => q
            .Include(b => b.User)
            .Include(b => b.Items)
                .ThenInclude(i => i.Product));

        var filtered = baskets
            .Where(b => string.IsNullOrWhiteSpace(status) || b.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
            .Where(b => string.IsNullOrWhiteSpace(userId) || b.UserId == userId)
            .OrderByDescending(b => b.ModifiedTime);

        return _mapper.Map<IEnumerable<BasketDto>>(filtered);
    }

    public async Task<BasketDto> GetBasketByIdAsync(string basketId)
    {
        var basket = await _unitOfWork.Baskets.GetByIdAsync(basketId, q => q
            .Include(b => b.User)
            .Include(b => b.Items)
                .ThenInclude(i => i.Product)) ?? throw new KeyNotFoundException($"Basket with ID {basketId} not found.");
        return _mapper.Map<BasketDto>(basket);
    }

    public async Task<BasketDto> UpdateBasketAsync(string basketId, UpdateBasketRequest request)
    {
        var basket = await _unitOfWork.Baskets.GetByIdAsync(basketId, q => q.Include(b => b.Items).ThenInclude(i => i.Product)) ?? throw new KeyNotFoundException($"Basket with ID {basketId} not found.");
        basket.AdminNotes = request.AdminNotes;

        foreach (var requestedItem in request.Items)
        {
            var item = basket.Items.FirstOrDefault(i => i.Id == requestedItem.Id || i.ProductId == requestedItem.ProductId);
            if (item is null) continue;
            if (requestedItem.Quantity <= 0)
            {
                basket.Items.Remove(item);
                await _unitOfWork.BasketItems.DeleteAsync(item);
            }
            else
            {
                item.Quantity = requestedItem.Quantity;
                await _unitOfWork.BasketItems.UpdateAsync(item);
            }
        }

        basket.ModifiedTime = DateTime.UtcNow;
        await _unitOfWork.Baskets.UpdateAsync(basket);
        await _unitOfWork.SaveChangesAsync();
        var basketDto = await CacheAndMapBasketAsync(basket);
        return basketDto;
    }

    public async Task<BasketActionResultDto> DeleteBasketAsync(string basketId)
    {
        var basket = await _unitOfWork.Baskets.GetByIdAsync(basketId, q => q.Include(b => b.Items)) ?? throw new KeyNotFoundException($"Basket with ID {basketId} not found.");
        await _unitOfWork.Baskets.DeleteAsync(basket);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveCachedDataAsync(GetBasketCacheKey(GetBasketOwnerId(basket)));
        return new BasketActionResultDto { BasketId = basketId, Message = "Basket deleted." };
    }

    public async Task<BasketActionResultDto> ConvertBasketAsync(string basketId)
    {
        var basket = await _unitOfWork.Baskets.GetByIdAsync(basketId, q => q.Include(b => b.Items)) ?? throw new KeyNotFoundException($"Basket with ID {basketId} not found.");
        if (!basket.Items.Any()) throw new InvalidOperationException("Cannot convert an empty basket.");
        basket.Status = "ready_to_convert";
        basket.ModifiedTime = DateTime.UtcNow;
        await _unitOfWork.Baskets.UpdateAsync(basket);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveCachedDataAsync(GetBasketCacheKey(GetBasketOwnerId(basket)));
        return new BasketActionResultDto { BasketId = basketId, Message = "Basket is validated and ready to convert to an order." };
    }

    public async Task<BasketActionResultDto> RemindBasketOwnerAsync(string basketId)
    {
        var basket = await _unitOfWork.Baskets.GetByIdAsync(basketId) ?? throw new KeyNotFoundException($"Basket with ID {basketId} not found.");
        basket.LastReminderAt = DateTime.UtcNow;
        basket.Status = "reminded";
        basket.ModifiedTime = DateTime.UtcNow;
        await _unitOfWork.Baskets.UpdateAsync(basket);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveCachedDataAsync(GetBasketCacheKey(GetBasketOwnerId(basket)));
        return new BasketActionResultDto { BasketId = basketId, Message = "Basket reminder registered for delivery pipeline." };
    }

    private async Task<Basket> GetOrCreateBasketFromDbAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("A user or guest identifier is required.", nameof(userId));
        }

        // A route identifier can represent an anonymous visitor.  Only assign
        // UserId when that identifier exists in Users; assigning a guest value
        // to UserId violates FK_Baskets_Users_UserId.
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        var basket = await _unitOfWork.Baskets.GetByUserIdAsync(user?.Id ?? string.Empty, user is null ? userId : string.Empty);
        if (basket != null)
        {
            return basket;
        }

        basket = user is null
            ? new Basket { Id = Guid.NewGuid().ToString(), GuestId = userId, Type = BasketType.Guest }
            : new Basket { Id = Guid.NewGuid().ToString(), UserId = user.Id, Type = BasketType.User };
        await _unitOfWork.Baskets.AddAsync(basket);
        await _unitOfWork.SaveChangesAsync();
        return basket;
    }

    private async Task<BasketDto> CacheAndMapBasketAsync(Basket basket)
    {
        var basketDto = _mapper.Map<BasketDto>(basket);
        await _cacheService.SetCachedDataAsync(GetBasketCacheKey(GetBasketOwnerId(basket)), basketDto, BasketCacheDuration);
        return basketDto;
    }

    private static string GetBasketOwnerId(Basket basket) => basket.UserId ?? basket.GuestId ?? basket.Id;

    private static string GetBasketCacheKey(string userId) => $"{BasketCacheKeyPrefix}{userId}";
}
