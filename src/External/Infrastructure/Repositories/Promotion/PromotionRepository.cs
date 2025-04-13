namespace Infrastructure.Repositories;

public class PromotionRepository : Repository<Promotion, int>, IPromotionRepository
{
    public PromotionRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync()
    {
        return await _context.Promotions
            .AsNoTracking()
            .Where(p => p.IsActive)
            .Include(p => p.Conditions)
            .Include(p => p.Discounts)
            .ToListAsync();
    }

    public async Task<Promotion> GetPromotionWithDetailsAsync(int promotionId)
    {
        return await _context.Promotions
            .AsNoTracking()
            .Include(p => p.Conditions)
            .Include(p => p.Discounts)
            .FirstOrDefaultAsync(p => p.Id == promotionId);
    }
}