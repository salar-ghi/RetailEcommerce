namespace Domain.IRepositories;

public interface IPromotionRepository : IRepository<Promotion, int>
{
    Task<IEnumerable<Promotion>> GetActivePromotionsAsync();
    Task<Promotion> GetPromotionWithDetailsAsync(int promotionId);
}
