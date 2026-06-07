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
        var cachedPromotions = await _cacheService.GetCachedDataAsync<List<PromotionDto>>(ActivePromotionsKey);
        if (cachedPromotions != null)
        {
            return cachedPromotions;
        }

        var promotions = (await _unitOfWork.Promotions.GetActivePromotionsAsync()).ToList();
        var promotionDtos = _mapper.Map<List<PromotionDto>>(promotions);
        await _cacheService.SetCachedDataAsync(ActivePromotionsKey, promotionDtos, TimeSpan.FromHours(5));
        return promotionDtos;
    }

    public async Task<IEnumerable<PromotionDiscountDto>> GetAllDiscountsAsync()
    {
        var promotions = await _unitOfWork.Promotions.GetPromotionsWithDetailsAsync();
        return promotions.Select(ToDiscountDto).ToList();
    }

    public async Task<PromotionDiscountDto> GetDiscountAsync(int discountId)
    {
        var promotion = await GetPromotionOrThrowAsync(discountId);
        return ToDiscountDto(promotion);
    }

    public async Task<PromotionDiscountDto> CreateDiscountAsync(CreateDiscountRequestDto request)
    {
        ValidateCreateRequest(request);

        var existingPromotion = await _unitOfWork.Promotions.GetPromotionByCodeAsync(request.Code);
        if (existingPromotion != null)
        {
            throw new InvalidOperationException($"Discount code '{request.Code}' already exists.");
        }

        var promotion = new Promotion
        {
            Code = NormalizeCode(request.Code),
            Name = NormalizeCode(request.Code),
            Description = request.Description ?? string.Empty,
            StartDate = NormalizeDate(request.StartDate),
            EndDate = NormalizeDate(request.EndDate),
            IsEnabled = request.IsActive,
            Scope = ParseScope(request.Scope, request.ProductIds, request.CategoryIds),
            Discounts = new List<Discount>
            {
                new()
                {
                    Type = ParseDiscountType(request.DiscountType),
                    Value = request.Amount,
                    MaxUsage = request.MaxUsage,
                    UsedCount = 0
                }
            }
        };

        ApplyTargetMappings(promotion, request.ProductIds, request.CategoryIds);
        SetMinimumOrderCondition(promotion, request.MinimumOrder);

        await _unitOfWork.Promotions.AddAsync(promotion);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveCachedDataAsync(ActivePromotionsKey);

        return ToDiscountDto(promotion);
    }

    public async Task<PromotionDiscountDto> UpdateDiscountAsync(int discountId, UpdateDiscountRequestDto request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var promotion = await GetPromotionOrThrowAsync(discountId, asNoTracking: false);
        var discount = GetPrimaryDiscount(promotion);

        if (!string.IsNullOrWhiteSpace(request.Code) && !NormalizeCode(request.Code).Equals(promotion.Code, StringComparison.OrdinalIgnoreCase))
        {
            var existingPromotion = await _unitOfWork.Promotions.GetPromotionByCodeAsync(request.Code);
            if (existingPromotion != null && existingPromotion.Id != promotion.Id)
            {
                throw new InvalidOperationException($"Discount code '{request.Code}' already exists.");
            }

            promotion.Code = NormalizeCode(request.Code);
            promotion.Name = NormalizeCode(request.Code);
        }

        if (!string.IsNullOrWhiteSpace(request.DiscountType))
        {
            discount.Type = ParseDiscountType(request.DiscountType);
        }

        if (request.Amount.HasValue)
        {
            ValidateAmount(request.Amount.Value, discount.Type);
            discount.Value = request.Amount.Value;
        }

        if (request.StartDate.HasValue)
        {
            promotion.StartDate = NormalizeDate(request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            promotion.EndDate = NormalizeDate(request.EndDate.Value);
        }

        if (promotion.EndDate < promotion.StartDate)
        {
            throw new ArgumentException("End date must be on or after start date.");
        }

        if (request.MaxUsage.HasValue)
        {
            if (request.MaxUsage.Value <= 0) throw new ArgumentException("Max usage must be greater than zero.");
            discount.MaxUsage = request.MaxUsage;
        }

        if (request.Description != null)
        {
            promotion.Description = request.Description;
        }

        if (request.IsActive.HasValue)
        {
            promotion.IsEnabled = request.IsActive.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.Scope) || request.ProductIds != null || request.CategoryIds != null)
        {
            promotion.Scope = ParseScope(request.Scope ?? promotion.Scope.ToString(), request.ProductIds, request.CategoryIds);
            ApplyTargetMappings(promotion, request.ProductIds, request.CategoryIds);
        }

        if (request.MinimumOrder.HasValue)
        {
            SetMinimumOrderCondition(promotion, request.MinimumOrder);
        }

        promotion.ModifiedTime = DateTime.UtcNow;

        await _unitOfWork.Promotions.UpdateAsync(promotion);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveCachedDataAsync(ActivePromotionsKey);

        return ToDiscountDto(promotion);
    }

    public async Task DeleteDiscountAsync(int discountId)
    {
        var promotion = await GetPromotionOrThrowAsync(discountId, asNoTracking: false);
        await _unitOfWork.Promotions.DeleteAsync(promotion);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveCachedDataAsync(ActivePromotionsKey);
    }

    public async Task<DiscountCalculationResultDto> CalculateDiscountAsync(DiscountCalculationRequestDto request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Code)) throw new ArgumentException("Discount code is required.");

        var promotion = await _unitOfWork.Promotions.GetPromotionByCodeAsync(request.Code);
        var subtotal = request.Items?.Any() == true
            ? request.Items.Sum(i => Math.Max(0, i.Quantity) * Math.Max(0m, i.UnitPrice))
            : Math.Max(0, request.OrderTotal);

        if (promotion == null)
        {
            return BuildCalculationResult(request.Code, false, "Discount code was not found.", subtotal, 0, new());
        }

        var validationMessage = GetApplicabilityMessage(promotion, subtotal);
        if (validationMessage != null)
        {
            return BuildCalculationResult(promotion.Code, false, validationMessage, subtotal, 0, new());
        }

        var discount = GetPrimaryDiscount(promotion);
        var eligibleLines = GetEligibleLines(promotion, request.Items ?? new List<DiscountCalculationItemDto>());
        var baseAmount = promotion.Scope is PromotionScope.Product or PromotionScope.Category or PromotionScope.Brand
            ? eligibleLines.Sum(i => Math.Max(0, i.Quantity) * Math.Max(0m, i.UnitPrice))
            : subtotal;

        if (baseAmount <= 0)
        {
            return BuildCalculationResult(promotion.Code, false, "No eligible amount exists for this discount.", subtotal, 0, new());
        }

        var discountAmount = CalculateDiscountAmount(baseAmount, discount);
        var lines = AllocateDiscountToLines(eligibleLines, discountAmount);

        return BuildCalculationResult(
            promotion.Code,
            true,
            "Discount applied successfully.",
            subtotal,
            discountAmount,
            lines);
    }

    public async Task ApplyPromotionsToOrderAsync(Order order)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        var activePromotions = await _unitOfWork.Promotions.GetActivePromotionsAsync();
        var subtotal = GetOrderSubtotal(order);
        var bestDiscount = activePromotions
            .Where(p => p.Scope == PromotionScope.Order || p.Scope == PromotionScope.Sitewide)
            .Where(p => GetApplicabilityMessage(p, subtotal) == null && CheckConditions(order, p.Conditions))
            .Select(p => CalculateDiscountAmount(subtotal, GetPrimaryDiscount(p)))
            .DefaultIfEmpty(0)
            .Max();

        order.DiscountAmount = Math.Min(subtotal, bestDiscount);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ApplyPromotionsToProductAsync(Product product)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));

        var activePromotions = await _unitOfWork.Promotions.GetActivePromotionsAsync();
        var applicablePromotions = activePromotions.Where(p =>
            p.Scope == PromotionScope.Product ||
            p.Scope == PromotionScope.Category ||
            p.Scope == PromotionScope.Brand ||
            p.Scope == PromotionScope.Sitewide);

        foreach (var promotion in applicablePromotions)
        {
            CheckConditions(product, promotion.Conditions);
        }

        await Task.CompletedTask;
    }

    private async Task<Promotion> GetPromotionOrThrowAsync(int discountId, bool asNoTracking = true)
    {
        var promotion = await _unitOfWork.Promotions.GetPromotionWithDetailsAsync(discountId, asNoTracking);
        if (promotion == null)
        {
            throw new KeyNotFoundException($"Discount with ID {discountId} was not found.");
        }

        return promotion;
    }

    private static PromotionDiscountDto ToDiscountDto(Promotion promotion)
    {
        var discount = GetPrimaryDiscount(promotion);
        var minimumOrder = GetMinimumOrder(promotion);
        var status = GetStatus(promotion, discount);

        return new PromotionDiscountDto
        {
            Id = promotion.Id,
            Code = promotion.Code,
            DiscountType = ToClientDiscountType(discount.Type),
            Amount = discount.Value,
            StartDate = promotion.StartDate,
            EndDate = promotion.EndDate,
            MinimumOrder = minimumOrder,
            MaxUsage = discount.MaxUsage,
            UsedCount = discount.UsedCount,
            Description = promotion.Description,
            IsActive = promotion.IsEnabled,
            Status = status,
            Scope = ToClientScope(promotion.Scope),
            CreatedAt = promotion.CreatedTime,
            UpdatedAt = promotion.ModifiedTime
        };
    }

    private static Discount GetPrimaryDiscount(Promotion promotion)
    {
        return promotion.Discounts?.FirstOrDefault() ?? throw new InvalidOperationException("Promotion does not contain a discount definition.");
    }

    private static void ValidateCreateRequest(CreateDiscountRequestDto request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Code)) throw new ArgumentException("Discount code is required.");
        if (request.StartDate == default) throw new ArgumentException("Start date is required.");
        if (request.EndDate == default) throw new ArgumentException("End date is required.");
        if (request.EndDate < request.StartDate) throw new ArgumentException("End date must be on or after start date.");
        if (request.MaxUsage.HasValue && request.MaxUsage.Value <= 0) throw new ArgumentException("Max usage must be greater than zero.");
        if (request.MinimumOrder.HasValue && request.MinimumOrder.Value < 0) throw new ArgumentException("Minimum order cannot be negative.");
        ValidateAmount(request.Amount, ParseDiscountType(request.DiscountType));
    }

    private static void ValidateAmount(decimal amount, DiscountType type)
    {
        if (amount <= 0) throw new ArgumentException("Discount amount must be greater than zero.");
        if (type == DiscountType.Percentage && amount > 100) throw new ArgumentException("Percentage discounts cannot be greater than 100.");
    }

    private static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();

    private static DateTime NormalizeDate(DateTime date) => date.Kind == DateTimeKind.Unspecified
        ? DateTime.SpecifyKind(date, DateTimeKind.Utc)
        : date.ToUniversalTime();

    private static DiscountType ParseDiscountType(string discountType)
    {
        return discountType?.Trim().ToLowerInvariant() switch
        {
            "percentage" or "percent" => DiscountType.Percentage,
            "fixed" or "fixedamount" or "fixed_amount" => DiscountType.FixedAmount,
            _ => throw new ArgumentException("Discount type must be 'percentage' or 'fixed'.")
        };
    }

    private static PromotionScope ParseScope(string scope, List<long> productIds = null, List<int> categoryIds = null)
    {
        if (productIds?.Any() == true) return PromotionScope.Product;
        if (categoryIds?.Any() == true) return PromotionScope.Category;

        return scope?.Trim().ToLowerInvariant() switch
        {
            null or "" or "general" or "sitewide" => PromotionScope.Sitewide,
            "order" or "orders" => PromotionScope.Order,
            "product" or "products" => PromotionScope.Product,
            "category" or "categories" => PromotionScope.Category,
            "brand" or "brands" => PromotionScope.Brand,
            _ => throw new ArgumentException("Scope must be general, order, product, category, or brand.")
        };
    }

    private static string ToClientDiscountType(DiscountType type) => type == DiscountType.Percentage ? "percentage" : "fixed";

    private static string ToClientScope(PromotionScope scope) => scope switch
    {
        PromotionScope.Sitewide => "general",
        PromotionScope.Order => "order",
        PromotionScope.Product => "product",
        PromotionScope.Category => "category",
        PromotionScope.Brand => "brand",
        _ => "general"
    };

    private static string GetStatus(Promotion promotion, Discount discount)
    {
        if (!promotion.IsEnabled) return "inactive";
        if (promotion.EndDate < DateTime.UtcNow) return "expired";
        if (discount.MaxUsage.HasValue && discount.UsedCount >= discount.MaxUsage.Value) return "used";
        return "active";
    }

    private static decimal? GetMinimumOrder(Promotion promotion)
    {
        var condition = promotion.Conditions?.FirstOrDefault(c => c.Type == ConditionType.MinOrderAmount);
        return decimal.TryParse(condition?.Value, out var minimumOrder) ? minimumOrder : null;
    }

    private static void SetMinimumOrderCondition(Promotion promotion, decimal? minimumOrder)
    {
        if (minimumOrder.HasValue && minimumOrder.Value < 0) throw new ArgumentException("Minimum order cannot be negative.");

        var existing = promotion.Conditions.FirstOrDefault(c => c.Type == ConditionType.MinOrderAmount);
        if (!minimumOrder.HasValue)
        {
            if (existing != null) promotion.Conditions.Remove(existing);
            return;
        }

        if (existing == null)
        {
            promotion.Conditions.Add(new PromotionCondition
            {
                Type = ConditionType.MinOrderAmount,
                Value = minimumOrder.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)
            });
            return;
        }

        existing.Value = minimumOrder.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    private static void ApplyTargetMappings(Promotion promotion, List<long> productIds, List<int> categoryIds)
    {
        if (productIds != null)
        {
            promotion.Products.Clear();
            foreach (var productId in productIds.Distinct())
            {
                promotion.Products.Add(new ProductPromotion { ProductId = productId });
            }
        }

        if (categoryIds != null)
        {
            promotion.Categories.Clear();
            foreach (var categoryId in categoryIds.Distinct())
            {
                promotion.Categories.Add(new CategoryPromotion { CategoryId = categoryId });
            }
        }
    }

    private static string GetApplicabilityMessage(Promotion promotion, decimal subtotal)
    {
        var discount = GetPrimaryDiscount(promotion);
        if (!promotion.IsEnabled) return "Discount is inactive.";
        if (promotion.StartDate > DateTime.UtcNow) return "Discount has not started yet.";
        if (promotion.EndDate < DateTime.UtcNow) return "Discount has expired.";
        if (discount.MaxUsage.HasValue && discount.UsedCount >= discount.MaxUsage.Value) return "Discount usage limit has been reached.";

        var minimumOrder = GetMinimumOrder(promotion);
        if (minimumOrder.HasValue && subtotal < minimumOrder.Value) return $"Minimum order amount is {minimumOrder.Value}.";

        return null;
    }

    private static List<DiscountCalculationItemDto> GetEligibleLines(Promotion promotion, List<DiscountCalculationItemDto> items)
    {
        if (promotion.Scope == PromotionScope.Sitewide || promotion.Scope == PromotionScope.Order || !items.Any())
        {
            return items;
        }

        var productIds = promotion.Products.Select(p => p.ProductId).ToHashSet();
        var categoryIds = promotion.Categories.Select(c => c.CategoryId).ToHashSet();

        return promotion.Scope switch
        {
            PromotionScope.Product when productIds.Any() => items.Where(i => productIds.Contains(i.ProductId)).ToList(),
            PromotionScope.Category when categoryIds.Any() => items.Where(i => i.CategoryId.HasValue && categoryIds.Contains(i.CategoryId.Value)).ToList(),
            _ => items
        };
    }

    private static decimal CalculateDiscountAmount(decimal amount, Discount discount)
    {
        var discountAmount = discount.Type == DiscountType.Percentage
            ? amount * (discount.Value / 100)
            : discount.Value;

        return Math.Round(Math.Min(amount, discountAmount), 2, MidpointRounding.AwayFromZero);
    }

    private static List<DiscountCalculationLineDto> AllocateDiscountToLines(List<DiscountCalculationItemDto> eligibleLines, decimal discountAmount)
    {
        if (!eligibleLines.Any() || discountAmount <= 0) return new List<DiscountCalculationLineDto>();

        var subtotal = eligibleLines.Sum(i => Math.Max(0, i.Quantity) * Math.Max(0m, i.UnitPrice));
        if (subtotal <= 0) return new List<DiscountCalculationLineDto>();

        return eligibleLines.Select(item =>
        {
            var lineSubtotal = Math.Max(0, item.Quantity) * Math.Max(0m, item.UnitPrice);
            var lineDiscount = Math.Round(discountAmount * (lineSubtotal / subtotal), 2, MidpointRounding.AwayFromZero);
            return new DiscountCalculationLineDto
            {
                ProductId = item.ProductId,
                LineSubtotal = lineSubtotal,
                DiscountAmount = lineDiscount,
                FinalLineTotal = Math.Max(0, lineSubtotal - lineDiscount)
            };
        }).ToList();
    }

    private static DiscountCalculationResultDto BuildCalculationResult(string code, bool isApplicable, string message, decimal subtotal, decimal discountAmount, List<DiscountCalculationLineDto> lines)
    {
        return new DiscountCalculationResultDto
        {
            Code = code,
            IsApplicable = isApplicable,
            Message = message,
            Subtotal = subtotal,
            DiscountAmount = discountAmount,
            FinalTotal = Math.Max(0, subtotal - discountAmount),
            Lines = lines
        };
    }

    private static bool CheckConditions(object entity, ICollection<PromotionCondition> conditions)
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
                    if (GetOrderSubtotal(order) < minAmount) return false;
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

    private static decimal GetOrderSubtotal(Order order) => order.Items.Sum(item => item.Quantity * item.UnitPrice);
}
