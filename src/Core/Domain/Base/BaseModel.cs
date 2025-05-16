namespace Domain;

public abstract class BaseModel<TId>
{
    public TId Id { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.Now;
    public string ModifiedBy { get; set; }
    public DateTime ModifiedTime { get; set; } = DateTime.Now;
}

