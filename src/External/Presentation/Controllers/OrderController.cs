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

    [HttpGet("admin/orders")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> ListOrders()
    {
        var orders = await _orderService.ListOrdersAsync();
        return Ok(orders);
    }

    [HttpPost("admin/orders")]
    public async Task<ActionResult<OrderDto>> CreateManualOrder([FromBody] CreateManualOrderRequest request)
    {
        var order = await _orderService.CreateManualOrderAsync(request);
        return CreatedAtAction(nameof(GetOrderById), new { orderId = order.Id }, order);
    }


    [HttpPut("admin/orders/{orderId}")]
    public async Task<ActionResult<OrderDto>> UpdateManualOrder(string orderId, [FromBody] UpdateManualOrderRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            await _orderService.UpdateOrderStatusAsync(orderId, ParseClientOrderStatus(request.Status));
        }

        var order = await _orderService.GetOrderByIdAsync(orderId);
        return Ok(order);
    }

    [HttpPost("admin/orders/{orderId}/returns")]
    public async Task<IActionResult> CreateReturn(string orderId, [FromBody] CreateReturnRequest request)
    {
        request.OrderId = orderId;
        await _orderService.CreateReturnAsync(request);
        return NoContent();
    }

    [HttpGet("admin/orders/{orderId}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(string orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        return Ok(order);
    }

    // The storefront posts the checkout payload directly to /api/Order/orders.
    // Admin-created orders use /api/Order/admin/orders and the separate manual contract.
    [HttpPost("orders")]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateStorefrontOrderRequest request)
    {
        var order = await _orderService.CreateStorefrontOrderAsync(request);
        return CreatedAtAction(nameof(GetOrder), new { userId = request.UserId, orderId = order.Id }, order);
    }

    [HttpGet("orders/{orderId}")]
    public async Task<ActionResult<OrderDto>> GetStorefrontOrderById(string orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        return Ok(order);
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
    public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] UpdateOrderStatusRequest request)
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

    private static OrderStatus ParseClientOrderStatus(string status) => status.Trim().Replace("-", "_").ToLowerInvariant() switch
    {
        "pending" => OrderStatus.Pending,
        "approved" => OrderStatus.Processing,
        "processing" => OrderStatus.Processing,
        "rejected" => OrderStatus.Rejected,
        "shipped" => OrderStatus.Shipped,
        "delivered" => OrderStatus.Delivered,
        "completed" => OrderStatus.Completed,
        "cancelled" => OrderStatus.Cancelled,
        _ => throw new ArgumentException($"Unsupported order status '{status}'.")
    };
}
