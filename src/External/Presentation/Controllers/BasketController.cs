namespace Presentation.Controllers;

using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;
    private readonly ICurrentUserService _currentUserService;

    public BasketController(IBasketService basketService, ICurrentUserService currentUserService)
    {
        _basketService = basketService;
        _currentUserService = currentUserService;
    }

    [HttpGet("baskets")]
    public async Task<ActionResult<IEnumerable<BasketDto>>> ListBaskets([FromQuery] string? status, [FromQuery] string? userId)
    {
        var baskets = await _basketService.ListBasketsAsync(status, userId);
        return Ok(baskets);
    }

    [HttpGet("baskets/{id}")]
    public async Task<ActionResult<BasketDto>> GetBasketById(string id)
    {
        return Ok(await _basketService.GetBasketByIdAsync(id));
    }

    [HttpPut("baskets/{id}")]
    public async Task<ActionResult<BasketDto>> UpdateBasket(string id, [FromBody] UpdateBasketRequest request)
    {
        return Ok(await _basketService.UpdateBasketAsync(id, request));
    }

    [HttpDelete("baskets/{id}")]
    public async Task<ActionResult<BasketActionResultDto>> DeleteBasket(string id)
    {
        return Ok(await _basketService.DeleteBasketAsync(id));
    }

    [HttpPost("baskets/{id}/convert")]
    public async Task<ActionResult<BasketActionResultDto>> ConvertBasket(string id)
    {
        return Ok(await _basketService.ConvertBasketAsync(id));
    }

    [HttpPost("baskets/{id}/remind")]
    public async Task<ActionResult<BasketActionResultDto>> RemindBasketOwner(string id)
    {
        return Ok(await _basketService.RemindBasketOwnerAsync(id));
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<BasketDto>> GetBasket(string userId)
    {
        var basket = await _basketService.GetBasketAsync(ResolveBasketOwner(userId));
        return Ok(basket);
    }

    [HttpPost("{userId}/items")]
    public async Task<IActionResult> AddItem(string userId, [FromBody] AddItemRequest request)
    {
        await _basketService.AddItemToBasketAsync(ResolveBasketOwner(userId), request.ProductId, request.Quantity);
        return Ok();
    }

    [HttpPut("{userId}/items")]
    public async Task<IActionResult> UpdateItemQuantity(string userId, [FromBody] UpdateQuantityRequest request)
    {
        await _basketService.UpdateItemQuantityAsync(ResolveBasketOwner(userId), request.ProductId, request.Quantity);
        return Ok();
    }

    [HttpDelete("{userId}/items/{productId:int}")]
    public async Task<IActionResult> RemoveItem(string userId, int productId)
    {
        await _basketService.RemoveItemFromBasketAsync(ResolveBasketOwner(userId), productId);
        return Ok();
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> ClearBasket(string userId)
    {
        await _basketService.ClearBasketAsync(ResolveBasketOwner(userId));
        return Ok();
    }

    // Logged-in clients must always use the identity in their validated token,
    // rather than a stale guest id supplied in the URL.  Guests continue to use
    // the route value as their anonymous basket key.
    private string ResolveBasketOwner(string routeUserId) =>
        string.IsNullOrWhiteSpace(_currentUserService.UserId) ? routeUserId : _currentUserService.UserId;
}

public class AddItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateQuantityRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
