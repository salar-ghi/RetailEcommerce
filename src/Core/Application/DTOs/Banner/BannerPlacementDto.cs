namespace Application.DTOs;

public class BannerPlacementDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? RecommendedSize { get; set; }
}


public class CreateBannerPlacementDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? RecommendedSize { get; set; }
}
