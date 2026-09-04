namespace Application.DTOs;

/// <summary>
/// The active basket value stored in Redis for one user or guest.
/// </summary>
public class BasketDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? GuestId { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime ModifiedTime { get; set; }
    public List<BasketItemDto> Items { get; set; } = [];
    public decimal TotalPrice { get; set; }
    public int TotalItems { get; set; }
}
