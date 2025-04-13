namespace Application.Services;

public class ProductReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductReviewService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductReviewDto>> GetAllReviewsAsync()
    {
        var reviews = await _unitOfWork.ProductReviews.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductReviewDto>>(reviews);
    }

    public async Task<ProductReviewDto> GetReviewByIdAsync(int id)
    {
        var review = await _unitOfWork.ProductReviews.GetByIdAsync(id);
        return review != null ? _mapper.Map<ProductReviewDto>(review) : throw new KeyNotFoundException($"Review with ID {id} not found.");
    }

    public async Task AddReviewAsync(ProductReviewDto reviewDto)
    {
        var review = _mapper.Map<ProductReview>(reviewDto);
        await _unitOfWork.ProductReviews.AddAsync(review);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateReviewAsync(ProductReviewDto reviewDto)
    {
        var review = await _unitOfWork.ProductReviews.GetByIdAsync(reviewDto.Id);
        if (review != null)
        {
            _mapper.Map(reviewDto, review);
            await _unitOfWork.ProductReviews.UpdateAsync(review);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException($"Review with ID {reviewDto.Id} not found.");
        }
    }

    public async Task DeleteReviewAsync(int id)
    {
        var review = await _unitOfWork.ProductReviews.GetByIdAsync(id);
        if (review != null)
        {
            await _unitOfWork.ProductReviews.DeleteAsync(review);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ProductReviewDto>> SearchReviewsByProductIdAsync(int productId)
    {
        var reviews = await _unitOfWork.ProductReviews.GetByProductIdAsync(productId);
        return _mapper.Map<IEnumerable<ProductReviewDto>>(reviews);
    }

    public async Task<IEnumerable<ProductReviewDto>> SearchReviewsByRatingAsync(int minRating, int maxRating)
    {
        var reviews = await _unitOfWork.ProductReviews.SearchByRatingAsync(minRating, maxRating);
        return _mapper.Map<IEnumerable<ProductReviewDto>>(reviews);
    }
}