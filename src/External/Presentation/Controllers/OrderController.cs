using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

using Application.Interfaces;

using Domain.Enums;

// Presentation/Controllers/OrderController.cs
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult<OrderDto>> CreateOrder(string userId, [FromBody] CreateOrderRequest request)
    {
        var order = await _orderService.CreateOrderFromBasketAsync(userId, request.ShippingAddress, request.PaymentMethod);
        return CreatedAtAction(nameof(GetOrder), new { userId, orderId = order.Id }, order);
    }

    [HttpGet("{userId}/{orderId}")]
    public async Task<ActionResult<OrderDto>> GetOrder(string userId, string orderId)
    {
        var order = await _orderService.GetOrderAsync(userId, orderId);
        return Ok(order);
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetUserOrders(string userId)
    {
        var orders = await _orderService.GetUserOrdersAsync(userId);
        return Ok(orders);
    }

    [HttpPut("{orderId}/status")]
    public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] UpdateStatusRequest request)
    {
        await _orderService.UpdateOrderStatusAsync(orderId, request.Status);
        return NoContent();
    }

    [HttpDelete("{orderId}")]
    public async Task<IActionResult> CancelOrder(string orderId)
    {
        await _orderService.CancelOrderAsync(orderId);
        return NoContent();
    }
}

public class CreateOrderRequest
{
    public ShippingAddressDto ShippingAddress { get; set; }
    public string PaymentMethod { get; set; }
}

public class UpdateStatusRequest
{
    public OrderStatus Status { get; set; }
}