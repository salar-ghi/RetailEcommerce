namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttributeController : ControllerBase
{
    private readonly AttributeDefinitionService _attributeDefinitionService;

    public AttributeController(AttributeDefinitionService attributeDefinitionService)
    {
        _attributeDefinitionService = attributeDefinitionService;
    }

    [HttpGet("attributes")]
    public async Task<IActionResult> GetAll()
    {
        var attributes = await _attributeDefinitionService.GetAllAsync();
        return Ok(attributes);
    }

    [HttpGet("attributes/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var attribute = await _attributeDefinitionService.GetByIdAsync(id);
        return Ok(attribute);
    }

    [HttpPost("attributes")]
    public async Task<IActionResult> Create(UpsertAttributeDefinitionDto request)
    {
        var attribute = await _attributeDefinitionService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = attribute.Id }, attribute);
    }

    [HttpPut("attributes/{id:int}")]
    public async Task<IActionResult> Update(int id, UpsertAttributeDefinitionDto request)
    {
        var attribute = await _attributeDefinitionService.UpdateAsync(id, request);
        return Ok(attribute);
    }

    [HttpDelete("attributes/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _attributeDefinitionService.DeleteAsync(id);
        return NoContent();
    }
}
