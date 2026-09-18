namespace Domain.Entities;

/// <summary>
/// A display block in a product's long-form introduction.
/// </summary>
public class ProductContentBlock : BaseModel<long>
{
    public long ProductId { get; set; }
    public Product Product { get; set; } = null!;

    // Preserves the client-generated identifier so editor blocks remain stable on reload.
    public string? ClientId { get; set; }
    public string Type { get; set; } = null!;
    public string? Text { get; set; }
    public string? ImageUrl { get; set; }
    public string? Caption { get; set; }
    public int SortOrder { get; set; }
}
