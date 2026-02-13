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

        builder.Property(x => x.Size).IsRequired().HasMaxLength(30);

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        //builder.HasMany(x => x.Placement)
        //    .WithOne(x => x.Banner)
        //    .HasForeignKey(x => x.BannerId)
        //    .OnDelete(DeleteBehavior.Cascade);
    }
}

public class BannerPlacementConfiguration : IEntityTypeConfiguration<BannerPlacement>
{
    public void Configure(EntityTypeBuilder<BannerPlacement> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(50);
        builder.Property(x => x.RecommendedSize).IsRequired(false).HasMaxLength(50);
        //builder.HasMany(x => x.Banners)
        //    .WithOne(x => x.Placement)
        //    .HasForeignKey(x => x.PlacementId)
        //    .OnDelete(DeleteBehavior.Cascade);
    }
}