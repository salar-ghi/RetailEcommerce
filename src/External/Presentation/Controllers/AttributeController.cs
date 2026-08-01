using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttributeController : ControllerBase
{
    private readonly AppDbContext _db;

    public AttributeController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("attributes")]
    public async Task<IActionResult> GetAll()
    {
        var attributes = await _db.AttributeDefinitions
            .Where(a => !a.IsDeleted)
            .Include(a => a.Options.Where(o => !o.IsDeleted))
            .OrderBy(a => a.SortOrder).ThenBy(a => a.Name)
            .ToListAsync();

        return Ok(attributes);
    }

    [HttpGet("attributes/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var attribute = await _db.AttributeDefinitions
            .Include(a => a.Options.Where(o => !o.IsDeleted))
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

        return attribute is null ? NotFound() : Ok(attribute);
    }

    [HttpPost("attributes")]
    public async Task<IActionResult> Create(AttributeDefinition attribute)
    {
        Normalize(attribute);
        _db.AttributeDefinitions.Add(attribute);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = attribute.Id }, attribute);
    }

    [HttpPut("attributes/{id:int}")]
    public async Task<IActionResult> Update(int id, AttributeDefinition request)
    {
        var attribute = await _db.AttributeDefinitions
            .Include(a => a.Options)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        if (attribute is null) return NotFound();

        attribute.Code = request.Code;
        attribute.Name = request.Name;
        attribute.DataType = request.DataType;
        attribute.Unit = request.Unit;
        attribute.IsFilterable = request.IsFilterable;
        attribute.IsSearchable = request.IsSearchable;
        attribute.IsComparable = request.IsComparable;
        attribute.IsRequired = request.IsRequired;
        attribute.IsVariantAttribute = request.IsVariantAttribute;
        attribute.SortOrder = request.SortOrder;
        attribute.ValidationRegex = request.ValidationRegex;
        attribute.MinValue = request.MinValue;
        attribute.MaxValue = request.MaxValue;

        attribute.Options.Clear();
        foreach (var option in request.Options ?? [])
        {
            option.Id = 0;
            attribute.Options.Add(option);
        }

        Normalize(attribute);
        await _db.SaveChangesAsync();
        return Ok(attribute);
    }

    [HttpDelete("attributes/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var attribute = await _db.AttributeDefinitions.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        if (attribute is null) return NotFound();
        attribute.IsDeleted = true;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static void Normalize(AttributeDefinition attribute)
    {
        attribute.Code = attribute.Code?.Trim().ToLowerInvariant() ?? string.Empty;
        attribute.Name = attribute.Name?.Trim() ?? string.Empty;
        foreach (var option in attribute.Options ?? [])
        {
            option.Value = option.Value?.Trim() ?? string.Empty;
            option.Label = string.IsNullOrWhiteSpace(option.Label) ? option.Value : option.Label.Trim();
        }
    }
}
