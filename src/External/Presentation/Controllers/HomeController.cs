namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{
    private readonly IBannerService _bannerService;
    private readonly IProductService _productService;

    private readonly ILogger<HomeController> _logger;
    public HomeController(IBannerService bannerService, IProductService productService)
    {
        _bannerService = bannerService;
        _productService = productService;
    }

    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        // get main index banners
        var heroBanners = await _bannerService.GetByPlacementAsync(BannerPageCode.HOME_TOP);

        // get some products according to different filters
        //var products = await _productService.GetProductsByCategory("Electronics");

        // get more product and category banners.

        return Ok();
    }



}
