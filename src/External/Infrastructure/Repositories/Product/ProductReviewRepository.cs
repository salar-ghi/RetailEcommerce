namespace Infrastructure.Repositories;

public class ProductReviewRepository : Repository<ProductReview, long>, IProductReviewRepository
{
    public ProductReviewRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<ProductReview>> GetByProductIdAsync(int productId)
    {
        return await _context.ProductReviews
            .Where(pr => pr.ProductId == productId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductReview>> SearchByRatingAsync(int minRating, int maxRating)
    {
        return await _context.ProductReviews
            .Where(pr => pr.Rating >= minRating && pr.Rating <= maxRating)
            .AsNoTracking()
            .ToListAsync();
    }
}
