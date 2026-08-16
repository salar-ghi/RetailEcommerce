namespace Application.Services;

public class BasketService : IBasketService
{
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
        var cachedBasket = await _cacheService.GetCachedDataAsync<Basket>(userId);
        if (cachedBasket != null)
        {
            return _mapper.Map<BasketDto>(cachedBasket);
        }

        var basket = await _unitOfWork.Baskets.GetByUserIdAsync(userId, null);
        if (basket == null)
        {
            basket = new Basket { Id = userId, UserId = userId };
            await _unitOfWork.Baskets.AddAsync(basket);
            await _unitOfWork.SaveChangesAsync();
        }

        await _cacheService.SetCachedDataAsync<Basket>(basket.Id, basket, TimeSpan.FromDays(1));
        return _mapper.Map<BasketDto>(basket);
    }

    public async Task AddItemToBasketAsync(string userId, int productId, int quantity)
    {
        var basket = await GetBasketFromCacheOrDbAsync(userId);
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
        }
        else
        {
            basket.Items.Add(new BasketItem
            {
                ProductId = productId,
                Quantity = quantity,
                UnitPrice= unitPrice,
                BasketId = basket.Id
            });
        }

        await _unitOfWork.Baskets.UpdateAsync(basket);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.SetCachedDataAsync<Basket>(basket.Id, basket, TimeSpan.FromDays(1));
    }

    public async Task UpdateItemQuantityAsync(string userId, int productId, int quantity)
    {
        var basket = await GetBasketFromCacheOrDbAsync(userId);
        var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
        {
            throw new KeyNotFoundException($"Item with Product ID {productId} not found.");
        }

        item.Quantity = quantity;
        await _unitOfWork.Baskets.UpdateAsync(basket);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.SetCachedDataAsync<Basket>(basket.Id, basket, TimeSpan.FromDays(1));
    }

    public async Task RemoveItemFromBasketAsync(string userId, int productId)
    {
        var basket = await GetBasketFromCacheOrDbAsync(userId);
        var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            basket.Items.Remove(item);
            await _unitOfWork.Baskets.UpdateAsync(basket);
            await _unitOfWork.SaveChangesAsync();
            await _cacheService.SetCachedDataAsync<Basket>(basket.Id, basket, TimeSpan.FromDays(1));
        }
    }

    public async Task ClearBasketAsync(string userId)
    {
        var basket = await _unitOfWork.Baskets.GetByUserIdAsync(userId, string.Empty);
        if (basket != null)
        {
            await _unitOfWork.Baskets.DeleteAsync(basket);
            await _unitOfWork.SaveChangesAsync();
            await _cacheService.RemoveCachedDataAsync(basket.Id);
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
            if (requestedItem.Quantity <= 0) basket.Items.Remove(item); else item.Quantity = requestedItem.Quantity;
        }

        basket.ModifiedTime = DateTime.UtcNow;
        await _unitOfWork.Baskets.UpdateAsync(basket);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveCachedDataAsync(basket.UserId ?? basket.Id);
        return _mapper.Map<BasketDto>(basket);
    }

    public async Task<BasketActionResultDto> DeleteBasketAsync(string basketId)
    {
        var basket = await _unitOfWork.Baskets.GetByIdAsync(basketId, q => q.Include(b => b.Items)) ?? throw new KeyNotFoundException($"Basket with ID {basketId} not found.");
        await _unitOfWork.Baskets.DeleteAsync(basket);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveCachedDataAsync(basket.UserId ?? basket.Id);
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
        return new BasketActionResultDto { BasketId = basketId, Message = "Basket reminder registered for delivery pipeline." };
    }

    private async Task<Basket> GetBasketFromCacheOrDbAsync(string userId)
    {
        var cachedBasket = await _cacheService.GetCachedDataAsync<Basket>(userId);
        if (cachedBasket != null)
        {
            return cachedBasket;
        }

        var basket = await _unitOfWork.Baskets.GetByUserIdAsync(userId, string.Empty);
        if (basket == null)
        {
            basket = new Basket { Id = userId, UserId = userId };
            await _unitOfWork.Baskets.AddAsync(basket);
            await _unitOfWork.SaveChangesAsync();
        }

        await _cacheService.SetCachedDataAsync<Basket>(basket.Id, basket, TimeSpan.FromDays(1));
        return basket;
    }
}