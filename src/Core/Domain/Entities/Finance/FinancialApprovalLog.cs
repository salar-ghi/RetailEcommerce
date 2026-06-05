namespace Domain.Entities;

public class FinancialApprovalLog : BaseModel<string>
{
    public string TenantId { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string ApproverUserId { get; set; } = string.Empty;
    public ApprovalDecision Decision { get; set; } = ApprovalDecision.Pending;
    public string? Note { get; set; }
    public DateTime DecisionDate { get; set; }
    public decimal? AmountThreshold { get; set; }
}
