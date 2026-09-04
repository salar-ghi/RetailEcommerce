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

    [HttpGet("{userId}")]
    public async Task<ActionResult<BasketDto>> GetBasket(string userId)
    {
        var basket = await _basketService.GetBasketAsync(ResolveBasketOwner(userId));
        return Ok(basket);
    }

    [HttpPost("{userId}/items")]
    public async Task<ActionResult<BasketDto>> AddItem(string userId, [FromBody] AddItemRequest request)
    {
        return Ok(await _basketService.AddItemToBasketAsync(ResolveBasketOwner(userId), request.ProductId, request.Quantity));
    }

    [HttpPut("{userId}/items")]
    public async Task<ActionResult<BasketDto>> UpdateItemQuantity(string userId, [FromBody] UpdateQuantityRequest request)
    {
        return Ok(await _basketService.UpdateItemQuantityAsync(ResolveBasketOwner(userId), request.ProductId, request.Quantity));
    }

    [HttpDelete("{userId}/items/{productId:long}")]
    public async Task<ActionResult<BasketDto>> RemoveItem(string userId, long productId)
    {
        return Ok(await _basketService.RemoveItemFromBasketAsync(ResolveBasketOwner(userId), productId));
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
    public long ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateQuantityRequest
{
    public long ProductId { get; set; }
    public int Quantity { get; set; }
}
