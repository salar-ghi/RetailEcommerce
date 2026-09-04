namespace Application.DTOs;

public class BasketItemDto
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    /// <summary>
    /// The product's primary image URL at the time the item was added to the basket.
    /// This is stored with the Redis-backed basket so the basket can be displayed
    /// without an additional product lookup.
    /// </summary>
    public string? CoverImage { get; set; }
    /// <summary>
    /// Image field consumed by storefront basket clients. It mirrors
    /// <see cref="CoverImage"/> so existing cached baskets remain compatible
    /// while the client can render <c>item.image</c> directly.
    /// </summary>
    public string? imageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
