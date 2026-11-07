namespace Infrastructure.EntityTypeConfiguration;

public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
{
    public void Configure(EntityTypeBuilder<UserActivity> builder)
    {
        builder.ToTable("UserActivities");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Platform)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.UserId)
            .HasMaxLength(150);

        builder.Property(x => x.SessionId)
            .HasMaxLength(150);

        builder.Property(x => x.EntityType)
            .HasMaxLength(100);

        builder.Property(x => x.EntityId)
            .HasMaxLength(100);

        builder.Property(x => x.Path)
            .HasMaxLength(500);

        builder.Property(x => x.Method)
            .HasMaxLength(20);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(100);

        builder.Property(x => x.UserAgent)
            .HasMaxLength(500);

        builder.Property(x => x.MetadataJson)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.CreatedTime)
            .IsRequired();

        // Indexes for high performance querying & analytics / recommendation pipelines
        builder.HasIndex(x => x.CreatedTime);
        builder.HasIndex(x => x.Platform);
        builder.HasIndex(x => x.Action);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.SessionId);
        builder.HasIndex(x => new { x.EntityType, x.EntityId });
        builder.HasIndex(x => new { x.Platform, x.Action, x.CreatedTime });
    }
}
