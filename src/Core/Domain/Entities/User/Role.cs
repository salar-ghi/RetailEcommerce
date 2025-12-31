namespace Domain.Entities;

public class Role : BaseModel<int>
{
    public string Name { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
