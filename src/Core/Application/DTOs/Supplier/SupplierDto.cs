namespace Application.DTOs;

public class SupplierDto
{
    public int Id { get; set; }
    public string SupplierName { get; set; }
    public string SupplierInfo { get; set; }
    public string SupplierEmail { get; set; }
    public string SupplierPhone { get; set; }
    public string Address { get; set; }
    public string Status { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string UserId { get; set; }
}


public class SupplierRegistrationDto
{
    
    public string Name { get; set; }
    public string Email { get; set; }
    public string ContactInfo { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Website { get; set; }
    public string Description { get; set; }
}

public class ApproveSupplierDto
{
    public int Id { get; set; }
    public string ApprovedByUserId { get; set; }
    public bool IsApproved { get; set; }
}

public class UpdateSupplierStatusDto
{
    public int Id { get; set; }
    public SupplierStatus? Status { get; set; }
    public string? SupplierPhone { get; set; }
    public string? SupplierName { get; set; }
    public string? SupplierInfo { get; set; }
    public string? SupplierEmail { get; set; }
}