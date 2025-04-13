namespace Infrastructure.EntityTypeConfiguration;

public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        // Primary Key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("PromotionCode"); // Custom column name for clarity

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.StartDate)
            .IsRequired()
            .HasColumnType("datetime");

        builder.Property(p => p.EndDate)
            .IsRequired()
            .HasColumnType("datetime");

        builder.Property(p => p.Scope)
            .IsRequired()
            .HasConversion<string>(); // Store enum as string in DB

        // Ignore computed property
        builder.Ignore(p => p.IsActive);

        //// BaseModel properties
        //builder.Property(p => p.CreatedDate)
        //    .IsRequired()
        //    .HasColumnType("datetime");

        //builder.Property(p => p.UpdatedDate)
        //    .HasColumnType("datetime");

        // Relationships
        builder.HasMany(p => p.Conditions)
            .WithOne(pc => pc.Promotion)
            .HasForeignKey(pc => pc.PromotionId)
            .OnDelete(DeleteBehavior.Cascade); // Delete conditions when promotion is deleted

        builder.HasMany(p => p.Discounts)
            .WithOne(d => d.Promotion)
            .HasForeignKey(d => d.PromotionId)
            .OnDelete(DeleteBehavior.Cascade); // Delete discounts when promotion is deleted

        builder.HasMany(p => p.Products)
            .WithOne(pp => pp.Promotion)
            .HasForeignKey(pp => pp.PromotionId)
            .OnDelete(DeleteBehavior.Cascade); // Delete product mappings when promotion is deleted

        builder.HasMany(p => p.Categories)
            .WithOne(cp => cp.Promotion)
            .HasForeignKey(cp => cp.PromotionId)
            .OnDelete(DeleteBehavior.Cascade); // Delete category mappings when promotion is deleted

        // Table name
        builder.ToTable("Promotions");
    }
}

public class PromotionConditionConfiguration : IEntityTypeConfiguration<PromotionCondition>
{
    public void Configure(EntityTypeBuilder<PromotionCondition> builder)
    {
        builder.HasKey(pc => pc.Id);

        builder.Property(pc => pc.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(pc => pc.Value)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pc => pc.PromotionId)
            .IsRequired();

        builder.HasOne(pc => pc.Promotion)
            .WithMany(p => p.Conditions)
            .HasForeignKey(pc => pc.PromotionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Table name
        builder.ToTable("PromotionConditions");
    }
}


public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        // Primary Key
        builder.HasKey(d => d.Id);

        // Properties
        builder.Property(d => d.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(d => d.Value)
            .IsRequired()
            .HasColumnType("decimal(18,2)"); 

        builder.Property(d => d.PromotionId)
            .IsRequired();

        // Relationships
        builder.HasOne(d => d.Promotion)
            .WithMany(p => p.Discounts)
            .HasForeignKey(d => d.PromotionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Table name
        builder.ToTable("Discounts");
    }
}

public class ProductPromotionConfiguration : IEntityTypeConfiguration<ProductPromotion>
{
    public void Configure(EntityTypeBuilder<ProductPromotion> builder)
    {
        // Composite Primary Key
        builder.HasKey(pp => new { pp.ProductId, pp.PromotionId });

        // Properties
        builder.Property(pp => pp.ProductId)
            .IsRequired();

        builder.Property(pp => pp.PromotionId)
            .IsRequired();

        // Relationships
        builder.HasOne(pp => pp.Product)
            .WithMany() 
            .HasForeignKey(pp => pp.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pp => pp.Promotion)
            .WithMany(p => p.Products)
            .HasForeignKey(pp => pp.PromotionId)
            .OnDelete(DeleteBehavior.Cascade); 

        // Table name
        builder.ToTable("ProductPromotions");
    }
}

public class OrderPromotionConfiguration : IEntityTypeConfiguration<OrderPromotion>
{
    public void Configure(EntityTypeBuilder<OrderPromotion> builder)
    {
        // Composite Primary Key
        builder.HasKey(pp => new { pp.OrderId, pp.PromotionId });

        // Properties
        builder.Property(pp => pp.OrderId)
            .IsRequired();

        builder.Property(pp => pp.PromotionId)
            .IsRequired();

        // Relationships
        builder.HasOne(pp => pp.Order)
            .WithMany()
            .HasForeignKey(pp => pp.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pp => pp.Promotion)
            .WithMany(p => p.Orders)
            .HasForeignKey(pp => pp.PromotionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Table name
        builder.ToTable("OrderPromotions");
    }
}


public class CategoryPromotionConfiguration : IEntityTypeConfiguration<CategoryPromotion>
{
    public void Configure(EntityTypeBuilder<CategoryPromotion> builder)
    {
        // Composite Primary Key
        builder.HasKey(pp => new { pp.CategoryId, pp.PromotionId });

        // Properties
        builder.Property(pp => pp.CategoryId)
            .IsRequired();

        builder.Property(pp => pp.PromotionId)
            .IsRequired();

        // Relationships
        builder.HasOne(pp => pp.Category)
            .WithMany()
            .HasForeignKey(pp => pp.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pp => pp.Promotion)
            .WithMany(p => p.Categories)
            .HasForeignKey(pp => pp.PromotionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Table name
        builder.ToTable("CategoryPromotions");
    }
}