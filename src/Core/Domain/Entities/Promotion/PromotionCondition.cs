namespace Domain.Entities;

public class PromotionCondition : BaseModel<int>
{
    public ConditionType Type { get; set; }
    public string Value { get; set; } // Flexible storage (e.g., "100" for min order amount)
    public int PromotionId { get; set; }
    public Promotion Promotion { get; set; }
}