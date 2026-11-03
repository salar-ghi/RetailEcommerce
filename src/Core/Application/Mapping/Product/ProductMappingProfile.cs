namespace Application.Mapping;

public class ProductMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductDto>()
            .Map(d => d.Id, s => (int)s.Id)
            .Map(d => d.Status, s => s.Status.HasValue ? s.Status.Value.ToString().ToLower() : null)
            .Map(d => d.Availability, s => s.Availability.ToString().ToLower())
            .Map(d => d.LegacyStatus, s => s.Availability == ProductAvailability.Discontinued ? "discontinued" : s.IsActive ? "active" : null)
            .Map(d => d.Price, s => ResolveDisplayPrice(s))
            .Map(d => d.PricingStrategy, s => NormalizePricingStrategy(s.PricingStrategy))
            .Map(d => d.StockQuantity, s => s.Stocks.Any() ? s.Stocks.Sum(st => st.Quantity - st.ReservedQuantity) : s.Batches.Sum(b => b.Quantity - b.SoldQuantity))
            .Map(d => d.CategoryName, s => s.Category != null ? s.Category.Name : null)
            .Map(d => d.BrandName, s => s.Brand != null ? s.Brand.Name : null)
            .Map(d => d.SupplierId, s => s.Suppliers.Any() ? s.Suppliers.First().SupplierId : 0)
            .Map(d => d.SupplierName, s => s.Suppliers.Any() && s.Suppliers.First().Supplier != null ? s.Suppliers.First().Supplier.Name : null)
            .Map(d => d.Images, s => s.Images.Select(i => i.ImageUrl).ToList())
            .Map(d => d.CoverImage, s => s.Images.OrderByDescending(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault())
            .Map(d => d.Location, s => s.StorageLocationNote)
            .Map(d => d.ReorderLevel, s => s.Stocks.Any() ? (int?)s.Stocks.First().ReorderThreshold : null)
            .Map(d => d.Stock, s => s.Stocks.FirstOrDefault())
            .Map(d => d.Prices, s => s.Batches)
            .Map(d => d.AttributeValues, s => s.AttributeValues)
            .Map(d => d.Variants, s => s.VariantDefinitions)
            .Map(d => d.ContentBlocks, s => s.ContentBlocks.OrderBy(block => block.SortOrder))
            .Map(d => d.SalesUnit, s => string.IsNullOrWhiteSpace(s.SalesUnitMode) ? null : new SalesUnitConfigDto
            {
                Mode = s.SalesUnitMode!,
                WeightUnit = s.SalesUnitWeightUnit,
                PricePerWeightUnit = s.SalesUnitPricePerWeightUnit,
                PackWeight = s.SalesUnitPackWeight,
                PackLabel = s.SalesUnitPackLabel
            })
            .Map(d => d.Tags, s => s.Tags.Select(pt => pt.Tag != null ? pt.Tag.Name : pt.TagId.ToString()).ToList());

        config.NewConfig<ProductDimensions, DimensionDto>();
        config.NewConfig<ProductStock, StockDto>()
            .Map(d => d.SpaceName, s => s.Space != null ? s.Space.Name : null)
            .Map(d => d.ZoneName, s => s.Zone != null ? s.Zone.Name : null)
            .Map(d => d.ShelfCode, s => s.Shelf != null ? s.Shelf.Code : null)
            .Map(d => d.WarehouseName, s => s.Warehouse != null ? s.Warehouse.Name : null)
            .Map(d => d.Location, s => s.LocationNote)
            .Map(d => d.QuantityUnit, _ => "piece");
        config.NewConfig<ProductInventoryBatch, BatchDto>()
            .Map(d => d.Id, s => (int)s.Id)
            .Map(d => d.Amount, s => s.SellingPrice);
        config.NewConfig<ProductAttribute, AttributeDto>();
        config.NewConfig<ProductAttributeValue, ProductAttributeValueDto>()
            .Map(d => d.AttributeCode, s => s.AttributeDefinition != null ? s.AttributeDefinition.Code : null)
            .Map(d => d.AttributeName, s => s.AttributeDefinition != null ? s.AttributeDefinition.Name : null)
            .Map(d => d.AttributeOptionIds, s => string.IsNullOrWhiteSpace(s.AttributeOptionIds) ? Array.Empty<int>() : System.Text.Json.JsonSerializer.Deserialize<int[]>(s.AttributeOptionIds));
        config.NewConfig<ProductVariantDefinition, VariantDto>();
        config.NewConfig<ProductVariantOption, VariantOptionDto>()
            .Map(d => d.Name, s => s.DisplayValue)
            .Map(d => d.Value, s => s.ActualValue);

        config.NewConfig<ProductAttribute, ProductAttributeDto>().TwoWays();
        config.NewConfig<ProductDimensions, ProductDimensionsDto>().TwoWays();
        config.NewConfig<ProductReview, ProductReviewDto>().TwoWays();
        config.NewConfig<ProductStock, ProductStockDto>().TwoWays();
        config.NewConfig<ProductVariant, ProductVariantDto>().TwoWays();
        config.NewConfig<ProductImage, ProductImageDto>().TwoWays();
        config.NewConfig<ProductContentBlock, ProductContentBlockDto>()
            .Map(d => d.Id, s => s.ClientId ?? s.Id.ToString())
            .Map(d => d.Image, s => s.ImageUrl)
            .TwoWays();
        config.NewConfig<ProductSupplier, ProductSupplierDto>().TwoWays();
        config.NewConfig<ProductTag, ProductTagDto>().TwoWays();
        config.NewConfig<ProductUnitPrice, ProductUnitPriceDto>().TwoWays();
    }

    private static string NormalizePricingStrategy(string? pricingStrategy)
    {
        return pricingStrategy?.Trim().ToLowerInvariant() switch
        {
            "latest" => "latest",
            "average" => "average",
            _ => "fifo"
        };
    }

    private static decimal ResolveDisplayPrice(Product product)
    {
        if (!product.Batches.Any())
            return 0;

        var availableBatches = product.Batches
            .Where(batch => batch.Quantity - batch.SoldQuantity > 0)
            .ToList();
        var batches = availableBatches.Any() ? availableBatches : product.Batches.ToList();

        return NormalizePricingStrategy(product.PricingStrategy) switch
        {
            "latest" => batches
                .OrderByDescending(batch => batch.EffectiveDate)
                .ThenByDescending(batch => batch.Id)
                .First()
                .SellingPrice,
            "average" => ResolveWeightedAveragePrice(batches),
            _ => batches
                .OrderBy(batch => batch.EffectiveDate)
                .ThenBy(batch => batch.Id)
                .First()
                .SellingPrice
        };
    }

    private static decimal ResolveWeightedAveragePrice(IEnumerable<ProductInventoryBatch> batches)
    {
        var weightedBatches = batches
            .Select(batch => new
            {
                batch.SellingPrice,
                Quantity = Math.Max(batch.Quantity - batch.SoldQuantity, 0)
            })
            .Where(batch => batch.Quantity > 0)
            .ToList();

        if (!weightedBatches.Any())
            return batches.OrderByDescending(batch => batch.EffectiveDate).First().SellingPrice;

        var totalQuantity = weightedBatches.Sum(batch => batch.Quantity);
        var totalPrice = weightedBatches.Sum(batch => batch.SellingPrice * batch.Quantity);

        return Math.Round(totalPrice / totalQuantity, 2, MidpointRounding.AwayFromZero);
    }
}
