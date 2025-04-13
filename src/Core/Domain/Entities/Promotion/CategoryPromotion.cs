namespace Domain.Entities;

public class CategoryPromotion
{
    public Guid PromotionId { get; set; }
    public Guid CategoryId { get; set; }
    public Promotion Promotion { get; set; }
    public Category Category { get; set; }
}
