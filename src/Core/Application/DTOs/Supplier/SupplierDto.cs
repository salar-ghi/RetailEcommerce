namespace Application.DTOs;

public class SupplierDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ContactInfo { get; set; }
    public string ContactName { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhone { get; set; }
    public string Address { get; set; }
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
    public string UserEmail { get; set; }
    public string UserPassword { get; set; }
}