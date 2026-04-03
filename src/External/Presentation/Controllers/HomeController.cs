namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{
    private readonly IBannerService _bannerService;
    public HomeController(IBannerService bannerService)
    {
        _bannerService = bannerService;
    }

    [HttpGet("Index")]
    public async Task<IActionResult> Index()
    {
        // get main index banners
        var heroBanners = await _bannerService.GetByPlacementAsync(BannerPageCode.HOME_TOP);

        // get some products according to different filters


        // get more product and category banners.

        return Ok();
    }



}
