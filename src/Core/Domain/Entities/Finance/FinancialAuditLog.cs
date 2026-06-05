namespace Domain.Entities;

public class FinancialAuditLog : BaseModel<string>
{
    public string TenantId { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? BeforeJson { get; set; }
    public string? AfterJson { get; set; }
    public string? CorrelationId { get; set; }
    public string? IpAddress { get; set; }
}
