using System.Collections.Concurrent;

namespace Application.Services;

/// <summary>
/// Redis-backed active basket service. Redis is the sole store for basket state;
/// the product repository is consulted only to snapshot product details when an
/// item is first added or its quantity is increased.
/// </summary>
public sealed class BasketService : IBasketService
{
    private static readonly TimeSpan BasketLifetime = TimeSpan.FromDays(1);
    private const string BasketKeyPrefix = "basket:";
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> BasketLocks = new();

    private readonly IProductRepository _products;
    private readonly IRedisCacheService _cache;

    public BasketService(IProductRepository products, IRedisCacheService cache)
    {
        _products = products;
        _cache = cache;
    }

    public async Task<BasketDto> GetBasketAsync(string ownerId)
    {
        ValidateOwnerId(ownerId);
        return await WithBasketLockAsync(ownerId, async () =>
        {
            var basket = await GetCachedBasketAsync(ownerId) ?? await CreateAndSaveBasketAsync(ownerId);
            if (PopulateClientImages(basket))
                await SaveBasketAsync(ownerId, basket);

            return basket;
        });
    }

    public async Task<BasketDto> AddItemToBasketAsync(string ownerId, long productId, int quantity)
    {
        ValidateOwnerId(ownerId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

        var product = await _products.GetByIdAsync(productId, query => query
            .Include(p => p.Batches)
            .Include(p => p.Images))
            ?? throw new KeyNotFoundException($"Product with ID {productId} was not found.");
        var unitPrice = product.Batches.FirstOrDefault()?.SellingPrice ?? 0;
        var coverImage = product.Images
            .OrderByDescending(image => image.IsPrimary)
            .ThenBy(image => image.Id)
            .Select(image => image.ImageUrl)
            .FirstOrDefault();

        return await UpdateBasketAsync(ownerId, basket =>
        {
            var item = basket.Items.FirstOrDefault(existing => existing.ProductId == productId);
            if (item is null)
            {
                basket.Items.Add(new BasketItemDto
                {
                    Id = productId,
                    ProductId = productId,
                    ProductName = product.Name,
                    CoverImage = coverImage,
                    Image = coverImage,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                });
            }
            else
            {
                item.Quantity += quantity;
                item.ProductName = product.Name;
                item.CoverImage = coverImage;
                item.Image = coverImage;
                item.UnitPrice = unitPrice;
            }
        });
    }

    public Task<BasketDto> UpdateItemQuantityAsync(string ownerId, long productId, int quantity)
    {
        ValidateOwnerId(ownerId);
        ArgumentOutOfRangeException.ThrowIfNegative(quantity);

        return UpdateBasketAsync(ownerId, basket =>
        {
            var item = basket.Items.FirstOrDefault(existing => existing.ProductId == productId)
                ?? throw new KeyNotFoundException($"Item with Product ID {productId} was not found.");

            if (quantity == 0)
                basket.Items.Remove(item);
            else
                item.Quantity = quantity;
        }, createIfMissing: false);
    }

    public Task<BasketDto> RemoveItemFromBasketAsync(string ownerId, long productId)
    {
        ValidateOwnerId(ownerId);
        return UpdateBasketAsync(ownerId, basket => basket.Items.RemoveAll(item => item.ProductId == productId));
    }

    public async Task ClearBasketAsync(string ownerId)
    {
        ValidateOwnerId(ownerId);
        await WithBasketLockAsync(ownerId, () => _cache.RemoveCachedDataAsync(GetBasketKey(ownerId)));
    }

    private async Task<BasketDto> UpdateBasketAsync(string ownerId, Action<BasketDto> update, bool createIfMissing = true)
    {
        return await WithBasketLockAsync(ownerId, async () =>
        {
            var basket = await GetCachedBasketAsync(ownerId);
            if (basket is null)
            {
                if (!createIfMissing)
                    throw new KeyNotFoundException($"Basket for owner {ownerId} was not found.");

                basket = CreateBasket(ownerId);
            }

            update(basket);
            basket.ModifiedTime = DateTime.UtcNow;
            RefreshTotals(basket);
            await SaveBasketAsync(ownerId, basket);
            return basket;
        });
    }

    private async Task<BasketDto?> GetCachedBasketAsync(string ownerId) =>
        await _cache.GetCachedDataAsync<BasketDto>(GetBasketKey(ownerId));

    private async Task<BasketDto> CreateAndSaveBasketAsync(string ownerId)
    {
        var basket = CreateBasket(ownerId);
        await SaveBasketAsync(ownerId, basket);
        return basket;
    }

    private Task SaveBasketAsync(string ownerId, BasketDto basket) =>
        _cache.SetCachedDataAsync(GetBasketKey(ownerId), basket, BasketLifetime);

    private static BasketDto CreateBasket(string ownerId) => new()
    {
        Id = Guid.NewGuid().ToString("N"),
        UserId = ownerId,
        GuestId = ownerId,
        CreatedTime = DateTime.UtcNow,
        ModifiedTime = DateTime.UtcNow
    };

    private static void RefreshTotals(BasketDto basket)
    {
        basket.TotalItems = basket.Items.Sum(item => item.Quantity);
        basket.TotalPrice = basket.Items.Sum(item => item.Quantity * item.UnitPrice);
    }

    private static bool PopulateClientImages(BasketDto basket)
    {
        var updated = false;
        foreach (var item in basket.Items.Where(item => string.IsNullOrWhiteSpace(item.Image) && !string.IsNullOrWhiteSpace(item.CoverImage)))
        {
            item.Image = item.CoverImage;
            updated = true;
        }

        return updated;
    }

    private static void ValidateOwnerId(string ownerId)
    {
        if (string.IsNullOrWhiteSpace(ownerId))
            throw new ArgumentException("A user or guest identifier is required.", nameof(ownerId));
    }

    private static async Task<T> WithBasketLockAsync<T>(string ownerId, Func<Task<T>> action)
    {
        var basketLock = BasketLocks.GetOrAdd(ownerId, _ => new SemaphoreSlim(1, 1));
        await basketLock.WaitAsync();
        try
        {
            return await action();
        }
        finally
        {
            basketLock.Release();
        }
    }

    private static async Task WithBasketLockAsync(string ownerId, Func<Task> action)
    {
        await WithBasketLockAsync(ownerId, async () =>
        {
            await action();
            return true;
        });
    }

    private static string GetBasketKey(string ownerId) => $"{BasketKeyPrefix}{ownerId}";
}
