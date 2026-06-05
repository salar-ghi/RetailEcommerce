namespace Domain.Entities;

public class FinanceBranch : BaseModel<string>
{
    public string TenantId { get; set; } = string.Empty;
    public FinanceTenant Tenant { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public FinanceBranchType Type { get; set; }
    public string? City { get; set; }
    public string? ManagerUserId { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<FinanceAccount> FinanceAccounts { get; set; } = new List<FinanceAccount>();
    public ICollection<CostCenter> CostCenters { get; set; } = new List<CostCenter>();
}
