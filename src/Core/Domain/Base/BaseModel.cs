namespace Domain;

public abstract class BaseModel<TId> : IAuditableEntity
{
    public TId Id { get; set; }
    public string? CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    public string ModifiedBy { get; set; } = string.Empty;
    public DateTime ModifiedTime { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; }

}