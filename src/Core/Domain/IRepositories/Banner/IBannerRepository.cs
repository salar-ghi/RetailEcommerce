namespace Domain.IRepositories;

public interface IBannerRepository : IRepository<Banner, int>
{
    Task<IEnumerable<Banner>> GetActiveBannersByPlacementAsync(BannerPageCode code);
    Task<IEnumerable<Banner>> GetAllActiveAsync();
    Task<IEnumerable<Banner>> GetAllWithPlacementsAsync();
    Task<IEnumerable<Banner>> SearchAsync(string? name, BannerType? type);
    Task<Banner?> GetByIdWithPlacesAsync(int id);
    Task<Banner?> GetByIdWithAllPlacementMapsAsync(int id);
}
