namespace Infrastructure.Persistence;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    public AuditInterceptor(ICurrentUserService currentUserService)
        => _currentUserService = currentUserService;

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, 
        InterceptionResult<int> result)
    {
        ApplyAudit(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAudit(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAudit(DbContext? context)
    {
        if (context is null) return;

        var now = DateTime.UtcNow;
        var userId = _currentUserService.UserId ?? "system";

        var entries = context.ChangeTracker.Entries<IAuditableEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = userId;
                entry.Entity.CreatedTime = now;
                entry.Entity.ModifiedBy = userId;
                entry.Entity.ModifiedTime = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedBy = userId;
                entry.Entity.ModifiedTime = now;

                // Keep original creation values
                entry.Property(x => x.CreatedBy).IsModified = false;
                entry.Property(x => x.CreatedTime).IsModified = false;
            }
            else if (entry.State == EntityState.Deleted && entry.Entity is IAuditableEntity deletable)
            {
                // Soft‑delete instead of hard‑delete
                entry.State = EntityState.Modified;
                deletable.IsDeleted = true;
                deletable.ModifiedBy = userId;
                deletable.ModifiedTime = now;
            }
        }
    }
}
