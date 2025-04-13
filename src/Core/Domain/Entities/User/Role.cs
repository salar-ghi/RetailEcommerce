namespace Domain.Entities;

public class Role : BaseModel<int>
{
    public string Name { get; set; } // Role name (e.g., "Customer", "Admin")

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
