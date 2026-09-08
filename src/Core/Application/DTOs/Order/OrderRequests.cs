using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

/// <summary>
/// The checkout contract used by the storefront.  It intentionally differs from
/// <see cref="CreateManualOrderRequest"/>, which is the richer admin-panel contract.
/// </summary>
public sealed class CreateStorefrontOrderRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public List<CreateStorefrontOrderItemRequest> Items { get; set; } = new();

    // Kept for compatibility with the checkout client. The server derives the
    // persisted order total from Items and does not trust this client-supplied value.
    public decimal TotalPrice { get; set; }

    public string? ShippingAddress { get; set; }

    public string PaymentMethod { get; set; } = "online";
}

public sealed class CreateStorefrontOrderItemRequest
{
    [Range(1, long.MaxValue)]
    public long ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
}

public sealed class UpdateOrderStatusRequest
{
    public OrderStatus Status { get; set; }
}

public sealed class UpdateManualOrderRequest
{
    public string? Status { get; set; }
}
