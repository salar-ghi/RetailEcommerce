namespace Application.DTOs;

public class ProductStockDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public int LowStockThreshold { get; set; }
    public int ProductId { get; set; }
}

public class StockDto
{
    public int? Quantity { get; set; }
    public int? ReorderThreshold { get; set; }
    public int? SpaceId { get; set; }
    public string? SpaceName { get; set; }
    public int? ZoneId { get; set; }
    public string? ZoneName { get; set; }
    public int? ShelfId { get; set; }
    public string? ShelfCode { get; set; }
    public int? WarehouseId { get; set; }                 // legacy – ignore if we use Space/Zone/Shelf
    public string? WarehouseName { get; set; }
    public string? Location { get; set; }
    public string? QuantityUnit { get; set; }
}