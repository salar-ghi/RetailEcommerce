namespace Domain.Entities;

public class ProductPromotion
{
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public int PromotionId { get; set; }
    public Promotion Promotion { get; set; }
}
