namespace Domain.Entities;

public class ChartOfAccount : BaseModel<string>
{
    public string TenantId { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public LedgerAccountType Type { get; set; }
    public LedgerAccountNormalBalance NormalBalance { get; set; }
    public string? ParentAccountId { get; set; }
    public ChartOfAccount? ParentAccount { get; set; }
    public bool IsPostingAllowed { get; set; } = true;
    public bool IsSystemAccount { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<ChartOfAccount> Children { get; set; } = new List<ChartOfAccount>();
    public ICollection<JournalEntryLine> JournalEntryLines { get; set; } = new List<JournalEntryLine>();
}
