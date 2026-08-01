using Presentation.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;
    private readonly CategoryAttributeService _categoryAttributeService;
    private readonly AppDbContext _db;
    public CategoryController(CategoryService categoryService,
        CategoryAttributeService categoryAttributeService,
        AppDbContext db)
    {
        _categoryService = categoryService;
        _categoryAttributeService = categoryAttributeService;
        _db = db;
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategoriesWithDetailsAsync();
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
        return Ok("category created successflly");
    }

    [HttpPut("categories/{id}")]
    public async Task<IActionResult> UpdateCategory(int? id, CategoryDto categoryDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        if (id == null || id is 0) return BadRequest();

        categoryDto.Id = id.Value;
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


    [HttpGet("categories/{categoryId:int}/attributes")]
    public async Task<IActionResult> GetCategoryAttributeDefinitions(int categoryId)
    {
        var attributes = await _db.CategoryAttributeDefinitions
            .Where(ca => ca.CategoryId == categoryId && !ca.IsDeleted)
            .Include(ca => ca.AttributeDefinition)
                .ThenInclude(a => a.Options.Where(o => !o.IsDeleted))
            .OrderBy(ca => ca.SortOrder)
            .ThenBy(ca => ca.AttributeDefinition.SortOrder)
            .ThenBy(ca => ca.AttributeDefinition.Name)
            .ToListAsync();

        return Ok(attributes);
    }

    [HttpPost("categories/{categoryId:int}/attributes")]
    public async Task<IActionResult> AssignCategoryAttribute(int categoryId, CategoryAttributeDefinition request)
    {
        var exists = await _db.CategoryAttributeDefinitions.AnyAsync(ca =>
            ca.CategoryId == categoryId && ca.AttributeDefinitionId == request.AttributeDefinitionId && !ca.IsDeleted);
        if (exists) return Conflict("Attribute is already assigned to this category.");

        request.Id = 0;
        request.CategoryId = categoryId;
        request.Category = null!;
        request.AttributeDefinition = null!;
        _db.CategoryAttributeDefinitions.Add(request);
        await _db.SaveChangesAsync();

        await _db.Entry(request).Reference(ca => ca.AttributeDefinition).LoadAsync();
        await _db.Entry(request.AttributeDefinition).Collection(a => a.Options).LoadAsync();
        return CreatedAtAction(nameof(GetCategoryAttributeDefinitions), new { categoryId }, request);
    }

    [HttpPut("categories/{categoryId:int}/attributes/{id:int}")]
    public async Task<IActionResult> UpdateCategoryAttributeDefinition(int categoryId, int id, CategoryAttributeDefinition request)
    {
        var row = await _db.CategoryAttributeDefinitions
            .Include(ca => ca.AttributeDefinition)
            .ThenInclude(a => a.Options.Where(o => !o.IsDeleted))
            .FirstOrDefaultAsync(ca => ca.Id == id && ca.CategoryId == categoryId && !ca.IsDeleted);
        if (row is null) return NotFound();

        row.IsRequired = request.IsRequired;
        row.SortOrder = request.SortOrder;
        row.IsVisibleOnProductPage = request.IsVisibleOnProductPage;
        row.IsFilterable = request.IsFilterable;
        await _db.SaveChangesAsync();
        return Ok(row);
    }

    [HttpDelete("categories/{categoryId:int}/attributes/{id:int}")]
    public async Task<IActionResult> RemoveCategoryAttributeDefinition(int categoryId, int id)
    {
        var row = await _db.CategoryAttributeDefinitions
            .FirstOrDefaultAsync(ca => ca.Id == id && ca.CategoryId == categoryId && !ca.IsDeleted);
        if (row is null) return NotFound();
        row.IsDeleted = true;
        await _db.SaveChangesAsync();
        return NoContent();
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
