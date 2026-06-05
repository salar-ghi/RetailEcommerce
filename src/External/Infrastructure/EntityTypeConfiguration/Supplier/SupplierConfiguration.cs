namespace Infrastructure.EntityTypeConfiguration;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();
        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Email).IsRequired(false).HasMaxLength(256);
        builder.Property(s => s.Info).IsRequired().HasMaxLength(500);
        builder.Property(s => s.Phone).IsRequired(false).HasMaxLength(50);
        builder.Property(s => s.Address).IsRequired(false).HasMaxLength(1000);
        builder.Property(s => s.Website).IsRequired(false).HasMaxLength(500);
        builder.Property(s => s.Description).IsRequired(false).HasMaxLength(2000);

        builder.HasMany(p => p.ProductSuppliers)
            .WithOne(ps => ps.Supplier)
            .HasForeignKey(ps => ps.SupplierId);
    }
}