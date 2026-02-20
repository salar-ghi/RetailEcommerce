namespace Application.Interfaces;

public interface IBannerService
{
    Task<IEnumerable<BannerDto>> GetAllAsync();
    Task<BannerDto> GetByIdAsync(int id);
    Task<IEnumerable<BannerDto>> GetActiveAsync();
    Task<IEnumerable<BannerDto>> GetByPlacementAsync(BannerPageCode placementKey);
    Task<int> CreateAsync(CreateBannerDto dto);
    Task UpdateAsync(UpdateBannerDto dto);
    Task DeleteAsync(int id);
}
