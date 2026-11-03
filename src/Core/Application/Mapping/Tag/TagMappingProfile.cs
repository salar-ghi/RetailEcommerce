namespace Application.Mapping;

public class TagMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Tag, TagDto>()
            .Map(dest => dest.CreatedAt, src => src.CreatedTime)
            .Map(dest => dest.ProductCount, src => src.Products.Count(productTag => !productTag.IsDeleted));

        config.NewConfig<CreateTagDto, Tag>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Products)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.CreatedTime)
            .Ignore(dest => dest.ModifiedBy)
            .Ignore(dest => dest.ModifiedTime)
            .Ignore(dest => dest.IsDeleted);

        config.NewConfig<UpdateTagDto, Tag>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Products)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.CreatedTime)
            .Ignore(dest => dest.ModifiedBy)
            .Ignore(dest => dest.ModifiedTime)
            .Ignore(dest => dest.IsDeleted);
    }
}
