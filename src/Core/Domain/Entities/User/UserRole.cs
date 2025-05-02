namespace Domain.Entities;

public class UserRole : BaseModel<int>
{
    public int RoleId { get; set; }
    public Role Role { get; set; }

    public string UserId { get; set; }
    public User User { get; set; }
}