namespace Domain.Entities;

public class ProductReview : BaseModel<long>
{
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public bool IsApproved { get; set; }
}