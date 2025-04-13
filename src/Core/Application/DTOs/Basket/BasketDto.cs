namespace Application.DTOs;

using System.Collections.Generic;

public class BasketDto
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public List<BasketItemDto> Items { get; set; } = new List<BasketItemDto>();
    public decimal TotalPrice { get; set; }
    public int TotalItems { get; set; }
}