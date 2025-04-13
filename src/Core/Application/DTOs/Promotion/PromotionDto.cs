namespace Application.DTOs;

public class PromotionDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public string Scope { get; set; }
    public List<PromotionConditionDto> Conditions { get; set; } = new List<PromotionConditionDto>();
    public List<DiscountDto> Discounts { get; set; } = new List<DiscountDto>();
}
