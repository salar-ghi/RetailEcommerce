
namespace Infrastructure.Repositories;

public class BasketRepository : Repository<Basket, string>, IBasketRepository
{
    public BasketRepository(AppDbContext context) : base(context) { }

    public Task AddItemAsync(Basket basket, Guid productId, int quantity)
    {
        throw new NotImplementedException();
    }

    public Task ApplyPromotionAsync(Basket basket, string promotionCode)
    {
        throw new NotImplementedException();
    }

    public async Task<Basket> GetByUserIdAsync(string userId)
    {
        return await _context.Baskets
            .AsNoTracking()
            .Include(b => b.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(b => b.UserId == userId);
    }

    public Task<Basket> GetByUserIdAsync(string userId, string? guestId)
    {
        throw new NotImplementedException();
    }

    public Task MergeBasketsAsync(string userBasketId, string guestBasketId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveItemAsync(Basket basket, Guid basketItemId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateItemQuantityAsync(Basket basket, Guid basketItemId, int newQuantity)
    {
        throw new NotImplementedException();
    }



    //public async Task<Basket> GetBasketAsync(string basketId)
    //{
    //    // Check Redis cache first
    //    var cachedBasket = await _cache.GetStringAsync(basketId);
    //    if (!string.IsNullOrEmpty(cachedBasket))
    //    {
    //        return JsonSerializer.Deserialize<Basket>(cachedBasket);
    //    }

    //    // If not in cache, retrieve from database
    //    var basket = await _context.Baskets
    //        .Include(b => b.Items)
    //        .ThenInclude(i => i.Product)
    //        .FirstOrDefaultAsync(b => b.Id == basketId);

    //    if (basket != null)
    //    {
    //        // Cache the basket in Redis
    //        await _cache.SetStringAsync(basketId, JsonSerializer.Serialize(basket), new DistributedCacheEntryOptions
    //        {
    //            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) // Cache for 30 minutes
    //        });
    //    }

    //    return basket;
    //}

    //public async Task AddBasketAsync(Basket basket)
    //{
    //    await _context.Baskets.AddAsync(basket);
    //    await _context.SaveChangesAsync();
    //    // Cache the new basket
    //    await _cache.SetStringAsync(basket.Id, JsonSerializer.Serialize(basket));
    //}

    //public async Task UpdateBasketAsync(Basket basket)
    //{
    //    _context.Baskets.Update(basket);
    //    await _context.SaveChangesAsync();
    //    // Update cache
    //    await _cache.SetStringAsync(basket.Id, JsonSerializer.Serialize(basket));
    //}

    //public async Task DeleteBasketAsync(string basketId)
    //{
    //    var basket = await _context.Baskets.FindAsync(basketId);
    //    if (basket != null)
    //    {
    //        _context.Baskets.Remove(basket);
    //        await _context.SaveChangesAsync();
    //        // Remove from cache
    //        await _cache.RemoveAsync(basketId);
    //    }
    //}
}