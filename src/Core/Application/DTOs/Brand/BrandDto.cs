namespace Application.DTOs;

public record BrandDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Logo { get; set; }
    public DateTime CreatedTime { get; set; }
}

public record BrandUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Logo { get; set; }
    public DateTime ModifiedBy { get; set; } = DateTime.Now;
}