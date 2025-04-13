namespace Application.DTOs;

public class ProductImageDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }
    public bool IsPrimary { get; set; }
    public int ProductId { get; set; }
}