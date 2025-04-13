namespace Application.Mapping;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name)).ReverseMap();
        //.ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Suppliers.Select(z => z.Product) .Name));


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
