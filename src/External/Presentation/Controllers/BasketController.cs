namespace Presentation.Controllers;

using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IImageHelper _imageHelper;

    public BasketController(
        IBasketService basketService,
        ICurrentUserService currentUserService,
        IImageHelper imageHelper)
    {
        _basketService = basketService;
        _currentUserService = currentUserService;
        _imageHelper = imageHelper;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<BasketDto>> GetBasket(string userId)
    {
        var basket = await _basketService.GetBasketAsync(ResolveBasketOwner(userId));
        await ConvertBasketImagesToBase64Async(basket);
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

    // Basket state stores image paths in Redis. Convert only the response values so
    // the cache remains compact while storefront clients receive usable data URLs.
    private async Task ConvertBasketImagesToBase64Async(BasketDto basket)
    {
        foreach (var item in basket.Items)
        {
            item.CoverImage = await ConvertImageToBase64Async(item.CoverImage);
            item.Image = await ConvertImageToBase64Async(item.Image);
        }
    }

    private async Task<string?> ConvertImageToBase64Async(string? image)
    {
        if (string.IsNullOrWhiteSpace(image) || image.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            return image;

        return await _imageHelper.GetImageBase64(image);
    }
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
