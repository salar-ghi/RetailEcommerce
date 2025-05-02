namespace Infrastructure.EntityTypeConfiguration;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.Property(p => p.OrderId).IsRequired();
        builder.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
        //builder.Property(p => p.PaymentMethod).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Status).IsRequired();
        builder.Property(p => p.TransactionId).HasMaxLength(100);
        builder.Property(p => p.RowVersion).IsRowVersion();

        builder.HasOne(p => p.Order)
               .WithMany(o => o.Payments)
               .HasForeignKey(p => p.OrderId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}