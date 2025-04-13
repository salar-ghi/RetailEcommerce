namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;
    private readonly CategoryAttributeService _categoryAttributeService;
    public CategoryController(CategoryService categoryService,
        CategoryAttributeService categoryAttributeService)
    {
        _categoryService = categoryService;
        _categoryAttributeService = categoryAttributeService;
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("categories/{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        return Ok(category);
    }

    [HttpPost("categories")]
    public async Task<IActionResult> AddCategory(CategoryDto categoryDto)
    {
        await _categoryService.AddCategoryAsync(categoryDto);
        return CreatedAtAction(nameof(GetCategoryById), new { id = categoryDto.Id }, categoryDto);
    }

    [HttpPut("categories/{id}")]
    public async Task<IActionResult> UpdateCategory(int id, CategoryDto categoryDto)
    {
        if (id != categoryDto.Id) return BadRequest();
        await _categoryService.UpdateCategoryAsync(categoryDto);
        return NoContent();
    }

    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }

    [HttpGet("categories/search/name")]
    public async Task<IActionResult> SearchCategoriesByName(string name)
    {
        var categories = await _categoryService.SearchCategoriesByNameAsync(name);
        return Ok(categories);
    }

    [HttpGet("categories/search/description")]
    public async Task<IActionResult> SearchCategoriesByDescription(string description)
    {
        var categories = await _categoryService.SearchCategoriesByDescriptionAsync(description);
        return Ok(categories);
    }


    [HttpGet("category-attributes")]
    public async Task<IActionResult> GetAllCategoryAttributes()
    {
        var attributes = await _categoryAttributeService.GetAllAttributesAsync();
        return Ok(attributes);
    }

    [HttpGet("category-attributes/{id}")]
    public async Task<IActionResult> GetCategoryAttributeById(int id)
    {
        var attribute = await _categoryAttributeService.GetAttributeByIdAsync(id);
        return Ok(attribute);
    }

    [HttpPost("category-attributes")]
    public async Task<IActionResult> AddCategoryAttribute(CategoryAttributeDto attributeDto)
    {
        await _categoryAttributeService.AddAttributeAsync(attributeDto);
        return CreatedAtAction(nameof(GetCategoryAttributeById), new { id = attributeDto.Id }, attributeDto);
    }

    [HttpPut("category-attributes/{id}")]
    public async Task<IActionResult> UpdateCategoryAttribute(int id, CategoryAttributeDto attributeDto)
    {
        if (id != attributeDto.Id) return BadRequest();
        await _categoryAttributeService.UpdateAttributeAsync(attributeDto);
        return NoContent();
    }

    [HttpDelete("category-attributes/{id}")]
    public async Task<IActionResult> DeleteCategoryAttribute(int id)
    {
        await _categoryAttributeService.DeleteAttributeAsync(id);
        return NoContent();
    }
    
    
    [HttpGet("category-attributes/search/category")]
    public async Task<IActionResult> SearchCategoryAttributesByCategoryId(int categoryId)
    {
        var attributes = await _categoryAttributeService.SearchAttributesByCategoryIdAsync(categoryId);
        return Ok(attributes);
    }

    [HttpGet("category-attributes/search/key")]
    public async Task<IActionResult> SearchCategoryAttributesByKey(string key)
    {
        var attributes = await _categoryAttributeService.SearchAttributesByKeyAsync(key);
        return Ok(attributes);
    }
}
