namespace Infrastructure.EntityTypeConfiguration;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();
        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Phone).IsRequired().HasMaxLength(50);

        builder.HasMany(p => p.ProductSuppliers)
            .WithOne(ps => ps.Supplier)
            .HasForeignKey(ps => ps.SupplierId);
    }
}