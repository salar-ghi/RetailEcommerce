namespace Domain.Entities;

public class OrderPromotion
{
    public string OrderId { get; set; }
    public Order Order { get; set; }
    public int PromotionId { get; set; }
    public Promotion Promotion { get; set; }
}
