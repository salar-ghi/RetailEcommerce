namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TagController : ControllerBase
{
    private readonly TagService _tagService;
    public TagController(TagService tagService)
    {
        _tagService = tagService;
    }

    // Tag CRUD Operations
    [HttpGet("tags")]
    public async Task<IActionResult> GetAllTags()
    {
        var tags = await _tagService.GetAllTagsAsync();
        return Ok(tags);
    }

    [HttpGet("tags/{id}")]
    public async Task<IActionResult> GetTagById(int id)
    {
        var tag = await _tagService.GetTagByIdAsync(id);
        return Ok(tag);
    }

    [HttpPost("tags")]
    public async Task<IActionResult> AddTag(TagDto tagDto)
    {
        await _tagService.AddTagAsync(tagDto);
        return CreatedAtAction(nameof(GetTagById), new { id = tagDto.Id }, tagDto);
    }

    [HttpPut("tags/{id}")]
    public async Task<IActionResult> UpdateTag(int id, TagDto tagDto)
    {
        if (id != tagDto.Id) return BadRequest();
        await _tagService.UpdateTagAsync(tagDto);
        return NoContent();
    }

    [HttpDelete("tags/{id}")]
    public async Task<IActionResult> DeleteTag(int id)
    {
        await _tagService.DeleteTagAsync(id);
        return NoContent();
    }

    [HttpGet("tags/search/name")]
    public async Task<IActionResult> SearchTagsByName(string name)
    {
        var tags = await _tagService.SearchTagsByNameAsync(name);
        return Ok(tags);
    }

}
