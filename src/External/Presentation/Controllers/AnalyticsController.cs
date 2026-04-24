namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }



    // ==================== MOST SELLING ====================

    [HttpGet("products/most-selling")]
    public async Task<IActionResult> GetTopSellingProducts([FromQuery] int top = 10)
    {
        var products = await _analyticsService.GetTopSellingProductsAsync(top);
        return Ok(products);
    }

    [HttpGet("products/most-selling/category/{categoryName}")]
    public async Task<IActionResult> GetTopSellingInCategory(
        string categoryName,
        [FromQuery] int top = 10)
    {
        var products = await _analyticsService.GetTopSellingProductsInCategoryAsync(categoryName, top);
        return Ok(products);
    }

    [HttpGet("products/most-selling/brand/{brandName}")]
    public async Task<IActionResult> GetTopSellingByBrand(
        string brandName,
        [FromQuery] int top = 10)
    {
        var products = await _analyticsService.GetTopSellingProductsByBrandAsync(brandName, top);
        return Ok(products);
    }




}
