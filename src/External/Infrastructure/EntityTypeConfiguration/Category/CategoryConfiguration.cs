namespace Infrastructure.EntityTypeConfiguration;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.ImageUrl).HasMaxLength(500);
        builder.HasMany(c => c.Attributes)
               .WithOne(a => a.Category)
               .HasForeignKey(a => a.CategoryId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class CategoryAttributeConfiguration : IEntityTypeConfiguration<Domain.Entities.CategoryAttribute>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.CategoryAttribute> builder)
    {
        builder.HasKey(ca => ca.Id);
        builder.Property(ca => ca.Id).ValueGeneratedOnAdd();
        builder.Property(ca => ca.Key).IsRequired().HasMaxLength(50);
        builder.Property(ca => ca.Value).IsRequired().HasMaxLength(200);
    }
}