namespace Infrastructure.EntityTypeConfiguration;

public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedOnAdd();
        builder.Property(b => b.Name).IsRequired().HasMaxLength(100);
        builder.Property(b => b.ImageUrl).HasMaxLength(500);
        builder.Property(b => b.Description).IsRequired(false).HasMaxLength(1000);
    }
}

public class BrandCategoryConfiguration : IEntityTypeConfiguration<BrandCategory>
{
    public void Configure(EntityTypeBuilder<BrandCategory> builder)
    {
        builder.HasKey(b => b.Id);

        builder.HasOne(z => z.Brand)
            .WithMany(z => z.BrandCategories)
            .HasForeignKey(z => z.BrandId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(z => z.Category)
            .WithMany(z => z.BrandCategories)
            .HasForeignKey(z => z.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(bc => new { bc.BrandId, bc.CategoryId }).IsUnique();
    }
}