namespace Infrastructure.EntityTypeConfiguration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).IsRequired(false).HasMaxLength(1000);
        builder.HasOne(p => p.Category)
               .WithMany()
               .HasForeignKey(p => p.CategoryId);

        builder.HasOne(p => p.Brand)
               .WithMany()
               .HasForeignKey(p => p.BrandId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Dimensions)
               .WithOne(d => d.Product)
               .HasForeignKey<ProductDimensions>(d => d.ProductId);

        builder.HasMany(p => p.Attributes)
               .WithOne(a => a.Product)
               .HasForeignKey(a => a.ProductId);

        builder.HasMany(p => p.Images)
               .WithOne(i => i.Product)
               .HasForeignKey(i => i.ProductId);

        builder.HasMany(p => p.Reviews)
               .WithOne(r => r.Product)
               .HasForeignKey(r => r.ProductId);

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

        builder.HasMany(p => p.Batches)
               .WithOne(b => b.Product)
               .HasForeignKey(b => b.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Stocks)
               .WithOne(s => s.Product)
               .HasForeignKey(s => s.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.VariantDefinitions)
               .WithOne(v => v.Product)
               .HasForeignKey(v => v.ProductId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ProductInventoryBatchConfiguration : IEntityTypeConfiguration<ProductInventoryBatch>
{
    public void Configure(EntityTypeBuilder<ProductInventoryBatch> builder)
    {
        builder.HasKey(pa => pa.Id);
        builder.Property(pa => pa.Id).ValueGeneratedOnAdd();

        builder.HasOne(pa => pa.Product)
            .WithMany(p => p.Batches)
            .HasForeignKey(pa => pa.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ProductVariantDefinitionConfiguration : IEntityTypeConfiguration<ProductVariantDefinition>
{
    public void Configure(EntityTypeBuilder<ProductVariantDefinition> builder)
    {
        builder.HasKey(pa => pa.Id);
        builder.Property(pa => pa.Id).ValueGeneratedOnAdd();

        builder.Property(pa => pa.Name).IsRequired().HasMaxLength(100);
        builder.Property(pa => pa.Type).IsRequired().HasMaxLength(50);

        builder.HasOne(pa => pa.Product)
            .WithMany(p => p.VariantDefinitions)
            .HasForeignKey(pa => pa.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ProductVariantOptionConfiguration : IEntityTypeConfiguration<ProductVariantOption>
{
    public void Configure(EntityTypeBuilder<ProductVariantOption> builder)
    {
        builder.HasKey(pa => pa.Id);
        builder.Property(pa => pa.Id).ValueGeneratedOnAdd();

        builder.Property(pa => pa.Value).IsRequired().HasMaxLength(200);
        builder.Property(pa => pa.OptionValue).IsRequired().HasMaxLength(200);
        builder.Property(pa => pa.Sku).HasMaxLength(100);
        builder.Property(pa => pa.PriceAdjustment).HasColumnType("decimal(18,2)");

        builder.HasOne(pa => pa.Definition)
            .WithMany(d => d.Options)
            .HasForeignKey(pa => pa.DefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pa => pa.Sku)
            .IsUnique()
            .HasFilter("[Sku] IS NOT NULL");
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
        builder.Property(pd => pd.DimensionUnit).HasMaxLength(20);
        builder.Property(pd => pd.WeightUnit).HasMaxLength(20);

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
        builder.Property(ps => ps.ReservedQuantity).HasDefaultValue(0);
        builder.Property(ps => ps.Sku).HasMaxLength(100);
        builder.Property(ps => ps.LocationNote).HasMaxLength(500);
        builder.Ignore(ps => ps.AvailableQuantity);

        builder.HasOne(ps => ps.Product)
            .WithMany(p => p.Stocks)
            .HasForeignKey(ps => ps.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ps => ps.Space)
            .WithMany(s => s.ProductStocks)
            .HasForeignKey(ps => ps.SpaceId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ps => ps.Zone)
            .WithMany(z => z.ProductStocks)
            .HasForeignKey(ps => ps.ZoneId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ps => ps.Shelf)
            .WithMany(s => s.ProductStocks)
            .HasForeignKey(ps => ps.ShelfId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ps => ps.ProductInventoryBatch)
            .WithMany()
            .HasForeignKey(ps => ps.ProductInventoryBatchId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ps => ps.ProductVariantOption)
            .WithMany()
            .HasForeignKey(ps => ps.ProductVariantOptionId)
            .OnDelete(DeleteBehavior.NoAction);
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
        builder.Property(t => t.Color).HasMaxLength(50);
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
