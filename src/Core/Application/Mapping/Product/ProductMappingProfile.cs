namespace Application.Mapping;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Status, 
                opt => opt.MapFrom(src => src.Status.HasValue ? src.Status.Value.ToString().ToLower() : null))
            .ForMember(dest => dest.Availability,
                opt => opt.MapFrom(src => src.Availability.ToString().ToLower()))  // not nullable
            .ForMember(dest => dest.Price,
                opt => opt.MapFrom(src =>
                    src.Batches.Any() ? (decimal?)src.Batches.First().SellingPrice : 0))
            .ForMember(dest => dest.StockQuantity,
                opt => opt.MapFrom(src => src.Batches.Sum(b => b.Quantity - b.SoldQuantity)))
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.BrandName,
                opt => opt.MapFrom(src => src.Brand.Name))
            .ForMember(dest => dest.SupplierName,
                opt => opt.MapFrom(src =>
                    src.Suppliers.Any() ? src.Suppliers.First().Supplier.Name : null))
            .ForMember(dest => dest.Images,
                opt => opt.MapFrom(src => src.Images.Select(i => i.ImageUrl).ToList()))

            .ForMember(dest => dest.CoverImage, 
                opt => opt.MapFrom(src => src.Images.OrderByDescending(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault()))

            .ForMember(d => d.SalesUnit, opt => opt.MapFrom(s => new SalesUnitConfigDto
            {
                Mode = s.SalesUnitMode,
                WeightUnit = s.SalesUnitWeightUnit,
                PricePerWeightUnit = s.SalesUnitPricePerWeightUnit,
                PackWeight = s.SalesUnitPackWeight,
                PackLabel = s.SalesUnitPackLabel
            }))
            .ForMember(d => d.PricingStrategy, opt => opt.MapFrom(s => s.PricingStrategy))
            .ForMember(dest => dest.Tags,
                opt => opt.MapFrom(src => src.Tags.Select(pt => pt.Tag.Name).ToList()));

        CreateMap<CreateProductRequest, Product>()
            .ForMember(d => d.SalesUnitMode, opt => opt.MapFrom(s => s.SalesUnit.Mode))
            .ForMember(d => d.SalesUnitWeightUnit, opt => opt.MapFrom(s => s.SalesUnit.WeightUnit))
            // ... map other fields ...
            .ForMember(d => d.PricingStrategy, opt => opt.MapFrom(s => s.PricingStrategy ?? "fifo"));

        CreateMap<ProductDimensions, DimensionDto>();

        CreateMap<ProductInventoryBatch, BatchDto>()
            .ForMember(dest => dest.Amount,
                opt => opt.MapFrom(src => src.SellingPrice))
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => (int?)src.Id)); // assuming id is long, convert

        CreateMap<ProductAttribute, AttributeDto>();

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
}
