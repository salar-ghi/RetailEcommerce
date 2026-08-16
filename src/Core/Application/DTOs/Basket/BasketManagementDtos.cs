namespace Application.DTOs;

public class BasketQueryDto
{
    public string? Status { get; set; }
    public string? UserId { get; set; }
}

public class UpdateBasketRequest
{
    public string? AdminNotes { get; set; }
    public List<UpdateBasketItemRequest> Items { get; set; } = new();
}

public class UpdateBasketItemRequest
{
    public int Id { get; set; }
    public long ProductId { get; set; }
    public int Quantity { get; set; }
}

public class BasketActionResultDto
{
    public string BasketId { get; set; }
    public string Message { get; set; }
    public DateTime ActionedAt { get; set; } = DateTime.UtcNow;
    public string? OrderId { get; set; }
}
