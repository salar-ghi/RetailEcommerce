using Application.Common.Json;
using System.Text.Json.Serialization;

namespace Application.DTOs;

public record BannerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BannerName
    {
        get => Name;
        set => Name = value;
    }
    public string? Description { get; set; }
    public string? BannerDescription
    {
        get => Description;
        set => Description = value;
    }
    public string? AltText { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string BannerImage
    {
        get => ImageUrl;
        set => ImageUrl = value;
    }
    public string? Link { get; set; }
    public string? CallToActionText { get; set; }
    [JsonConverter(typeof(NumericEnumJsonConverter<BannerType>))]
    public BannerType Type { get; set; }
    [JsonConverter(typeof(NumericEnumJsonConverter<BannerType>))]
    public BannerType BannerType
    {
        get => Type;
        set => Type = value;
    }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    [JsonConverter(typeof(NumericEnumJsonConverter<BannerSize>))]
    public BannerSize Size { get; set; }
    [JsonConverter(typeof(NumericEnumJsonConverter<BannerSize>))]
    public BannerSize BannerSize
    {
        get => Size;
        set => Size = value;
    }
    public bool IsActive { get; set; }
    public int Priority { get; set; }
    public int ClickCount { get; set; }
    public int ViewCount { get; set; }
    public List<PlacementDto> Placements { get; set; } = new();
}

public class PlacementDto
{
    public int Id { get; set; }

    public string Name { get; set; }
    public string Code { get; set; } = string.Empty;
}

public class CreateBannerDto
{
    public string Name { get; set; } = string.Empty;
    public string BannerName
    {
        get => Name;
        set => Name = value;
    }
    public string? Description { get; set; }
    public string? BannerDescription
    {
        get => Description;
        set => Description = value;
    }
    public string ImageUrl { get; set; } = string.Empty;
    public string BannerImage
    {
        get => ImageUrl;
        set => ImageUrl = value;
    }
    public string? AltText { get; set; }
    public string? Link { get; set; }
    public string? CallToActionText { get; set; }
    [JsonConverter(typeof(NumericEnumJsonConverter<BannerType>))]
    public BannerType Type { get; set; }
    [JsonConverter(typeof(NumericEnumJsonConverter<BannerType>))]
    public BannerType BannerType
    {
        get => Type;
        set => Type = value;
    }
    [JsonConverter(typeof(NumericEnumJsonConverter<BannerSize>))]
    public BannerSize Size { get; set; }
    [JsonConverter(typeof(NumericEnumJsonConverter<BannerSize>))]
    public BannerSize BannerSize
    {
        get => Size;
        set => Size = value;
    }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int Priority { get; set; } = 0;
    public List<int>? PlacementIds { get; set; }   // required
}

public class UpdateBannerDto : CreateBannerDto
{
    public int Id { get; set; }

    // Allows clients that edit a saved BannerDto to send the existing `placements`
    // collection back instead of manually converting it to `placementIds`.
    public List<PlacementDto> Placements { get; set; } = new();
}

public class UpdateBannerStatusDto
{
    public bool IsActive { get; set; }
}
