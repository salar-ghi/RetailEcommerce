namespace Infrastructure.EntityTypeConfiguration;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.Property(o => o.RowVersion).IsRowVersion();

        builder.Property(o => o.CustomerId).IsRequired();
        builder.Property(o => o.Status).IsRequired();
        builder.Property(o => o.DiscountAmount).HasPrecision(18, 2).HasDefaultValue(0m);
        //builder.Property(o => o.ShippingAddress).IsRequired().HasMaxLength(500);
        //builder.Property(o => o.PaymentMethod).IsRequired();
        builder.Property(o => o.RowVersion).IsConcurrencyToken();

        builder.Ignore(o => o.TotalAmount);
        builder.Ignore(o => o.TotalItems);

        builder.OwnsOne(o => o.ShippingAddress);

        // Relationships
        builder.HasOne(o => o.Customer)
               .WithMany()
               .HasForeignKey(o => o.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Items)
               .WithOne(oi => oi.Order)
               .HasForeignKey(oi => oi.OrderId);

        builder.HasMany(o => o.Payments)
            .WithOne(p => p.Order)
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.CustomerId);
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        // Primary key (inherited from BaseModel<int>)
        builder.HasKey(oi => oi.Id);

        // Properties
        builder.Property(oi => oi.ProductId).IsRequired();
        builder.Property(oi => oi.Quantity).IsRequired();
        builder.Property(oi => oi.UnitPrice).HasPrecision(18, 2).IsRequired();
        builder.Property(oi => oi.OrderId).IsRequired();

        builder.Property(oi => oi.DiscountedPrice)
            .HasColumnType("decimal(18,2)");

        // Relationships
        builder.HasOne(oi => oi.Product)
               .WithMany()
               .HasForeignKey(oi => oi.ProductId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(oi => oi.Order)
               .WithMany(o => o.Items)
               .HasForeignKey(oi => oi.OrderId);


        // Indexes
        builder.HasIndex(oi => oi.OrderId);
        builder.HasIndex(oi => oi.ProductId);
    }
}