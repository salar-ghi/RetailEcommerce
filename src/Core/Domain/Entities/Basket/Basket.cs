namespace Domain.Entities;

public class Basket : BaseModel<string>
{
    public string? UserId { get; set; }
    public User? User { get; set; }

    public string? GuestId { get; set; }
    public BasketType Type { get; set; }

    public List<BasketItem> Items { get; set; } = new();

    [Timestamp]
    public byte[] ConcurrencyToken { get; set; }

    public decimal TotalPrice => Items.Sum(item => item.UnitPrice * item.Quantity);
    public int TotalItems => Items.Sum(item => item.Quantity);
}
