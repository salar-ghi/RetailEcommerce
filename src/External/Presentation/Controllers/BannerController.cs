namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BannerController : ControllerBase
{
    private readonly IBannerService _bannerService;
    private readonly IBannerPlacementService _placementService;
    public BannerController(IBannerService bannerService, IBannerPlacementService placementService)
    {
        _bannerService = bannerService;
        _placementService = placementService;
    }

    // GET: api/banner/placements (for React dropdown/select multiple)
    [HttpGet("placements")]
    public async Task<ActionResult<IEnumerable<BannerPlacementDto>>> GetPlacements()
    {
        return Ok(await _placementService.GetAllAsync());
    }

    // POST: api/banner/placements (create new placement dynamically)
    [HttpPost("placements")]
    public async Task<ActionResult<BannerPlacementDto>> CreatePlacement(CreateBannerPlacementDto dto)
    {
        var placement = await _placementService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetPlacementById), new { id = placement.Id }, placement);
    }

    // GET: api/banner/placements/{id}
    [HttpGet("placements/{id}")]
    public async Task<ActionResult<BannerPlacementDto>> GetPlacementById(int id)
    {
        var placement = await _placementService.GetByIdAsync(id);
        if (placement == null) return NotFound();
        return Ok(placement);
    }

    // DELETE: api/banner/placements/{id}
    [HttpDelete("placements/{id}")]
    public async Task<IActionResult> DeletePlacement(int id)
    {
        await _placementService.DeleteAsync(id);
        return NoContent();
    }

    // GET: api/banner (all banners, for admin)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BannerDto>>> GetAll()
    {
        return Ok(await _bannerService.GetAllAsync());
    }

    // GET: api/banner/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<BannerDto>> GetById(int id)
    {
        return Ok(await _bannerService.GetByIdAsync(id));
    }

    // GET: api/banner/active (all active, for frontend optional)
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<BannerDto>>> GetActive()
    {
        return Ok(await _bannerService.GetActiveAsync());
    }

    // GET: api/banner/placement/{placementKey} (active by placement, for React page load)
    [HttpGet("placement/{placementKey}")]
    public async Task<ActionResult<IEnumerable<BannerDto>>> GetByPlacement(string placementKey)
    {
        return Ok(await _bannerService.GetByPlacementAsync(placementKey));
    }

    // POST: api/banner (create from React form, with multiple PlacementIds)
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateBannerDto dto)
    {
        var id = await _bannerService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    // PUT: api/banner/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateBannerDto dto)
    {
        if (id != dto.Id) return BadRequest();
        await _bannerService.UpdateAsync(dto);
        return NoContent();
    }

    // DELETE: api/banner/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _bannerService.DeleteAsync(id);
        return NoContent();
    }

}
