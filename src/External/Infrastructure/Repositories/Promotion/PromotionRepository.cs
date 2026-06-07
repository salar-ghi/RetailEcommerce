namespace Infrastructure.Repositories;

public class PromotionRepository : Repository<Promotion, int>, IPromotionRepository
{
    public PromotionRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync()
    {
        return await QueryWithDetails(asNoTracking: true)
            .Where(p => p.IsEnabled && DateTime.UtcNow >= p.StartDate && DateTime.UtcNow <= p.EndDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Promotion>> GetPromotionsWithDetailsAsync()
    {
        return await QueryWithDetails(asNoTracking: true)
            .OrderByDescending(p => p.CreatedTime)
            .ToListAsync();
    }

    public async Task<Promotion> GetPromotionWithDetailsAsync(int promotionId, bool asNoTracking = true)
    {
        return await QueryWithDetails(asNoTracking)
            .FirstOrDefaultAsync(p => p.Id == promotionId);
    }

    public async Task<Promotion> GetPromotionByCodeAsync(string code, bool asNoTracking = true)
    {
        var normalizedCode = NormalizeCode(code);
        return await QueryWithDetails(asNoTracking)
            .FirstOrDefaultAsync(p => p.Code == normalizedCode);
    }

    public async Task<bool> PromotionCodeExistsAsync(string code)
    {
        var normalizedCode = NormalizeCode(code);
        return !string.IsNullOrWhiteSpace(normalizedCode)
            && await _context.Promotions.AsNoTracking().AnyAsync(p => p.Code == normalizedCode);
    }

    private IQueryable<Promotion> QueryWithDetails(bool asNoTracking)
    {
        var query = _context.Promotions
            .Include(p => p.Conditions)
            .Include(p => p.Discounts)
            .Include(p => p.Products)
            .Include(p => p.Categories)
            .AsSplitQuery();

        return asNoTracking ? query.AsNoTracking() : query.AsTracking();
    }

    private static string NormalizeCode(string code) => code?.Trim().ToUpperInvariant();
}
