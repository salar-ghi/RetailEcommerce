namespace Domain.Common;

public interface IAuditableEntity
{
    string? CreatedBy { get; set; }
    DateTime CreatedTime { get; set; }
    string? ModifiedBy { get; set; }
    DateTime ModifiedTime { get; set; }
    bool IsDeleted { get; set; }
}
