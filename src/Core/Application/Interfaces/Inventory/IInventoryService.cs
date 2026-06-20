namespace Application.Interfaces;

public interface IInventoryService
{
    Task<InventorySummaryDto> GetSummaryAsync();
    Task<IEnumerable<StorageSpaceDto>> GetSpacesAsync();
    Task<StorageSpaceDto> GetSpaceByIdAsync(int id);
    Task<StorageSpaceDto> CreateSpaceAsync(CreateStorageSpaceDto dto);
    Task UpdateSpaceAsync(int id, CreateStorageSpaceDto dto);
    Task DeleteSpaceAsync(int id);
    Task<IEnumerable<StorageZoneDto>> GetZonesAsync(int? spaceId = null);
    Task<StorageZoneDto> CreateZoneAsync(CreateStorageZoneDto dto);
    Task UpdateZoneAsync(int id, CreateStorageZoneDto dto);
    Task DeleteZoneAsync(int id);
    Task<IEnumerable<ShelfDto>> GetShelvesAsync(int? spaceId = null, int? zoneId = null);
    Task<ShelfDto> CreateShelfAsync(CreateShelfDto dto);
    Task UpdateShelfAsync(int id, CreateShelfDto dto);
    Task DeleteShelfAsync(int id);
    Task<IEnumerable<InventoryStockDto>> GetStockAsync(long? productId = null, int? spaceId = null, int? zoneId = null, int? shelfId = null);
    Task<StockInputDto> RegisterInputAsync(InventoryInputDto dto);
    Task<IEnumerable<StockInputDto>> GetInputsAsync(long? productId = null, int? supplierId = null, DateTime? from = null, DateTime? to = null);
    Task<IEnumerable<StockInputDto>> GetRecentInputsAsync(int limit = 10);
    Task<IEnumerable<StockInputDto>> GetExpiringInputsAsync(int days = 30);
    Task ReserveAsync(long stockId, int quantity);
    Task ReleaseReservationAsync(long stockId, int quantity);
}
