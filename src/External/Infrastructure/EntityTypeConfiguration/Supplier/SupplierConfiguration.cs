namespace Infrastructure.EntityTypeConfiguration;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();
        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.ContactInfo).HasMaxLength(500);

        builder.HasMany(p => p.ProductSuppliers)
            .WithOne(ps => ps.Supplier)
            .HasForeignKey(ps => ps.SupplierId);
    }
}