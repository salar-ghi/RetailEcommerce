namespace Application.Interfaces;

public interface IPromotionService
{
    Task<PromotionDto> GetPromotionAsync(int promotionId);
    Task<IEnumerable<PromotionDto>> GetActivePromotionsAsync();
    Task<IEnumerable<PromotionDiscountDto>> GetAllDiscountsAsync();
    Task<PromotionDiscountDto> GetDiscountAsync(int discountId);
    Task<PromotionDiscountDto> CreateDiscountAsync(CreateDiscountRequestDto request);
    Task<PromotionDiscountDto> UpdateDiscountAsync(int discountId, UpdateDiscountRequestDto request);
    Task DeleteDiscountAsync(int discountId);
    Task<DiscountCalculationResultDto> CalculateDiscountAsync(DiscountCalculationRequestDto request);
    Task ApplyPromotionsToOrderAsync(Order order);
    Task ApplyPromotionsToProductAsync(Product product);
}
