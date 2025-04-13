namespace Domain.Entities;

public class UserAddress : BaseModel<int>
{
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }
    public bool IsPrimary { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}