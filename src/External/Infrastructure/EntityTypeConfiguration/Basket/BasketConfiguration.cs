namespace Infrastructure.EntityTypeConfiguration;

public class BasketConfiguration : IEntityTypeConfiguration<Basket>
{
    public void Configure(EntityTypeBuilder<Basket> builder)
    {
        builder.HasKey(b => b.Id);

        //builder
        //    .Property(b => b.UserId)
        //    .HasMaxLength(50);

        builder.Property(b => b.GuestId)
            .HasMaxLength(50);

        builder.Property(b => b.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.HasOne(b => b.User)
            .WithMany(z => z.Basket)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Items)
            .WithOne(bi => bi.Basket)
            .HasForeignKey(bi => bi.BasketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(b => b.ConcurrencyToken)
            .IsConcurrencyToken();

        builder.Ignore(b => b.TotalPrice);
        builder.Ignore(b => b.TotalItems);

        //builder.HasIndex(b => b.UserId);
        //builder.HasIndex(b => b.GuestId);
    }
}

public class BasketItemConfiguration : IEntityTypeConfiguration<BasketItem>
{
    public void Configure(EntityTypeBuilder<BasketItem> builder)
    {
        builder.HasKey(bi => bi.Id);
        builder.Property(b => b.Id).ValueGeneratedOnAdd();
        builder.Property(bi => bi.Quantity)
            .IsRequired();

        builder.Property(bi => bi.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(bi => bi.DiscountedPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(bi => bi.AppliedPromotionCode)
            .HasMaxLength(50);

        builder.HasOne(bi => bi.Product)
            .WithMany()
            .HasForeignKey(bi => bi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(bi => bi.Basket)
            .WithMany(b => b.Items)
            .HasForeignKey(bi => bi.BasketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(bi => bi.Subtotal);
    }
}