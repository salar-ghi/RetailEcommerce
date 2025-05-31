namespace Application.DTOs;

public class SupplierDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ContactName { get; set; }
    public string ContactInfo { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhone { get; set; }
    public string Address { get; set; }
    public string Status { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string UserId { get; set; }
}


public class SupplierRegistrationDto
{
    public string Name { get; set; }
    public string ContactInfo { get; set; }
    public string ContactName { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhone { get; set; }
    public string Address { get; set; }
    // User details (for new users)
    public string PhoneNumber { get; set; } // For user registration
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

public class ApproveSupplierDto
{
    public int SupplierId { get; set; }
    public string ApprovedByUserId { get; set; }
    public bool IsApproved { get; set; }
}
