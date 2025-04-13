namespace Application.Interfaces;

public interface IPromotionService
{
    Task<PromotionDto> GetPromotionAsync(int promotionId);
    Task<IEnumerable<PromotionDto>> GetActivePromotionsAsync();
    Task ApplyPromotionsToOrderAsync(Order order);
    Task ApplyPromotionsToProductAsync(Product product);
    //Task ApplyPromotionsToCategoryAsync(Category category);
}
