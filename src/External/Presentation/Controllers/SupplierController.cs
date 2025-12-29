namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SupplierController : ControllerBase
{
    private readonly SupplierService _supplierService;
    public SupplierController(SupplierService supplierService)
    {
        _supplierService = supplierService;
    }


    //[HttpGet("suppliers")]
    //public async Task<IActionResult> GetAllSuppliers()
    //{
    //    var suppliers = await _supplierService.GetAllSuppliersAsync();
    //    return Ok(suppliers);
    //}

    [HttpGet("suppliers")]
    public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAllSuppliers()
    {
        var suppliers = await _supplierService.GetAllSuppliersAsync();
        return Ok(suppliers);
    }

    [HttpPost("approve")]
    public async Task<ActionResult<SupplierDto>> ApproveSupplier([FromBody] ApproveSupplierDto request)
    {
        var supplier = await _supplierService.ApproveSupplierAsync(request);
        return Ok(supplier);
    }

    [HttpGet("suppliers/{id}")]
    public async Task<IActionResult> GetSupplierById(int id)
    {
        var supplier = await _supplierService.GetSupplierByIdAsync(id);
        return Ok(supplier);
    }

    [HttpPost("suppliers")]
    public async Task<IActionResult> AddSupplier(SupplierRegistrationDto supplierDto)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            await _supplierService.AddSupplierAsync(supplierDto);
            return Ok(new { Message = "User added successfully", Name = supplierDto.Name });
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [HttpPost("register-new")]
    public async Task<ActionResult<SupplierDto>> RegisterNewSupplier([FromBody] SupplierRegistrationDto request)
    {
        await _supplierService.AddSupplierAsync(request);
        return Ok();
    }

    [HttpPut("suppliers/{id}")]
    public async Task<IActionResult> UpdateSupplier(int id, UpdateSupplierStatusDto supplierDto)
    {
        if (id != supplierDto.Id) return BadRequest();
        await _supplierService.UpdateSupplierAsync(supplierDto);
        return NoContent();
    }

    [HttpDelete("suppliers/{id}")]
    public async Task<IActionResult> DeleteSupplier(int id)
    {
        await _supplierService.DeleteSupplierAsync(id);
        return NoContent();
    }

    [HttpGet("suppliers/search/name")]
    public async Task<IActionResult> SearchSuppliersByName(string name)
    {
        var suppliers = await _supplierService.SearchSuppliersByNameAsync(name);
        return Ok(suppliers);
    }

    [HttpGet("suppliers/search/contact")]
    public async Task<IActionResult> SearchSuppliersByContactInfo(string contactInfo)
    {
        var suppliers = await _supplierService.SearchSuppliersByContactInfoAsync(contactInfo);
        return Ok(suppliers);
    }

}
