namespace Application.DTOs;

public class ProductReviewDto
{
    public int Id { get; set; }
    public string ReviewerName { get; set; }
    public string Comment { get; set; }
    public int Rating { get; set; }
    public DateTime ReviewDate { get; set; }
    public int ProductId { get; set; }
}