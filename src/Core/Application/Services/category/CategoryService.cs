namespace Application.Services;
public class CategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IImageHelper _imageHelper;

    public CategoryService(IUnitOfWork unitOfWork, 
        ICurrentUserService currentUserService, 
        IMapper mapper, IImageHelper imageHelper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _imageHelper = imageHelper;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<List<CategoryDetailsDto>> GetAllCategoriesWithDetailsAsync()
    {
        var query = _unitOfWork.Categories.GetAll();
        var categories = await query
            .ProjectTo<CategoryDetailsDto>(_mapper.ConfigurationProvider)
            .OrderBy(z => z.Name)
            .ToListAsync();

        foreach (var dto in categories)
        {
            if (string.IsNullOrEmpty(dto.Image))
                continue;
            dto.Image = await _imageHelper.GetImageBase64(dto.Image);
        }
        
        return categories;
    }

    public async Task<List<CategoryDto>> GetCategoriesWithProductCount()
    {
        List<CategoryDto> categoryDto = new List<CategoryDto>();
        var categories = await _unitOfWork.Categories.GetAllAsync();
        foreach (var category in categories) 
        {
            CategoryDto dto = new CategoryDto();
            var products = await _unitOfWork.Products.GetProductsByCategoryAsync(category.Id);

            dto.Id = category.Id;
            dto.Name = category.Name;
            dto.Description = category.Description;
            dto.ParentId = category.ParentId;
            dto.ProductCount = products.Count();
            dto.CreatedAt = category.CreatedTime;

            categoryDto.Add(dto);
        }
        return categoryDto;
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) throw new KeyNotFoundException($"Category with ID {id} not found.");
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task AddCategoryAsync(CategoryDto categoryDto)
    {
        try
        {
            var category = _mapper.Map<Category>(categoryDto);
            //var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            //category.CreatedBy = userIdClaim?.Value ?? string.Empty;
            category.CreatedBy = "bdfb65f1-9024-4736-846d-df7de909f571";
            category.ModifiedBy = "bdfb65f1-9024-4736-846d-df7de909f571";

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.Clear();
            Console.WriteLine(ex.ToString());
            throw ex;
        }
    }

    public async Task UpdateCategoryAsync(CategoryDto categoryDto)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(categoryDto.Id);
        if (category == null) throw new KeyNotFoundException($"Category with ID {categoryDto.Id} not found.");

        string imagePath = null;
        const string subFolder = "images/categories";
        if (!string.IsNullOrWhiteSpace(categoryDto.Image))
        {
            imagePath = await _imageHelper.SaveBase64Image(categoryDto.Image, subFolder);
        }
        categoryDto.Image = imagePath;
        _mapper.Map(categoryDto, category);
        
        category.ModifiedBy = _currentUserService.UserId;
        category.ModifiedTime = DateTime.Now;

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
}