namespace Application.Services;

public class PromotionService : IPromotionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRedisCacheService _cacheService;
    private readonly IMapper _mapper;
    private const string ActivePromotionsKey = "ActivePromotions";

    public PromotionService(IUnitOfWork unitOfWork, IMapper mapper, IRedisCacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<PromotionDto> GetPromotionAsync(int promotionId)
    {
        var promotion = await _unitOfWork.Promotions.GetPromotionWithDetailsAsync(promotionId);
        if (promotion == null)
        {
            throw new KeyNotFoundException($"Promotion with ID {promotionId} not found.");
        }
        return _mapper.Map<PromotionDto>(promotion);
    }

    public async Task<IEnumerable<PromotionDto>> GetActivePromotionsAsync()
    {
        var cachedPromotions = await _cacheService.GetCachedDataAsync<Promotion>(ActivePromotionsKey);
        if (cachedPromotions != null)
        {
            return _mapper.Map<IEnumerable<PromotionDto>>(cachedPromotions);
        }

        var promotions = await _unitOfWork.Promotions.GetActivePromotionsAsync();
        await _cacheService.SetCachedDataAsync(ActivePromotionsKey, promotions.ToList(), TimeSpan.FromHours(5));
        return _mapper.Map<IEnumerable<PromotionDto>>(promotions);
    }

    public async Task ApplyPromotionsToOrderAsync(Order order)
    {
        var activePromotions = await _unitOfWork.Promotions.GetActivePromotionsAsync();
        var applicablePromotions = activePromotions.Where(p => p.Scope == PromotionScope.Order || p.Scope == PromotionScope.Sitewide);

        foreach (var promotion in applicablePromotions)
        {
            if (CheckConditions(order, promotion.Conditions))
            {
                ApplyDiscountToOrder(order, promotion.Discounts.FirstOrDefault());
            }
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ApplyPromotionsToProductAsync(Product product)
    {
        var activePromotions = await _unitOfWork.Promotions.GetActivePromotionsAsync();
        var applicablePromotions = activePromotions.Where(p =>
            p.Scope == PromotionScope.Product ||
            p.Scope == PromotionScope.Category ||
            p.Scope == PromotionScope.Brand);


        foreach (var promotion in applicablePromotions)
        {
            if (CheckConditions(product, promotion.Conditions))
            {
                ApplyDiscountToProduct(product, promotion.Discounts.FirstOrDefault());
            }
        }
        await _unitOfWork.SaveChangesAsync();
    }

    //public async Task ApplyPromotionsToCategoryAsync(Category category)
    //{
    //    var activePromotions = await _unitOfWork.Promotions.GetActivePromotionsAsync();
    //    var applicablePromotions = activePromotions.Where(p => p.Scope == PromotionScope.Category);

    //    foreach (var promotion in applicablePromotions)
    //    {
    //        if (CheckConditions(category, promotion.Conditions))
    //        {
    //            ApplyDiscountTocateg(category, promotion.Discounts.FirstOrDefault());
    //        }
    //    }
    //    await _unitOfWork.SaveChangesAsync();
    //}

    private async Task<List<Promotion>> GetActivePromotionsFromCacheOrDbAsync()
    {
        var cachedPromotions = await _cacheService.GetCachedDataAsync<List<Promotion>>(ActivePromotionsKey);
        if (cachedPromotions != null)
        {
            return cachedPromotions;
        }

        var promotions = await _unitOfWork.Promotions.GetActivePromotionsAsync();

        await _cacheService.SetCachedDataAsync<List<Promotion>>(ActivePromotionsKey, promotions.ToList(), TimeSpan.FromHours(5));
        return promotions.ToList();
    }

    private bool CheckConditions(object entity, ICollection<PromotionCondition> conditions)
    {
        foreach (var condition in conditions)
        {
            if (condition.Type == ConditionType.DateRange)
            {
                var dateRange = condition.Value.Split(" to ");
                DateTime start = DateTime.Parse(dateRange[0]);
                DateTime end = DateTime.Parse(dateRange[1]);
                if (DateTime.UtcNow < start || DateTime.UtcNow > end) return false;
            }
            if (entity is Order order)
            {
                if (condition.Type == ConditionType.MinOrderAmount)
                {
                    decimal minAmount = decimal.Parse(condition.Value);
                    if (order.TotalAmount < minAmount) return false;
                }
            }
            else if (entity is Product product)
            {
                if (condition.Type == ConditionType.ProductId)
                {
                    int productId = int.Parse(condition.Value);
                    if (product.Id != productId) return false;
                }
                else if (condition.Type == ConditionType.CategoryId)
                {
                    int categoryId = int.Parse(condition.Value);
                    if (product.CategoryId != categoryId) return false;
                }
                else if (condition.Type == ConditionType.BrandId)
                {
                    int brandId = int.Parse(condition.Value);
                    if (product.BrandId != brandId) return false;
                }
            }
        }
        return true;
    }

    private void ApplyDiscountToOrder(Order order, Discount discount)
    {
        if (discount == null) return;
        decimal discountAmount = discount.Type == DiscountType.Percentage
            ? order.Items.Sum(item => item.Quantity * item.UnitPrice) * (discount.Value / 100)
            : discount.Value;
        order.DiscountAmount += discountAmount;
    }

    private void ApplyDiscountToProduct(Product product, Discount discount)
    {
        if (discount == null) return;
        decimal discountAmount = discount.Type == DiscountType.Percentage
            ? product.Price * (discount.Value / 100)
            : discount.Value;
        product.Price -= discountAmount;
    }

    //private void ApplyDiscountToCatagory(Category category, Discount discount)
    //{
    //    if (discount == null) return;
    //    decimal discountAmount = discount.Type == DiscountType.Percentage
    //        ? product.Price * (discount.Value / 100)
    //        : discount.Value;
    //    product.Price -= discountAmount;
    //}
}