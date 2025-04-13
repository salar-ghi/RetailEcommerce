namespace Presentation.Controllers;

using Application.Interfaces;

// Presentation/Controllers/BasketController.cs
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;

    public BasketController(IBasketService basketService)
    {
        _basketService = basketService;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<BasketDto>> GetBasket(string userId)
    {
        var basket = await _basketService.GetBasketAsync(userId);
        return Ok(basket);
    }

    [HttpPost("{userId}/items")]
    public async Task<IActionResult> AddItem(string userId, [FromBody] AddItemRequest request)
    {
        await _basketService.AddItemToBasketAsync(userId, request.ProductId, request.Quantity);
        return Ok();
    }

    [HttpPut("{userId}/items")]
    public async Task<IActionResult> UpdateItemQuantity(string userId, [FromBody] UpdateQuantityRequest request)
    {
        await _basketService.UpdateItemQuantityAsync(userId, request.ProductId, request.Quantity);
        return Ok();
    }

    [HttpDelete("{userId}/items/{productId}")]
    public async Task<IActionResult> RemoveItem(string userId, int productId)
    {
        await _basketService.RemoveItemFromBasketAsync(userId, productId);
        return Ok();
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> ClearBasket(string userId)
    {
        await _basketService.ClearBasketAsync(userId);
        return Ok();
    }
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