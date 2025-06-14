﻿namespace Domain.Entities;

public class Supplier : BaseModel<int>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Info { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public SupplierStatus Status { get; set; } = SupplierStatus.Pending;
    public DateTime? ApprovalDate { get; set; }
    public string ApprovedByUserId { get; set; } = string.Empty;
    public string UserId { get; set; }
    public User User { get; set; }
    public ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}