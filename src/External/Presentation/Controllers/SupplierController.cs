﻿using Domain.Entities;
using System.Security.Claims;

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


    [HttpGet("suppliers")]
    public async Task<IActionResult> GetAllSuppliers()
    {
        var suppliers = await _supplierService.GetAllSuppliersAsync();
        return Ok(suppliers);
    }

    [HttpGet("suppliers/{id}")]
    public async Task<IActionResult> GetSupplierById(int id)
    {
        var supplier = await _supplierService.GetSupplierByIdAsync(id);
        return Ok(supplier);
    }

    [Authorize]
    [HttpPost("suppliers")]
    public async Task<IActionResult> AddSupplier(SupplierDto supplierDto)
    {
        try
        {
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(userId))
            //{
            //    return Unauthorized();
            //}
            //var claims = User.Claims.Select(c => new { c.Type, c.Value });

            await _supplierService.AddSupplierAsync(supplierDto);
            return Ok(new { Message = "User added successfully", id = supplierDto.Id });
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [HttpPut("suppliers/{id}")]
    public async Task<IActionResult> UpdateSupplier(int id, SupplierDto supplierDto)
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
