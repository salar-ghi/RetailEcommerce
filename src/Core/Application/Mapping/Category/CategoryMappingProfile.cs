namespace Application.Mapping;

public class CategoryMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Category, CategoryDto>()
            .Map(dest => dest.ParentId, src => src.ParentId)
            .Map(dest => dest.Image, src => src.ImageUrl)
            .TwoWays();

        config.NewConfig<Category, CategoryDetailsDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.CreatedAt, src => src.CreatedTime)
            .Map(dest => dest.ParentId, src => src.ParentId)
            .Map(dest => dest.Image, src => src.ImageUrl)
            .Map(dest => dest.ProductCount, src => src.Products.Count())
            .Map(dest => dest.Brands, src => src.Products.Select(p => p.Brand).Distinct())
            .TwoWays();

        config.NewConfig<CategoryAttribute, CategoryAttributeDto>().TwoWays();
    }
}
