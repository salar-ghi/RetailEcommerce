namespace Infrastructure.EntityTypeConfiguration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).IsRequired(false).HasMaxLength(1000);
        builder.Property(p => p.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.HasOne(p => p.Category)
               .WithMany()
               .HasForeignKey(p => p.CategoryId);

        builder.HasOne(p => p.Brand)
               .WithMany()
               .HasForeignKey(p => p.BrandId);

        builder.HasOne(p => p.Dimensions)
               .WithOne(d => d.Product)
               .HasForeignKey<ProductDimensions>(d => d.ProductId);

        builder.HasOne(p => p.Stock)
                .WithOne(s => s.Product)
                .HasForeignKey<ProductStock>(s => s.ProductId);

        builder.HasMany(p => p.Attributes)
               .WithOne(a => a.Product)
               .HasForeignKey(a => a.ProductId);

        builder.HasMany(p => p.Images)
               .WithOne(i => i.Product)
               .HasForeignKey(i => i.ProductId);

        builder.HasMany(p => p.Reviews)
               .WithOne(r => r.Product)
               .HasForeignKey(r => r.ProductId);

        builder.HasMany(p => p.UnitPrices)
               .WithOne(up => up.Product)
               .HasForeignKey(up => up.ProductId);

        builder.HasMany(p => p.Variants)
               .WithOne(v => v.Product)
               .HasForeignKey(v => v.ProductId);


        builder.HasMany(p => p.Suppliers)
               .WithOne(ps => ps.Product)
               .HasForeignKey(ps => ps.ProductId);

        builder.HasMany(p => p.Tags)
               .WithOne(pt => pt.Product)
               .HasForeignKey(pt => pt.ProductId);

        builder.HasMany(p => p.BasketItems)
            .WithOne(bi => bi.Product)
            .HasForeignKey(bi => bi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}

public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.HasKey(pa => pa.Id);
        builder.Property(pa => pa.Id).ValueGeneratedOnAdd();
        builder.Property(pa => pa.Key).IsRequired().HasMaxLength(50);
        builder.Property(pa => pa.Value).IsRequired().HasMaxLength(200);
    }
}

public class ProductDimensionsConfiguration : IEntityTypeConfiguration<ProductDimensions>
{
    public void Configure(EntityTypeBuilder<ProductDimensions> builder)
    {
        builder.HasKey(pd => pd.Id);
        builder.Property(pd => pd.Id).ValueGeneratedOnAdd();
        builder.Property(pd => pd.Unit).HasMaxLength(20);

        builder.Property(p => p.Length).HasColumnType("decimal(18,2)");

        builder.Property(p => p.Height).HasColumnType("decimal(18,2)");

        builder.Property(p => p.Width).HasColumnType("decimal(18,2)");

        builder.Property(p => p.Weight).HasColumnType("decimal(18,2)");
    }
}

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.HasKey(pi => pi.Id);
        builder.Property(pi => pi.Id).ValueGeneratedOnAdd();
        builder.Property(pi => pi.ImageUrl).IsRequired().HasMaxLength(500);
        builder.Property(pi => pi.Description).IsRequired(false).HasMaxLength(5000);
    }
}

public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder.HasKey(pr => pr.Id);
        builder.Property(pr => pr.Id).ValueGeneratedOnAdd();
        builder.Property(pr => pr.Comment).HasMaxLength(1000);
        builder.Property(pr => pr.Rating).IsRequired();
    }
}

public class ProductStockConfiguration : IEntityTypeConfiguration<ProductStock>
{
    public void Configure(EntityTypeBuilder<ProductStock> builder)
    {
        builder.HasKey(ps => ps.Id);
        builder.Property(ps => ps.Id).ValueGeneratedOnAdd();
        builder.Property(ps => ps.Quantity).IsRequired();
    }
}

public class ProductSupplierConfiguration : IEntityTypeConfiguration<ProductSupplier>
{
    public void Configure(EntityTypeBuilder<ProductSupplier> builder)
    {
        builder.HasKey(ps => ps.Id);
        builder.Property(ps => ps.Id).ValueGeneratedOnAdd();
        builder.HasOne(ps => ps.Product)
               .WithMany(p => p.Suppliers)
               .HasForeignKey(ps => ps.ProductId);

        builder.HasOne(ps => ps.Supplier)
               .WithMany(s => s.ProductSuppliers)
               .HasForeignKey(ps => ps.SupplierId);
    }
}

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();
        builder.Property(t => t.Name).IsRequired().HasMaxLength(50);
    }
}

public class ProductTagConfiguration : IEntityTypeConfiguration<ProductTag>
{
    public void Configure(EntityTypeBuilder<ProductTag> builder)
    {
        builder.HasKey(pt => pt.Id);
        builder.Property(pt => pt.Id).ValueGeneratedOnAdd();
        builder.HasOne(pt => pt.Product)
               .WithMany(p => p.Tags)
               .HasForeignKey(pt => pt.ProductId);
        builder.HasOne(pt => pt.Tag)
               .WithMany(t => t.Products)
               .HasForeignKey(pt => pt.TagId);
    }
}

public class ProductUnitPriceConfiguration : IEntityTypeConfiguration<ProductUnitPrice>
{
    public void Configure(EntityTypeBuilder<ProductUnitPrice> builder)
    {
        builder.HasKey(pup => pup.Id);
        builder.Property(pup => pup.Id).ValueGeneratedOnAdd();
        builder.Property(pup => pup.EffectiveDate).IsRequired();
    }
}

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.HasKey(pv => pv.Id);
        builder.Property(pv => pv.Id).ValueGeneratedOnAdd();
        builder.Property(pv => pv.VariantName).IsRequired().HasMaxLength(50);
        builder.Property(pv => pv.VariantValue).IsRequired().HasMaxLength(50);
    }
}