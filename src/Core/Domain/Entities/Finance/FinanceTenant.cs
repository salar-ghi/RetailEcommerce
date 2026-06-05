namespace Domain.Entities;

public class FinanceTenant : BaseModel<string>
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public FinanceCurrency BaseCurrency { get; set; } = FinanceCurrency.IRR;
    public bool IsActive { get; set; } = true;
    public ICollection<FinanceBranch> Branches { get; set; } = new List<FinanceBranch>();
}
