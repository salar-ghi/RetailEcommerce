namespace Domain.Entities;

public class JournalEntry : BaseModel<string>
{
    public string TenantId { get; set; } = string.Empty;
    public string JournalNumber { get; set; } = string.Empty;
    public DateTime AccountingDate { get; set; }
    public JournalEntryStatus Status { get; set; } = JournalEntryStatus.Draft;
    public FinanceSourceDocumentType SourceDocumentType { get; set; } = FinanceSourceDocumentType.ManualJournal;
    public string? SourceDocumentId { get; set; }
    public string? BranchId { get; set; }
    public FinanceBranch? Branch { get; set; }
    public string? Description { get; set; }
    public DateTime? PostedOn { get; set; }
    public string? PostedBy { get; set; }
    public string? ReversalOfJournalEntryId { get; set; }
    public JournalEntry? ReversalOfJournalEntry { get; set; }
    public ICollection<JournalEntryLine> Lines { get; set; } = new List<JournalEntryLine>();

    [NotMapped]
    public decimal TotalDebit => Lines.Sum(line => line.DebitAmount);

    [NotMapped]
    public decimal TotalCredit => Lines.Sum(line => line.CreditAmount);

    [NotMapped]
    public bool IsBalanced => TotalDebit == TotalCredit && Lines.Count >= 2;
}
