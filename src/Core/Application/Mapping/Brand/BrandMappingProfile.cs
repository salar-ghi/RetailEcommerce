namespace Application.Mapping;

public class BrandMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Brand, BrandDto>()
            .Map(dest => dest.Logo, src => src.ImageUrl)
            .Map(dest => dest.CreatedTime, src => src.CreatedTime)
            .Map(dest => dest.CategoryIds, src => src.BrandCategories.Select(bc => bc.CategoryId))
            .Map(dest => dest.Categories, src => src.BrandCategories
                .Select(bc => new BrandCategories { Id = bc.Category.Id, Name = bc.Category.Name }));

        config.NewConfig<BrandDto, Brand>()
            .Map(dest => dest.ImageUrl, src => src.Logo)
            .Ignore(dest => dest.BrandCategories)
            .Ignore(dest => dest.Products);

        config.NewConfig<Brand, BrandUpdateDto>()
            .Map(dest => dest.Logo, src => src.ImageUrl)
            .Map(dest => dest.CategoryIds, src => src.BrandCategories.Select(bc => bc.CategoryId))
            .Map(dest => dest.Categories, src => src.BrandCategories
                .Select(bc => new BrandCategories { Id = bc.Category.Id, Name = bc.Category.Name }));

        config.NewConfig<BrandUpdateDto, Brand>()
            .Map(dest => dest.ImageUrl, src => src.Logo)
            .Ignore(dest => dest.BrandCategories)
            .Ignore(dest => dest.Products)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.CreatedTime)
            .Ignore(dest => dest.ModifiedBy)
            .Ignore(dest => dest.ModifiedTime)
            .Ignore(dest => dest.IsDeleted);
    }
}
