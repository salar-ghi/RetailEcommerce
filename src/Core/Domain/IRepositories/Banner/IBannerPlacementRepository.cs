namespace Domain.IRepositories;

public interface IBannerPlacementRepository : IRepository<BannerPlacement, int>
{
    Task<BannerPlacement?> GetByCodeAsync(string code);
}
