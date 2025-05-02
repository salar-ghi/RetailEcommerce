namespace Domain.Entities;

public class CategoryPromotion
{
    public int PromotionId { get; set; }
    public Promotion Promotion { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; }
}
