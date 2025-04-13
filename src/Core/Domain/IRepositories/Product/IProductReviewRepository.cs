namespace Domain.IRepositories;

public interface IProductReviewRepository : IRepository<ProductReview, long>
{
    Task<IEnumerable<ProductReview>> GetByProductIdAsync(int productId);
    Task<IEnumerable<ProductReview>> SearchByRatingAsync(int minRating, int maxRating);
}