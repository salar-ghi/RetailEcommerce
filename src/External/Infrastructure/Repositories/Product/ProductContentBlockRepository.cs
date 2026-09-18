namespace Infrastructure.Repositories;

public class ProductContentBlockRepository : Repository<ProductContentBlock, long>, IProductContentBlockRepository
{
    public ProductContentBlockRepository(AppDbContext context) : base(context)
    {
    }
}
