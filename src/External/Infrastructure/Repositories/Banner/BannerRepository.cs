using Domain.Enums;

namespace Infrastructure.Repositories;

public class BannerRepository : Repository<Banner, int>, IBannerRepository
{
    public BannerRepository(AppDbContext context) : base(context)
    {
    }

    public Task<IEnumerable<Banner>> GetActiveBannersByPlacementAsync(string placementCode)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Banner>> GetAllActiveAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Banner?> GetByIdWithPlacesAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Banner>> SearchAsync(string? name, BannerType? type)
    {
        throw new NotImplementedException();
    }
}
