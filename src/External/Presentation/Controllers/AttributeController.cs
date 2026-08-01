using Domain.Entities;
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

        return Ok(attributes.Select(ToDto));
    }

    [HttpGet("attributes/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var attribute = await _db.AttributeDefinitions
            .Include(a => a.Options.Where(o => !o.IsDeleted))
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

        return attribute is null ? NotFound() : Ok(ToDto(attribute));
    }

    [HttpPost("attributes")]
    public async Task<IActionResult> Create(UpsertAttributeDefinitionDto request)
    {
        var attribute = ToEntity(request);
        Normalize(attribute);
        _db.AttributeDefinitions.Add(attribute);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = attribute.Id }, ToDto(attribute));
    }

    [HttpPut("attributes/{id:int}")]
    public async Task<IActionResult> Update(int id, UpsertAttributeDefinitionDto request)
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
            attribute.Options.Add(new AttributeOption
            {
                Value = option.Value,
                Label = option.Label,
                SortOrder = option.SortOrder,
                IsActive = option.IsActive
            });
        }

        Normalize(attribute);
        await _db.SaveChangesAsync();
        return Ok(ToDto(attribute));
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

    private static AttributeDefinition ToEntity(UpsertAttributeDefinitionDto request) => new()
    {
        Code = request.Code,
        Name = request.Name,
        DataType = request.DataType,
        Unit = request.Unit,
        IsFilterable = request.IsFilterable,
        IsSearchable = request.IsSearchable,
        IsComparable = request.IsComparable,
        IsRequired = request.IsRequired,
        IsVariantAttribute = request.IsVariantAttribute,
        SortOrder = request.SortOrder,
        ValidationRegex = request.ValidationRegex,
        MinValue = request.MinValue,
        MaxValue = request.MaxValue,
        Options = request.Options.Select(o => new AttributeOption
        {
            Value = o.Value,
            Label = o.Label,
            SortOrder = o.SortOrder,
            IsActive = o.IsActive
        }).ToList()
    };

    private static AttributeDefinitionDto ToDto(AttributeDefinition attribute) => new()
    {
        Id = attribute.Id,
        Code = attribute.Code,
        Name = attribute.Name,
        DataType = attribute.DataType,
        Unit = attribute.Unit,
        IsFilterable = attribute.IsFilterable,
        IsSearchable = attribute.IsSearchable,
        IsComparable = attribute.IsComparable,
        IsRequired = attribute.IsRequired,
        IsVariantAttribute = attribute.IsVariantAttribute,
        SortOrder = attribute.SortOrder,
        ValidationRegex = attribute.ValidationRegex,
        MinValue = attribute.MinValue,
        MaxValue = attribute.MaxValue,
        Options = attribute.Options?.Where(o => !o.IsDeleted).Select(o => new AttributeOptionDto
        {
            Id = o.Id,
            Value = o.Value,
            Label = o.Label,
            SortOrder = o.SortOrder,
            IsActive = o.IsActive
        }).ToList() ?? new List<AttributeOptionDto>()
    };

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
