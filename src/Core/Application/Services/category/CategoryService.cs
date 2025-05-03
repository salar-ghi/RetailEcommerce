using Microsoft.AspNetCore.Http;

namespace Application.Services;
public class CategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    //private readonly IHttpContextAccessor _httpContextAccessor;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        //_httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
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
}