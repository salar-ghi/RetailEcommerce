namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CoffeeController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CoffeeController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("Index")]
    public async Task<ActionResult<CategoryPageDataDto>> Index()
    {
        var result = await _categoryService.GetCategoryPageDataAsync("coffee");
        return Ok(result);
    }
}
