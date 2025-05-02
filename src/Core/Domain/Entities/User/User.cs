namespace Domain.Entities;

public class User : BaseModel<string>
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }

    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }


    public DateTime? DateOfBirth { get; set; }
    public string ProfilePictureUrl { get; set; }
    public bool IsActive { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTime? LastLoginTime { get; set; }
    public bool IsEmailConfirmed { get; set; }

    //public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = new Supplier();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();
    public ICollection<ProductReview> Reviewer { get; set; } = new List<ProductReview>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public ICollection<Basket> Basket { get; set; } = new List<Basket>();
}