namespace Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public InventoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<InventorySummaryDto> GetSummaryAsync()
    {
        var stocks = await _unitOfWork.ProductStocks.GetAllAsync();
        var spaces = await _unitOfWork.StorageSpaces.GetAllAsync(space => !space.IsDeleted);
        var zones = await _unitOfWork.StorageZones.GetAllAsync(zone => !zone.IsDeleted && !zone.Space.IsDeleted);
        var shelves = await _unitOfWork.Shelves.GetAllAsync(shelf => !shelf.IsDeleted && !shelf.Space.IsDeleted && (shelf.Zone == null || !shelf.Zone.IsDeleted));

        return new InventorySummaryDto
        {
            TotalQuantity = stocks.Sum(s => s.Quantity),
            TotalReserved = stocks.Sum(s => s.ReservedQuantity),
            TotalAvailable = stocks.Sum(s => s.AvailableQuantity),
            LowStockCount = stocks.Count(s => s.AvailableQuantity <= s.ReorderThreshold),
            SpaceCount = spaces.Count(),
            ZoneCount = zones.Count(),
            ShelfCount = shelves.Count()
        };
    }

    public async Task<IEnumerable<StorageSpaceDto>> GetSpacesAsync()
    {
        var spaces = await _unitOfWork.StorageSpaces.GetAllAsync(q => q
            .Where(s => !s.IsDeleted)
            .Include(s => s.Zones.Where(z => !z.IsDeleted))
            .Include(s => s.Shelves.Where(sh => !sh.IsDeleted && (sh.Zone == null || !sh.Zone.IsDeleted))));
        return spaces.Select(MapSpace);
    }

    public async Task<StorageSpaceDto> GetSpaceByIdAsync(int id)
    {
        var space = await _unitOfWork.StorageSpaces
            .GetAll(space => space.Id == id && !space.IsDeleted)
            .Include(s => s.Zones.Where(z => !z.IsDeleted))
            .Include(s => s.Shelves.Where(sh => !sh.IsDeleted && (sh.Zone == null || !sh.Zone.IsDeleted)))
            .FirstOrDefaultAsync();
        return space == null ? throw new KeyNotFoundException($"Storage space with ID {id} not found.") : MapSpace(space);
    }

    public async Task<StorageSpaceDto> CreateSpaceAsync(CreateStorageSpaceDto dto)
    {
        var space = new StorageSpace
        {
            Name = dto.Name,
            Type = ParseSpaceType(dto.Type),
            Code = dto.Code,
            Address = dto.Address,
            Description = dto.Description,
            Capacity = dto.Capacity,
            Used = 0,
            IsActive = true
        };
        await _unitOfWork.StorageSpaces.AddAsync(space);
        await _unitOfWork.SaveChangesAsync();
        return MapSpace(space);
    }

    public async Task UpdateSpaceAsync(int id, CreateStorageSpaceDto dto)
    {
        var space = await _unitOfWork.StorageSpaces.GetByIdAsync(id);
        if (space == null || space.IsDeleted) throw new KeyNotFoundException($"Storage space with ID {id} not found.");
        space.Name = dto.Name;
        space.Type = ParseSpaceType(dto.Type);
        space.Code = dto.Code;
        space.Address = dto.Address;
        space.Description = dto.Description;
        space.Capacity = dto.Capacity;
        await _unitOfWork.StorageSpaces.UpdateAsync(space);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteSpaceAsync(int id)
    {
        var space = await _unitOfWork.StorageSpaces.GetByIdAsync(id);
        if (space == null) return;
        await _unitOfWork.StorageSpaces.DeleteAsync(space);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<StorageZoneDto>> GetZonesAsync(int? spaceId = null)
    {
        var zones = spaceId.HasValue
            ? await _unitOfWork.StorageZones.GetBySpaceIdAsync(spaceId.Value)
            : await _unitOfWork.StorageZones.GetAllAsync(q => q
                .Where(z => !z.IsDeleted && !z.Space.IsDeleted)
                .Include(z => z.Space)
                .Include(z => z.Shelves.Where(s => !s.IsDeleted)));
        return zones.Select(MapZone);
    }

    public async Task<StorageZoneDto> CreateZoneAsync(CreateStorageZoneDto dto)
    {
        await EnsureSpaceExists(dto.SpaceId);
        var zone = new StorageZone
        {
            SpaceId = dto.SpaceId,
            Name = dto.Name,
            Code = dto.Code,
            Description = dto.Description,
            IsActive = true
        };
        await _unitOfWork.StorageZones.AddAsync(zone);
        await _unitOfWork.SaveChangesAsync();
        return MapZone(zone);
    }

    public async Task UpdateZoneAsync(int id, CreateStorageZoneDto dto)
    {
        await EnsureSpaceExists(dto.SpaceId);
        var zone = await _unitOfWork.StorageZones.GetByIdAsync(id);
        if (zone == null || zone.IsDeleted) throw new KeyNotFoundException($"Storage zone with ID {id} not found.");
        zone.SpaceId = dto.SpaceId;
        zone.Name = dto.Name;
        zone.Code = dto.Code;
        zone.Description = dto.Description;
        await _unitOfWork.StorageZones.UpdateAsync(zone);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteZoneAsync(int id)
    {
        var zone = await _unitOfWork.StorageZones.GetByIdAsync(id);
        if (zone == null) return;
        await _unitOfWork.StorageZones.DeleteAsync(zone);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<ShelfDto>> GetShelvesAsync(int? spaceId = null, int? zoneId = null)
    {
        IEnumerable<Shelf> shelves;
        if (zoneId.HasValue)
            shelves = await _unitOfWork.Shelves.GetByZoneIdAsync(zoneId.Value);
        else if (spaceId.HasValue)
            shelves = await _unitOfWork.Shelves.GetBySpaceIdAsync(spaceId.Value);
        else
            shelves = await _unitOfWork.Shelves.GetAllAsync(q => q
                .Where(s => !s.IsDeleted && !s.Space.IsDeleted && (s.Zone == null || !s.Zone.IsDeleted))
                .Include(s => s.Space)
                .Include(s => s.Zone));

        return shelves.Select(MapShelf);
    }

    public async Task<ShelfDto> CreateShelfAsync(CreateShelfDto dto)
    {
        await ValidateShelfLocation(dto.SpaceId, dto.ZoneId);
        var shelf = new Shelf
        {
            SpaceId = dto.SpaceId,
            ZoneId = dto.ZoneId,
            Code = dto.Code,
            Name = dto.Name,
            Level = dto.Level,
            Row = dto.Row,
            Column = dto.Column,
            Capacity = dto.Capacity,
            Used = 0,
            IsActive = true
        };
        await _unitOfWork.Shelves.AddAsync(shelf);
        await _unitOfWork.SaveChangesAsync();
        return MapShelf(shelf);
    }

    public async Task UpdateShelfAsync(int id, CreateShelfDto dto)
    {
        await ValidateShelfLocation(dto.SpaceId, dto.ZoneId);
        var shelf = await _unitOfWork.Shelves.GetByIdAsync(id);
        if (shelf == null || shelf.IsDeleted) throw new KeyNotFoundException($"Shelf with ID {id} not found.");
        shelf.SpaceId = dto.SpaceId;
        shelf.ZoneId = dto.ZoneId;
        shelf.Code = dto.Code;
        shelf.Name = dto.Name;
        shelf.Level = dto.Level;
        shelf.Row = dto.Row;
        shelf.Column = dto.Column;
        shelf.Capacity = dto.Capacity;
        await _unitOfWork.Shelves.UpdateAsync(shelf);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteShelfAsync(int id)
    {
        var shelf = await _unitOfWork.Shelves.GetByIdAsync(id);
        if (shelf == null) return;
        await _unitOfWork.Shelves.DeleteAsync(shelf);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<InventoryStockDto>> GetStockAsync(long? productId = null, int? spaceId = null, int? zoneId = null, int? shelfId = null)
    {
        IEnumerable<ProductStock> stocks = productId.HasValue
            ? await _unitOfWork.ProductStocks.GetByProductLocationsAsync(productId.Value)
            : await _unitOfWork.ProductStocks.GetByStorageScopeAsync(spaceId, zoneId, shelfId);

        if (productId.HasValue && (spaceId.HasValue || zoneId.HasValue || shelfId.HasValue))
        {
            stocks = stocks
                .Where(s => !spaceId.HasValue || s.SpaceId == spaceId.Value)
                .Where(s => !zoneId.HasValue || s.ZoneId == zoneId.Value)
                .Where(s => !shelfId.HasValue || s.ShelfId == shelfId.Value);
        }

        return stocks.Select(MapStock);
    }

    public async Task<InventoryStockDto> RegisterInputAsync(InventoryInputDto dto)
    {
        await ValidateStockLocation(dto.SpaceId, dto.ZoneId, dto.ShelfId);
        var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
        if (product == null) throw new KeyNotFoundException($"Product with ID {dto.ProductId} not found.");

        Shelf shelf = null;
        if (dto.ShelfId.HasValue)
            shelf = await _unitOfWork.Shelves.GetByIdAsync(dto.ShelfId.Value);

        var resolvedSpaceId = dto.SpaceId ?? shelf?.SpaceId;
        var resolvedZoneId = dto.ZoneId ?? shelf?.ZoneId;

        var stock = new ProductStock
        {
            ProductId = dto.ProductId,
            ProductInventoryBatchId = dto.ProductInventoryBatchId,
            ProductVariantOptionId = dto.ProductVariantOptionId,
            Sku = dto.Sku,
            Quantity = dto.Quantity,
            ReservedQuantity = 0,
            ReorderThreshold = dto.ReorderThreshold,
            MinimumStockLevel = dto.MinimumStockLevel,
            SpaceId = resolvedSpaceId,
            ZoneId = resolvedZoneId,
            ShelfId = dto.ShelfId,
            LocationNote = dto.LocationNote ?? dto.Notes
        };

        await _unitOfWork.ProductStocks.AddAsync(stock);
        product.SpaceId = resolvedSpaceId;
        product.ZoneId = resolvedZoneId;
        product.ShelfId = dto.ShelfId;
        product.StorageLocationNote = dto.LocationNote ?? product.StorageLocationNote;
        await _unitOfWork.Products.UpdateAsync(product);

        if (resolvedSpaceId.HasValue)
        {
            var space = await _unitOfWork.StorageSpaces.GetByIdAsync(resolvedSpaceId.Value);
            if (space != null)
            {
                space.Used += dto.Quantity;
                await _unitOfWork.StorageSpaces.UpdateAsync(space);
            }
        }

        if (shelf != null)
        {
            shelf.Used += dto.Quantity;
            await _unitOfWork.Shelves.UpdateAsync(shelf);
        }

        await _unitOfWork.SaveChangesAsync();
        return MapStock(stock);
    }

    public async Task ReserveAsync(long stockId, int quantity)
    {
        var stock = await _unitOfWork.ProductStocks.GetByIdAsync(stockId);
        if (stock == null) throw new KeyNotFoundException($"Stock with ID {stockId} not found.");
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
        if (stock.AvailableQuantity < quantity) throw new InvalidOperationException("Not enough available stock to reserve.");
        stock.ReservedQuantity += quantity;
        await _unitOfWork.ProductStocks.UpdateAsync(stock);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ReleaseReservationAsync(long stockId, int quantity)
    {
        var stock = await _unitOfWork.ProductStocks.GetByIdAsync(stockId);
        if (stock == null) throw new KeyNotFoundException($"Stock with ID {stockId} not found.");
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");
        stock.ReservedQuantity = Math.Max(0, stock.ReservedQuantity - quantity);
        await _unitOfWork.ProductStocks.UpdateAsync(stock);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task EnsureSpaceExists(int spaceId)
    {
        var space = await _unitOfWork.StorageSpaces.GetByIdAsync(spaceId);
        if (space == null || space.IsDeleted) throw new KeyNotFoundException($"Storage space with ID {spaceId} not found.");
    }

    private async Task ValidateShelfLocation(int spaceId, int? zoneId)
    {
        await EnsureSpaceExists(spaceId);
        if (!zoneId.HasValue) return;
        var zone = await _unitOfWork.StorageZones.GetByIdAsync(zoneId.Value);
        if (zone == null || zone.IsDeleted) throw new KeyNotFoundException($"Storage zone with ID {zoneId.Value} not found.");
        if (zone.SpaceId != spaceId) throw new InvalidOperationException("Selected zone does not belong to selected storage space.");
    }

    private async Task ValidateStockLocation(int? spaceId, int? zoneId, int? shelfId)
    {
        if (shelfId.HasValue)
        {
            var shelf = await _unitOfWork.Shelves.GetByIdAsync(shelfId.Value);
            if (shelf == null || shelf.IsDeleted) throw new KeyNotFoundException($"Shelf with ID {shelfId.Value} not found.");
            if (spaceId.HasValue && shelf.SpaceId != spaceId.Value) throw new InvalidOperationException("Selected shelf does not belong to selected storage space.");
            if (zoneId.HasValue && shelf.ZoneId != zoneId.Value) throw new InvalidOperationException("Selected shelf does not belong to selected zone.");
        }
        else if (zoneId.HasValue)
        {
            var zone = await _unitOfWork.StorageZones.GetByIdAsync(zoneId.Value);
            if (zone == null || zone.IsDeleted) throw new KeyNotFoundException($"Storage zone with ID {zoneId.Value} not found.");
            if (spaceId.HasValue && zone.SpaceId != spaceId.Value) throw new InvalidOperationException("Selected zone does not belong to selected storage space.");
        }
        else if (spaceId.HasValue)
        {
            await EnsureSpaceExists(spaceId.Value);
        }
    }

    private static StorageSpaceType ParseSpaceType(string type)
    {
        if (Enum.TryParse<StorageSpaceType>(type, true, out var parsed)) return parsed;
        return type switch
        {
            "store_floor" => StorageSpaceType.StoreFloor,
            "back_room" => StorageSpaceType.BackRoom,
            "dark_store" => StorageSpaceType.DarkStore,
            _ => StorageSpaceType.Other
        };
    }

    private static string FormatSpaceType(StorageSpaceType type) => type switch
    {
        StorageSpaceType.StoreFloor => "store_floor",
        StorageSpaceType.BackRoom => "back_room",
        StorageSpaceType.DarkStore => "dark_store",
        _ => type.ToString().ToLowerInvariant()
    };

    private static StorageSpaceDto MapSpace(StorageSpace space) => new()
    {
        Id = space.Id,
        Name = space.Name,
        Type = FormatSpaceType(space.Type),
        Code = space.Code,
        Address = space.Address,
        Description = space.Description,
        Capacity = space.Capacity,
        Used = space.Used,
        IsActive = space.IsActive,
        ZoneCount = space.Zones?.Count(z => !z.IsDeleted) ?? 0,
        ShelfCount = space.Shelves?.Count(shelf => !shelf.IsDeleted && (shelf.Zone == null || !shelf.Zone.IsDeleted)) ?? 0
    };

    private static StorageZoneDto MapZone(StorageZone zone) => new()
    {
        Id = zone.Id,
        SpaceId = zone.SpaceId,
        SpaceName = zone.Space?.Name,
        Name = zone.Name,
        Code = zone.Code,
        Description = zone.Description,
        IsActive = zone.IsActive,
        ShelfCount = zone.Shelves?.Count(shelf => !shelf.IsDeleted) ?? 0
    };

    private static ShelfDto MapShelf(Shelf shelf) => new()
    {
        Id = shelf.Id,
        SpaceId = shelf.SpaceId,
        SpaceName = shelf.Space?.Name,
        ZoneId = shelf.ZoneId,
        ZoneName = shelf.Zone?.Name,
        Code = shelf.Code,
        Name = shelf.Name,
        Level = shelf.Level,
        Row = shelf.Row,
        Column = shelf.Column,
        Capacity = shelf.Capacity,
        Used = shelf.Used,
        IsActive = shelf.IsActive
    };

    private static InventoryStockDto MapStock(ProductStock stock) => new()
    {
        Id = stock.Id,
        ProductId = stock.ProductId,
        ProductName = stock.Product?.Name,
        ProductInventoryBatchId = stock.ProductInventoryBatchId,
        ProductVariantOptionId = stock.ProductVariantOptionId,
        Sku = stock.Sku,
        Quantity = stock.Quantity,
        ReservedQuantity = stock.ReservedQuantity,
        AvailableQuantity = stock.AvailableQuantity,
        ReorderThreshold = stock.ReorderThreshold,
        MinimumStockLevel = stock.MinimumStockLevel,
        SpaceId = stock.SpaceId,
        SpaceName = stock.Space?.Name,
        ZoneId = stock.ZoneId,
        ZoneName = stock.Zone?.Name,
        ShelfId = stock.ShelfId,
        ShelfCode = stock.Shelf?.Code,
        ShelfName = stock.Shelf?.Name,
        LocationNote = stock.LocationNote
    };
}
