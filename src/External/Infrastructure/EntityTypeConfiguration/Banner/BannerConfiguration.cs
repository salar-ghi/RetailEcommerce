namespace Infrastructure.EntityTypeConfiguration;

public class BannerConfiguration : IEntityTypeConfiguration<Banner>
{
    public void Configure(EntityTypeBuilder<Banner> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Description).IsRequired(false).HasMaxLength(2000);
        builder.Property(x => x.ImageUrl).IsRequired(false).HasMaxLength(500);
        builder.Property(x => x.Link).IsRequired(false).HasMaxLength(1000);
        builder.Property(x => x.AltText).IsRequired(false).HasMaxLength(1000);
        builder.Property(x => x.CallToActionText).IsRequired(false).HasMaxLength(1000);

        builder.Property(x => x.Size)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        // Many-to-many
        //builder.HasMany(x => x.Placements)
        //    .WithMany(x => x.Banners)
        //    .UsingEntity<Dictionary<string, object>>(
        //        "BannerPlacementMap",
        //        j => j.HasOne<BannerPlacement>().WithMany().HasForeignKey("PlacementId"),
        //        j => j.HasOne<Banner>().WithMany().HasForeignKey("BannerId"));
    }
}

public class BannerPlacementConfiguration : IEntityTypeConfiguration<BannerPlacement>
{
    public void Configure(EntityTypeBuilder<BannerPlacement> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Code)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(x => x.RecommendedSize).IsRequired(false).HasMaxLength(50);
    }
}

public class BannerPlacementMapConfiguration
    : IEntityTypeConfiguration<BannerPlacementMap>
{
    public void Configure(EntityTypeBuilder<BannerPlacementMap> builder)
    {
        builder.ToTable("BannerPlacementMap");

        builder.HasKey(x => new { x.BannerId, x.PlacementId });

        builder.Property(x => x.CreatedTime)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(100);

        builder.HasOne(x => x.Banner)
            .WithMany(x => x.BannerPlacementMaps)
            .HasForeignKey(x => x.BannerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Placement)
            .WithMany(x => x.BannerPlacementMaps)
            .HasForeignKey(x => x.PlacementId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}