namespace Domain.Entities;

public class PayrollLine : BaseModel<string>
{
    public string PayrollRunId { get; set; } = string.Empty;
    public PayrollRun PayrollRun { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string? Position { get; set; }
    public string? BranchId { get; set; }
    public FinanceBranch? Branch { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal Bonus { get; set; }
    public decimal Deductions { get; set; }
    public decimal TaxWithheld { get; set; }
    public decimal InsuranceWithheld { get; set; }
    public decimal NetPay { get; set; }
}
