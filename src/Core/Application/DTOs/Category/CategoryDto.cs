﻿namespace Application.DTOs;

public record CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? ParentId { get; set; }
    public string Image { get; set; }
    public int ProductCount { get; set; }
    public DateTime CreatedAt { get; set; }
}


public record CategoryDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? ParentId { get; set; }
    public string Image { get; set; }
    public int ProductCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<BrandDto> Brands { get; set; }

}