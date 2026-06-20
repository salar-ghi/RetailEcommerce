namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        return Ok(await _inventoryService.GetSummaryAsync());
    }

    [HttpGet("spaces")]
    public async Task<IActionResult> GetSpaces()
    {
        return Ok(await _inventoryService.GetSpacesAsync());
    }

    [HttpGet("spaces/{id:int}")]
    public async Task<IActionResult> GetSpaceById(int id)
    {
        return Ok(await _inventoryService.GetSpaceByIdAsync(id));
    }

    [HttpPost("spaces")]
    public async Task<IActionResult> CreateSpace(CreateStorageSpaceDto dto)
    {
        var created = await _inventoryService.CreateSpaceAsync(dto);
        return CreatedAtAction(nameof(GetSpaceById), new { id = created.Id }, created);
    }

    [HttpPut("spaces/{id:int}")]
    public async Task<IActionResult> UpdateSpace(int id, CreateStorageSpaceDto dto)
    {
        await _inventoryService.UpdateSpaceAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("spaces/{id:int}")]
    public async Task<IActionResult> DeleteSpace(int id)
    {
        await _inventoryService.DeleteSpaceAsync(id);
        return NoContent();
    }

    [HttpGet("zones")]
    public async Task<IActionResult> GetZones([FromQuery] int? spaceId)
    {
        return Ok(await _inventoryService.GetZonesAsync(spaceId));
    }

    [HttpPost("zones")]
    public async Task<IActionResult> CreateZone(CreateStorageZoneDto dto)
    {
        var created = await _inventoryService.CreateZoneAsync(dto);
        return Ok(created);
    }

    [HttpPut("zones/{id:int}")]
    public async Task<IActionResult> UpdateZone(int id, CreateStorageZoneDto dto)
    {
        await _inventoryService.UpdateZoneAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("zones/{id:int}")]
    public async Task<IActionResult> DeleteZone(int id)
    {
        await _inventoryService.DeleteZoneAsync(id);
        return NoContent();
    }

    [HttpGet("shelves")]
    public async Task<IActionResult> GetShelves([FromQuery] int? spaceId, [FromQuery] int? zoneId)
    {
        return Ok(await _inventoryService.GetShelvesAsync(spaceId, zoneId));
    }

    [HttpPost("shelves")]
    public async Task<IActionResult> CreateShelf(CreateShelfDto dto)
    {
        var created = await _inventoryService.CreateShelfAsync(dto);
        return Ok(created);
    }

    [HttpPut("shelves/{id:int}")]
    public async Task<IActionResult> UpdateShelf(int id, CreateShelfDto dto)
    {
        await _inventoryService.UpdateShelfAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("shelves/{id:int}")]
    public async Task<IActionResult> DeleteShelf(int id)
    {
        await _inventoryService.DeleteShelfAsync(id);
        return NoContent();
    }

    [HttpGet("stock")]
    public async Task<IActionResult> GetStock(
        [FromQuery] long? productId,
        [FromQuery] int? spaceId,
        [FromQuery] int? zoneId,
        [FromQuery] int? shelfId)
    {
        return Ok(await _inventoryService.GetStockAsync(productId, spaceId, zoneId, shelfId));
    }

    [HttpGet("inputs")]
    public async Task<IActionResult> GetInputs(
        [FromQuery] long? productId,
        [FromQuery] int? supplierId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        return Ok(await _inventoryService.GetInputsAsync(productId, supplierId, from, to));
    }

    [HttpGet("inputs/recent")]
    public async Task<IActionResult> GetRecentInputs([FromQuery] int limit = 10)
    {
        return Ok(await _inventoryService.GetRecentInputsAsync(limit));
    }

    [HttpGet("inputs/expiring")]
    public async Task<IActionResult> GetExpiringInputs([FromQuery] int days = 30)
    {
        return Ok(await _inventoryService.GetExpiringInputsAsync(days));
    }

    [HttpPost("inputs")]
    public async Task<IActionResult> RegisterInput(InventoryInputDto dto)
    {
        var created = await _inventoryService.RegisterInputAsync(dto);
        return Ok(created);
    }

    [HttpPost("stock/{stockId:long}/reserve")]
    public async Task<IActionResult> Reserve(long stockId, [FromQuery] int quantity)
    {
        await _inventoryService.ReserveAsync(stockId, quantity);
        return NoContent();
    }

    [HttpPost("stock/{stockId:long}/release-reservation")]
    public async Task<IActionResult> ReleaseReservation(long stockId, [FromQuery] int quantity)
    {
        await _inventoryService.ReleaseReservationAsync(stockId, quantity);
        return NoContent();
    }
}
