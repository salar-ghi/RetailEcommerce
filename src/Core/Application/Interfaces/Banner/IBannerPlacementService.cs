namespace Application.Interfaces;

public interface IBannerPlacementService
{
    Task<IEnumerable<BannerPlacementDto>> GetAllAsync();
    Task<BannerPlacementDto?> GetByIdAsync(int id);
    Task SyncPlacementsWithEnumAsync();
    Task<BannerPlacementDto> CreateAsync(CreateBannerPlacementDto dto);
    Task DeleteAsync(int id);
}