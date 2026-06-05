namespace Domain.Entities;

public class CostCenter : BaseModel<string>
{
    public string TenantId { get; set; } = string.Empty;
    public string? BranchId { get; set; }
    public FinanceBranch? Branch { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ParentCostCenterId { get; set; }
    public CostCenter? ParentCostCenter { get; set; }
    public bool IsActive { get; set; } = true;
}
