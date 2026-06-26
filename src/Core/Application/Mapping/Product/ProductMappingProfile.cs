namespace Application.Mapping;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => (int)s.Id))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.HasValue ? s.Status.Value.ToString().ToLower() : null))
            .ForMember(d => d.Availability, o => o.MapFrom(s => s.Availability.ToString().ToLower()))
            .ForMember(d => d.LegacyStatus, o => o.MapFrom(s => s.Availability == ProductAvailability.Discontinued ? "discontinued" : s.IsActive ? "active" : null))
            .ForMember(d => d.Price, o => o.MapFrom(s => ResolveDisplayPrice(s)))
            .ForMember(d => d.PricingStrategy, o => o.MapFrom(s => NormalizePricingStrategy(s.PricingStrategy)))
            .ForMember(d => d.StockQuantity, o => o.MapFrom(s => s.Stocks.Any() ? s.Stocks.Sum(st => st.Quantity - st.ReservedQuantity) : s.Batches.Sum(b => b.Quantity - b.SoldQuantity)))
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category != null ? s.Category.Name : null))
            .ForMember(d => d.BrandName, o => o.MapFrom(s => s.Brand != null ? s.Brand.Name : null))
            .ForMember(d => d.SupplierId, o => o.MapFrom(s => s.Suppliers.Any() ? s.Suppliers.First().SupplierId : 0))
            .ForMember(d => d.SupplierName, o => o.MapFrom(s => s.Suppliers.Any() && s.Suppliers.First().Supplier != null ? s.Suppliers.First().Supplier.Name : null))
            .ForMember(d => d.Images, o => o.MapFrom(s => s.Images.Select(i => i.ImageUrl).ToList()))
            .ForMember(d => d.CoverImage, o => o.MapFrom(s => s.Images.OrderByDescending(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault()))
            .ForMember(d => d.Location, o => o.MapFrom(s => s.StorageLocationNote))
            .ForMember(d => d.ReorderLevel, o => o.MapFrom(s => s.Stocks.Any() ? (int?)s.Stocks.First().ReorderThreshold : null))
            .ForMember(d => d.Stock, o => o.MapFrom(s => s.Stocks.FirstOrDefault()))
            .ForMember(d => d.Prices, o => o.MapFrom(s => s.Batches))
            .ForMember(d => d.Variants, o => o.MapFrom(s => s.VariantDefinitions))
            .ForMember(d => d.SalesUnit, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.SalesUnitMode) ? null : new SalesUnitConfigDto
            {
                Mode = s.SalesUnitMode!,
                WeightUnit = s.SalesUnitWeightUnit,
                PricePerWeightUnit = s.SalesUnitPricePerWeightUnit,
                PackWeight = s.SalesUnitPackWeight,
                PackLabel = s.SalesUnitPackLabel
            }))
            .ForMember(d => d.Tags, o => o.MapFrom(s => s.Tags.Select(pt => pt.Tag != null ? pt.Tag.Name : pt.TagId.ToString()).ToList()));

        CreateMap<ProductDimensions, DimensionDto>();
        CreateMap<ProductStock, StockDto>()
            .ForMember(d => d.SpaceName, o => o.MapFrom(s => s.Space != null ? s.Space.Name : null))
            .ForMember(d => d.ZoneName, o => o.MapFrom(s => s.Zone != null ? s.Zone.Name : null))
            .ForMember(d => d.ShelfCode, o => o.MapFrom(s => s.Shelf != null ? s.Shelf.Code : null))
            .ForMember(d => d.WarehouseName, o => o.MapFrom(s => s.Warehouse != null ? s.Warehouse.Name : null))
            .ForMember(d => d.Location, o => o.MapFrom(s => s.LocationNote))
            .ForMember(d => d.QuantityUnit, o => o.MapFrom(_ => "piece"));
        CreateMap<ProductInventoryBatch, BatchDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => (int)s.Id))
            .ForMember(d => d.Amount, o => o.MapFrom(s => s.SellingPrice));
        CreateMap<ProductAttribute, AttributeDto>();
        CreateMap<ProductVariantDefinition, VariantDto>();
        CreateMap<ProductVariantOption, VariantOptionDto>()
            .ForMember(d => d.Name, o => o.MapFrom(s => s.DisplayValue))
            .ForMember(d => d.Value, o => o.MapFrom(s => s.ActualValue));

        CreateMap<ProductAttribute, ProductAttributeDto>().ReverseMap();
        CreateMap<ProductDimensions, ProductDimensionsDto>().ReverseMap();
        CreateMap<ProductReview, ProductReviewDto>().ReverseMap();
        CreateMap<ProductStock, ProductStockDto>().ReverseMap();
        CreateMap<ProductVariant, ProductVariantDto>().ReverseMap();
        CreateMap<ProductImage, ProductImageDto>().ReverseMap();
        CreateMap<ProductSupplier, ProductSupplierDto>().ReverseMap();
        CreateMap<ProductTag, ProductTagDto>().ReverseMap();
        CreateMap<ProductUnitPrice, ProductUnitPriceDto>().ReverseMap();
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
