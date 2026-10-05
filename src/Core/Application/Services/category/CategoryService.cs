namespace Application.Services;

public class CategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IImageHelper _imageHelper;
    private readonly IBannerService? _bannerService;
    private readonly CategoryAttributeService? _categoryAttributeService;

    public CategoryService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService, 
        IMapper mapper,
        IImageHelper imageHelper,
        IBannerService? bannerService = null,
        CategoryAttributeService? categoryAttributeService = null)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageHelper = imageHelper;
        _bannerService = bannerService;
        _categoryAttributeService = categoryAttributeService;
    }

    public async Task<CategoryPageDataDto> GetCategoryPageDataAsync(string categorySearch)
    {
        var result = new CategoryPageDataDto();
        if (string.IsNullOrWhiteSpace(categorySearch))
        {
            return result;
        }

        var searchTrimmed = categorySearch.Trim();

        var allCategories = await _unitOfWork.Categories.GetAll(c => !c.IsDeleted)
            .AsNoTracking()
            .ToListAsync();

        var matchedCategory = allCategories.FirstOrDefault(c =>
            string.Equals(c.Name?.Trim(), searchTrimmed, StringComparison.OrdinalIgnoreCase))
            ?? allCategories.FirstOrDefault(c =>
                c.Name != null && (
                    c.Name.Contains(searchTrimmed, StringComparison.OrdinalIgnoreCase) ||
                    (searchTrimmed.Equals("coffee", StringComparison.OrdinalIgnoreCase) &&
                     (c.Name.Contains("قهوه", StringComparison.OrdinalIgnoreCase) || c.Name.Contains("Coffee", StringComparison.OrdinalIgnoreCase))) ||
                    (searchTrimmed.Contains("قهوه", StringComparison.OrdinalIgnoreCase) &&
                     c.Name.Contains("coffee", StringComparison.OrdinalIgnoreCase))
                ));

        var categoryIds = new HashSet<int>();
        if (matchedCategory != null)
        {
            result.Category = _mapper.Map<CategoryDto>(matchedCategory);

            categoryIds.Add(matchedCategory.Id);
            var queue = new Queue<int>();
            queue.Enqueue(matchedCategory.Id);

            while (queue.Count > 0)
            {
                var parentId = queue.Dequeue();
                foreach (var child in allCategories.Where(c => c.ParentId == parentId))
                {
                    if (categoryIds.Add(child.Id))
                    {
                        queue.Enqueue(child.Id);
                    }
                }
            }
        }
        else if (searchTrimmed.Equals("coffee", StringComparison.OrdinalIgnoreCase) || searchTrimmed.Contains("قهوه"))
        {
            var coffeeCategories = allCategories.Where(c =>
                c.Name != null && (c.Name.Contains("قهوه", StringComparison.OrdinalIgnoreCase) || c.Name.Contains("coffee", StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (var cat in coffeeCategories)
            {
                if (categoryIds.Add(cat.Id))
                {
                    var queue = new Queue<int>();
                    queue.Enqueue(cat.Id);
                    while (queue.Count > 0)
                    {
                        var parentId = queue.Dequeue();
                        foreach (var child in allCategories.Where(c => c.ParentId == parentId))
                        {
                            if (categoryIds.Add(child.Id))
                            {
                                queue.Enqueue(child.Id);
                            }
                        }
                    }
                }
            }
        }

        if (!categoryIds.Any())
        {
            return result;
        }

        var productsQuery = _unitOfWork.Products.GetAll(p => !p.IsDeleted && p.IsActive)
            .Where(p => categoryIds.Contains(p.CategoryId))
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Batches)
            .Include(p => p.Stocks)
            .Include(p => p.Images)
            .Include(p => p.ContentBlocks)
            .OrderByDescending(p => p.Id)
            .AsNoTracking();

        var productsList = await productsQuery.ToListAsync();
        var productDtos = _mapper.Map<List<ProductDto>>(productsList);

        await ConvertProductsImagesToBase64Async(productDtos);
        result.Products = productDtos;

        var categoryAttributeService = _categoryAttributeService ?? new CategoryAttributeService(_unitOfWork, _mapper);
        var defs = await categoryAttributeService.GetCategoryAttributeDefinitionsAsync(categoryIds);
        result.AttributeDefinitions = defs.DistinctBy(a => a.Id).ToList();

        if (_bannerService != null)
        {
            try
            {
                var banners = await _bannerService.GetByPlacementAsync(BannerPageCode.CATEGORY);
                result.Banners = banners.ToList();
            }
            catch
            {
                result.Banners = new List<BannerDto>();
            }
        }

        return result;
    }

    private async Task ConvertProductsImagesToBase64Async(IEnumerable<ProductDto> products)
    {
        var productList = products.ToList();
        await Task.WhenAll(productList.Select(product => ConvertProductImagesToBase64Async(product)));
    }

    private async Task ConvertProductImagesToBase64Async(ProductDto product)
    {
        var imagesTask = ConvertStoredImagesToBase64Async(product.Images);
        var coverImageTask = ConvertStoredImageToBase64Async(product.CoverImage);

        await Task.WhenAll(imagesTask, coverImageTask);

        product.Images = await imagesTask;
        product.CoverImage = await coverImageTask;
    }

    private async Task<string> ConvertStoredImageToBase64Async(string? image)
    {
        if (string.IsNullOrWhiteSpace(image))
            return string.Empty;

        return await _imageHelper.GetImageBase64(image) ?? string.Empty;
    }

    private async Task<List<string>> ConvertStoredImagesToBase64Async(List<string>? images)
    {
        var conversionTasks = (images ?? new List<string>())
            .Where(image => !string.IsNullOrWhiteSpace(image))
            .Select(image => _imageHelper.GetImageBase64(image))
            .ToArray();

        var converted = await Task.WhenAll(conversionTasks);
        return converted.Where(image => !string.IsNullOrWhiteSpace(image)).ToList();
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<List<CategoryDetailsDto>> GetAllCategoriesWithDetailsAsync()
    {
        var data = _unitOfWork.Categories.GetAll(z => z.IsDeleted == false);
        var categories = await data
            .ProjectTo<CategoryDetailsDto>(_mapper.ConfigurationProvider)
            .OrderBy(z => z.Name)
            .ToListAsync();

        var imageMap = await _imageHelper.GetImagesBase64Async(
            categories.Select(dto => dto.Image).Where(img => !string.IsNullOrEmpty(img))!
        );

        foreach (var dto in categories)
        {
            if (string.IsNullOrEmpty(dto.Image))
                continue;

            dto.Image = imageMap.TryGetValue(dto.Image, out var base64) ? base64 : null;
        }
        
        return categories;
    }

    public async Task<List<CategoryDto>> GetCategoriesWithProductCount()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        var productCounts = await _unitOfWork.Products.GetProductCountsGroupedByCategoryAsync();

        return categories.Select(category => new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentId = category.ParentId,
            ProductCount = productCounts.TryGetValue(category.Id, out var count) ? count : 0,
            CreatedAt = category.CreatedTime
        }).ToList();
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) throw new KeyNotFoundException($"Category with ID {id} not found.");
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task AddCategoryAsync(CategoryDto categoryDto)
    {
        categoryDto.Name = NormalizeName(categoryDto.Name, nameof(categoryDto.Name));

        if (await _unitOfWork.Categories.ExistsByNameAsync(categoryDto.Name.ToLower()))
        {
            throw new ArgumentException($"Category with name '{categoryDto.Name}' already exists.");
        }

        if (categoryDto.Image != null)
        {
            const string subFolder = "images/categories";
            if (!string.IsNullOrWhiteSpace(categoryDto.Image))
            {
                categoryDto.Image = await _imageHelper.SaveBase64Image(categoryDto.Image, subFolder, "category");
            }
        }

        var category = _mapper.Map<Category>(categoryDto);
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateCategoryAsync(CategoryDto categoryDto)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(categoryDto.Id);
        if (category == null) throw new KeyNotFoundException($"Category with ID {categoryDto.Id} not found.");

        categoryDto.Name = NormalizeName(categoryDto.Name, nameof(categoryDto.Name));

        if (await _unitOfWork.Categories.ExistsByNameAsync(categoryDto.Name.ToLower(), categoryDto.Id))
        {
            throw new ArgumentException($"Category with name '{categoryDto.Name}' already exists.");
        }

        const string subFolder = "images/categories";
        if (!string.IsNullOrWhiteSpace(categoryDto.Image) &&
            categoryDto.Image.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
        {
            categoryDto.Image = await _imageHelper.SaveBase64ImageIfChanged(
                categoryDto.Image,
                category.ImageUrl,
                subFolder,
                "category");
        }
        else if (string.IsNullOrWhiteSpace(categoryDto.Image))
        {
            categoryDto.Image = category.ImageUrl;
        }

        _mapper.Map(categoryDto, category);        
        await _unitOfWork.Categories.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category != null)
        {
            await _unitOfWork.Categories.DeleteAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<CategoryDto>> SearchCategoriesByNameAsync(string name)
    {
        var categories = await _unitOfWork.Categories.SearchByNameAsync(name);
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<IEnumerable<CategoryDto>> SearchCategoriesByDescriptionAsync(string description)
    {
        var categories = await _unitOfWork.Categories.SearchByDescriptionAsync(description);
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    private static string NormalizeName(string name, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name is required.", parameterName);
        }

        return name.Trim();
    }
}
