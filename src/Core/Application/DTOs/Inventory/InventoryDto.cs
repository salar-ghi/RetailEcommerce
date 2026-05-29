namespace Application.DTOs;

public class StorageSpaceDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Code { get; set; }
    public string Address { get; set; }
    public string Description { get; set; }
    public int Capacity { get; set; }
    public int Used { get; set; }
    public bool IsActive { get; set; } = true;
    public int ZoneCount { get; set; }
    public int ShelfCount { get; set; }
}

public class CreateStorageSpaceDto
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Code { get; set; }
    public string Address { get; set; }
    public string Description { get; set; }
    public int Capacity { get; set; }
}

public class StorageZoneDto
{
    public int Id { get; set; }
    public int SpaceId { get; set; }
    public string SpaceName { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int ShelfCount { get; set; }
}

public class CreateStorageZoneDto
{
    public int SpaceId { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
}

public class ShelfDto
{
    public int Id { get; set; }
    public int SpaceId { get; set; }
    public string SpaceName { get; set; }
    public int? ZoneId { get; set; }
    public string ZoneName { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int? Level { get; set; }
    public int? Row { get; set; }
    public int? Column { get; set; }
    public int Capacity { get; set; }
    public int Used { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CreateShelfDto
{
    public int SpaceId { get; set; }
    public int? ZoneId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int? Level { get; set; }
    public int? Row { get; set; }
    public int? Column { get; set; }
    public int Capacity { get; set; }
}

public class InventoryStockDto
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public string ProductName { get; set; }
    public long? ProductInventoryBatchId { get; set; }
    public int? ProductVariantOptionId { get; set; }
    public string Sku { get; set; }
    public int Quantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public int ReorderThreshold { get; set; }
    public int MinimumStockLevel { get; set; }
    public int? SpaceId { get; set; }
    public string SpaceName { get; set; }
    public int? ZoneId { get; set; }
    public string ZoneName { get; set; }
    public int? ShelfId { get; set; }
    public string ShelfCode { get; set; }
    public string ShelfName { get; set; }
    public string LocationNote { get; set; }
}

public class CreateInventoryStockDto
{
    public long ProductId { get; set; }
    public long? ProductInventoryBatchId { get; set; }
    public int? ProductVariantOptionId { get; set; }
    public string Sku { get; set; }
    public int Quantity { get; set; }
    public int ReorderThreshold { get; set; }
    public int MinimumStockLevel { get; set; }
    public int? SpaceId { get; set; }
    public int? ZoneId { get; set; }
    public int? ShelfId { get; set; }
    public string LocationNote { get; set; }
}

public class InventoryInputDto : CreateInventoryStockDto
{
    public string BatchNumber { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal? SellingPrice { get; set; }
    public string Currency { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Notes { get; set; }
}

public class InventorySummaryDto
{
    public int TotalQuantity { get; set; }
    public int TotalReserved { get; set; }
    public int TotalAvailable { get; set; }
    public int LowStockCount { get; set; }
    public int SpaceCount { get; set; }
    public int ZoneCount { get; set; }
    public int ShelfCount { get; set; }
}
