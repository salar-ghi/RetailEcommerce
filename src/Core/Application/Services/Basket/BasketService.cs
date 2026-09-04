using System.Collections.Concurrent;

namespace Application.Services;

public class BasketService : IBasketService
{
    private static readonly TimeSpan BasketCacheDuration = TimeSpan.FromDays(1);
    private const string BasketCacheKeyPrefix = "basket:";
    // Redis is the source of truth for storefront baskets. Serializing writes for
    // a basket prevents quick consecutive clicks from overwriting each other in a
    // single application instance.
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> BasketLocks = new();

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
        ValidateBasketOwner(userId);
        var basket = await GetCachedBasketAsync(userId);
        if (basket != null) return basket;

        basket = CreateBasket(userId);
        await SaveBasketAsync(userId, basket);
        return basket;
    }

    public async Task AddItemToBasketAsync(string userId, int productId, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }

        var product = await _unitOfWork.Products.GetByIdAsync(productId,
            include: q => q.Include(p => p.Batches));
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {productId} not found.");

        decimal unitPrice = product.Batches.Any()
                        ? product.Batches.First().SellingPrice
                        : 0;

        await WithBasketLockAsync(userId, async () =>
        {
            var basket = await GetOrCreateCachedBasketAsync(userId);
            var existingItem = basket.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.UnitPrice = unitPrice;
                existingItem.ProductName = product.Name;
            }
            else
            {
                basket.Items.Add(new BasketItemDto
                {
                    Id = productId,
                    ProductId = productId,
                    ProductName = product.Name,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                });
            }

            basket.ModifiedTime = DateTime.UtcNow;
            RefreshTotals(basket);
            await SaveBasketAsync(userId, basket);
        });
    }

    public async Task UpdateItemQuantityAsync(string userId, int productId, int quantity)
    {
        if (quantity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity cannot be negative.");
        }

        await WithBasketLockAsync(userId, async () =>
        {
            var basket = await GetCachedBasketAsync(userId)
                ?? throw new KeyNotFoundException($"Basket for owner {userId} was not found.");
            var item = basket.Items.FirstOrDefault(i => i.ProductId == productId)
                ?? throw new KeyNotFoundException($"Item with Product ID {productId} not found.");

            if (quantity == 0) basket.Items.Remove(item);
            else item.Quantity = quantity;

            basket.ModifiedTime = DateTime.UtcNow;
            RefreshTotals(basket);
            await SaveBasketAsync(userId, basket);
        });
    }

    public async Task RemoveItemFromBasketAsync(string userId, int productId)
    {
        await WithBasketLockAsync(userId, async () =>
        {
            var basket = await GetCachedBasketAsync(userId);
            if (basket == null) return;

            basket.Items.RemoveAll(item => item.ProductId == productId);
            basket.ModifiedTime = DateTime.UtcNow;
            RefreshTotals(basket);
            await SaveBasketAsync(userId, basket);
        });
    }

    public async Task ClearBasketAsync(string userId)
    {
        ValidateBasketOwner(userId);
        await WithBasketLockAsync(userId, () => _cacheService.RemoveCachedDataAsync(GetBasketCacheKey(userId)));
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

    private async Task<BasketDto?> GetCachedBasketAsync(string userId) =>
        await _cacheService.GetCachedDataAsync<BasketDto>(GetBasketCacheKey(userId));

    private async Task<BasketDto> GetOrCreateCachedBasketAsync(string userId) =>
        await GetCachedBasketAsync(userId) ?? CreateBasket(userId);

    private async Task SaveBasketAsync(string userId, BasketDto basket)
    {
        await _cacheService.SetCachedDataAsync(GetBasketCacheKey(userId), basket, BasketCacheDuration);
    }

    private static BasketDto CreateBasket(string userId) => new()
    {
        Id = Guid.NewGuid().ToString(),
        UserId = userId,
        GuestId = userId,
        CreatedTime = DateTime.UtcNow,
        ModifiedTime = DateTime.UtcNow
    };

    private static void RefreshTotals(BasketDto basket)
    {
        basket.TotalItems = basket.Items.Sum(item => item.Quantity);
        basket.TotalPrice = basket.Items.Sum(item => item.Quantity * item.UnitPrice);
    }

    private static void ValidateBasketOwner(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("A user or guest identifier is required.", nameof(userId));
        }
    }

    private static async Task WithBasketLockAsync(string userId, Func<Task> action)
    {
        ValidateBasketOwner(userId);
        var basketLock = BasketLocks.GetOrAdd(userId, _ => new SemaphoreSlim(1, 1));
        await basketLock.WaitAsync();
        try
        {
            await action();
        }
        finally
        {
            basketLock.Release();
        }
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
