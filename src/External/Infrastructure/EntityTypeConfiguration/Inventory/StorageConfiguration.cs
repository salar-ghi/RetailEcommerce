namespace Infrastructure.EntityTypeConfiguration;

public class StorageSpaceConfiguration : IEntityTypeConfiguration<StorageSpace>
{
    public void Configure(EntityTypeBuilder<StorageSpace> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Code).HasMaxLength(50);
        builder.Property(s => s.Address).HasMaxLength(500);
        builder.Property(s => s.Description).HasMaxLength(1000);
        builder.Property(s => s.Type).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(s => s.Code).IsUnique().HasFilter("[Code] IS NOT NULL");
    }
}

public class StorageZoneConfiguration : IEntityTypeConfiguration<StorageZone>
{
    public void Configure(EntityTypeBuilder<StorageZone> builder)
    {
        builder.HasKey(z => z.Id);
        builder.Property(z => z.Id).ValueGeneratedOnAdd();
        builder.Property(z => z.Name).IsRequired().HasMaxLength(200);
        builder.Property(z => z.Code).HasMaxLength(50);
        builder.Property(z => z.Description).HasMaxLength(1000);
        builder.HasOne(z => z.Space)
            .WithMany(s => s.Zones)
            .HasForeignKey(z => z.SpaceId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

public class ShelfConfiguration : IEntityTypeConfiguration<Shelf>
{
    public void Configure(EntityTypeBuilder<Shelf> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();
        builder.Property(s => s.Code).IsRequired().HasMaxLength(80);
        builder.Property(s => s.Name).HasMaxLength(200);
        builder.HasIndex(s => s.Code).IsUnique();
        builder.HasOne(s => s.Space)
            .WithMany(sp => sp.Shelves)
            .HasForeignKey(s => s.SpaceId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(s => s.Zone)
            .WithMany(z => z.Shelves)
            .HasForeignKey(s => s.ZoneId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
