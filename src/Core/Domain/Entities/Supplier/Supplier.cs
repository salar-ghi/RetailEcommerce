namespace Domain.Entities;

public class Supplier : BaseModel<int>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Info { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SupplierStatus Status { get; set; } = SupplierStatus.Pending;
    public DateTime? ApprovalDate { get; set; }
    public string ApprovedByUserId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; }
    public ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}