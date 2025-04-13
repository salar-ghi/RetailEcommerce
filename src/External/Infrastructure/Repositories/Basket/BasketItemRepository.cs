namespace Infrastructure.Repositories;

internal class BasketItemRepository : Repository<BasketItem, int>, IBasketItemRepository
{
    public BasketItemRepository(AppDbContext context) : base(context) { }
    public async Task<IEnumerable<BasketItem>> GetByBasketIdAsync(string basketId)
    {
        return await _context.BasketItems
            .Where(bi => bi.BasketId == basketId)
            .Include(bi => bi.Product)
            .ToListAsync();
    }
}
