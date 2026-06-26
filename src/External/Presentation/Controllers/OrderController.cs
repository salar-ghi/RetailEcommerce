using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("orders")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> ListOrders()
    {
        var orders = await _orderService.ListOrdersAsync();
        return Ok(orders);
    }

    [HttpPost("orders")]
    public async Task<ActionResult<OrderDto>> CreateManualOrder([FromBody] CreateManualOrderRequest request)
    {
        var order = await _orderService.CreateManualOrderAsync(request);
        return CreatedAtAction(nameof(GetOrderById), new { orderId = order.Id }, order);
    }

    [HttpPost("orders/{orderId}/returns")]
    public async Task<IActionResult> CreateReturn(string orderId, [FromBody] CreateReturnRequest request)
    {
        request.OrderId = orderId;
        await _orderService.CreateReturnAsync(request);
        return NoContent();
    }

    [HttpGet("orders/{orderId}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(string orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        return Ok(order);
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult<OrderDto>> CreateOrder(string userId, [FromBody] CreateBasketOrderRequest request)
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

public class CreateBasketOrderRequest
{
    public ShippingAddressDto ShippingAddress { get; set; } = new();
    public string PaymentMethod { get; set; } = "OnlineGateway";
}

public class UpdateStatusRequest
{
    public OrderStatus Status { get; set; }
}
