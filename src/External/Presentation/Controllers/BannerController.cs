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

    
    [HttpGet("placements")]
    public async Task<ActionResult<IEnumerable<BannerPlacementDto>>> GetPlacements()
    {
        return Ok(await _placementService.GetAllAsync());
    }

    
    [HttpPost("placements")]
    public async Task<ActionResult<BannerPlacementDto>> CreatePlacement(CreateBannerPlacementDto dto)
    {
        var placement = await _placementService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetPlacementById), new { id = placement.Id }, placement);
    }


    [HttpGet("placements/{id}")]
    public async Task<ActionResult<BannerPlacementDto>> GetPlacementById(int id)
    {
        var placement = await _placementService.GetByIdAsync(id);
        if (placement == null) return NotFound();
        return Ok(placement);
    }

    [HttpDelete("placements/{id}")]
    public async Task<IActionResult> DeletePlacement(int id)
    {
        await _placementService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BannerDto>>> GetAll()
    {
        return Ok(await _bannerService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BannerDto>> GetById(int id)
    {
        return Ok(await _bannerService.GetByIdAsync(id));
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<BannerDto>>> GetActive()
    {
        return Ok(await _bannerService.GetActiveAsync());
    }

    [HttpGet("placement/{placementKey}")]
    public async Task<ActionResult<IEnumerable<BannerDto>>> GetByPlacement(string placementKey)
    {
        return Ok(await _bannerService.GetByPlacementAsync(placementKey));
    }

    [HttpPost("banner")]
    public async Task<ActionResult<int>> Create(CreateBannerDto dto)
    {
        var id = await _bannerService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateBannerDto dto)
    {
        if (id != dto.Id) return BadRequest();
        await _bannerService.UpdateAsync(dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _bannerService.DeleteAsync(id);
        return NoContent();
    }

}
