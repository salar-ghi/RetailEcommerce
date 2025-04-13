namespace Domain.IRepositories;

public interface IProductDimensionsRepository : IRepository<ProductDimensions, int> 
{
    Task<ProductDimensions> GetByProductIdAsync(int productId);
}
