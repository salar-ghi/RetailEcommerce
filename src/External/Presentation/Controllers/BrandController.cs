namespace Presentation.Controllers;


[ApiController]
[Route("api/[controller]")]
public class BrandController : ControllerBase
{
    private readonly BrandService _brandService;

    public BrandController(BrandService brandService)
    {
        _brandService = brandService;
    }

    // Brand CRUD Operations
    [HttpGet("brands")]
    public async Task<IActionResult> GetAllBrands()
    {
        var brands = await _brandService.GetAllBrandsAsync();
        return Ok(brands);
    }

    [HttpGet("brands/{id}")]
    public async Task<IActionResult> GetBrandById(int id)
    {
        var brand = await _brandService.GetBrandByIdAsync(id);
        return Ok(brand);
    }

    [HttpPost("brands")]
    public async Task<IActionResult> AddBrand(BrandDto brandDto)
    {
        await _brandService.AddBrandAsync(brandDto);
        //return CreatedAtAction(nameof(GetBrandById), new { id = brandDto.Id }, brandDto);
        return Ok("brand created successflly");
    }

    [HttpPut("brands/{id}")]
    public async Task<IActionResult> UpdateBrand(int? id, BrandUpdateDto brandDto)
    {
        if (id ==  null || id is 0) return BadRequest();
        brandDto.Id = id.Value;
        await _brandService.UpdateBrandAsync(brandDto);
        return NoContent();
    }

    [HttpDelete("brands/{id}")]
    public async Task<IActionResult> DeleteBrand(int id)
    {
        await _brandService.DeleteBrandAsync(id);
        return NoContent();
    }

    [HttpGet("brands/search/name")]
    public async Task<IActionResult> SearchBrandsByName(string name)
    {
        var brands = await _brandService.SearchBrandsByNameAsync(name);
        return Ok(brands);
    }

    [HttpGet("brands/search/description")]
    public async Task<IActionResult> SearchBrandsByDescription(string description)
    {
        var brands = await _brandService.SearchBrandsByDescriptionAsync(description);
        return Ok(brands);
    }
}