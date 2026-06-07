namespace Domain.IRepositories;

public interface IPromotionRepository : IRepository<Promotion, int>
{
    Task<IEnumerable<Promotion>> GetActivePromotionsAsync();
    Task<IEnumerable<Promotion>> GetPromotionsWithDetailsAsync();
    Task<Promotion> GetPromotionWithDetailsAsync(int promotionId, bool asNoTracking = true);
    Task<Promotion> GetPromotionByCodeAsync(string code, bool asNoTracking = true);
}
