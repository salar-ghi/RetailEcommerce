using Application.Common;
using Application.DTOs;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SupplierController : ControllerBase
{
    private readonly SupplierService _supplierService;
    private readonly ICurrentUserService _currentUserService;
    public SupplierController(SupplierService supplierService, ICurrentUserService currentUserService)
    {
        _supplierService = supplierService;
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
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

    //[HttpPost("approve")]
    [HttpPut("suppliers/{id}/approve")]
    public async Task<ActionResult<SupplierDto>> ApproveSupplier([FromRoute] int id)
    {
        var request = new ApproveSupplierDto 
        {
            Id = id,
        };
        
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
        var result = await _supplierService.CreateSupplierAsync(supplierDto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        return Ok(new { result.Value });
    }

    [HttpPost("register-new")]
    public async Task<ActionResult<SupplierDto>> RegisterNewSupplier(SupplierRegistrationDto supplierDto)
    {
        var result = await _supplierService.RegisterSupplierAsync(supplierDto);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        return Ok(new { result.Value });
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
