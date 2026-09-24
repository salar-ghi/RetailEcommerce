namespace Application.Services;
public class CategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IImageHelper _imageHelper;

    public CategoryService(IUnitOfWork unitOfWork, 
        ICurrentUserService currentUserService, 
        IMapper mapper, IImageHelper imageHelper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageHelper = imageHelper;
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
