namespace Application.DTOs;

public class DiscountDto
{
    public int Id { get; set; }
    public string Type { get; set; }
    public decimal Value { get; set; }
}

public class PromotionDiscountDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string DiscountType { get; set; }
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal? MinimumOrder { get; set; }
    public int? MaxUsage { get; set; }
    public int UsedCount { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public string Status { get; set; }
    public string Scope { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateDiscountRequestDto
{
    public string Code { get; set; }
    public string DiscountType { get; set; }
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal? MinimumOrder { get; set; }
    public int? MaxUsage { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; } = true;
    public string Scope { get; set; } = "general";
    public List<long> ProductIds { get; set; } = new();
    public List<int> CategoryIds { get; set; } = new();
}

public class UpdateDiscountRequestDto
{
    public string Code { get; set; }
    public string DiscountType { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MinimumOrder { get; set; }
    public int? MaxUsage { get; set; }
    public string Description { get; set; }
    public bool? IsActive { get; set; }
    public string Scope { get; set; }
    public List<long> ProductIds { get; set; }
    public List<int> CategoryIds { get; set; }
}

public class DiscountCalculationRequestDto
{
    public string Code { get; set; }
    public decimal OrderTotal { get; set; }
    public List<DiscountCalculationItemDto> Items { get; set; } = new();
}

public class DiscountCalculationItemDto
{
    public long ProductId { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
}

public class DiscountCalculationResultDto
{
    public string Code { get; set; }
    public bool IsApplicable { get; set; }
    public string Message { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalTotal { get; set; }
    public List<DiscountCalculationLineDto> Lines { get; set; } = new();
}

public class DiscountCalculationLineDto
{
    public long ProductId { get; set; }
    public decimal LineSubtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalLineTotal { get; set; }
}
