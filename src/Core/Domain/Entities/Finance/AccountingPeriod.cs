namespace Domain.Entities;

public class AccountingPeriod : BaseModel<string>
{
    public string TenantId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime StartsOn { get; set; }
    public DateTime EndsOn { get; set; }
    public bool IsClosed { get; set; }
    public DateTime? ClosedOn { get; set; }
    public string? ClosedBy { get; set; }
}
