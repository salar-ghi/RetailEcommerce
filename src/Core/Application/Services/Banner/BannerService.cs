using Application.DTOs;

namespace Application.Services;

public class BannerService : IBannerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IImageHelper _imageHelper;

    public BannerService(
        IUnitOfWork unitOfWork,
        IMapper mapper, IImageHelper imageHelper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageHelper = imageHelper;
    }

    public async Task<int> CreateAsync(CreateBannerDto dto)
    {
        if (dto.PlacementIds is null || dto.PlacementIds.Count == 0)
            throw new ArgumentException("At least one Placement is required.");

        var banner = _mapper.Map<Banner>(dto);
        const string subFolder = "images/banners";
        if (!string.IsNullOrEmpty(dto.ImageUrl))
            banner.ImageUrl = await _imageHelper.SaveBase64Image(dto.ImageUrl, subFolder, "banner");        

        var placements = await _unitOfWork.BannerPlacements.GetAllByIdsAsync(dto.PlacementIds);

        if (placements.Count != dto.PlacementIds.Count)
            throw new Exception("Some placements not found.");

        foreach (var placement in placements)
        {
            banner.BannerPlacementMaps.Add(new BannerPlacementMap
            {
                PlacementId = placement.Id,
            });
        }
        await _unitOfWork.Banners.AddAsync(banner);
        await _unitOfWork.SaveChangesAsync();
        return banner.Id;
    }

    public async Task DeleteAsync(int id)
    {
        var banner = await _unitOfWork.Banners.GetByIdAsync(id);
        if (banner == null)
            throw new KeyNotFoundException($"Banner with id {id} was not found.");

        // Soft delete
        banner.IsDeleted = true;
        await _unitOfWork.Banners.UpdateAsync(banner);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<BannerDto>> GetAllAsync()
    {
        var banners = await _unitOfWork.Banners.GetAllWithPlacementsAsync();
        var result = _mapper.Map<List<BannerDto>>(banners);

        var tasks = result
            .Where(b => !string.IsNullOrEmpty(b.ImageUrl))
            .Select(async banner =>
            {
                banner.ImageUrl = await _imageHelper.GetImageBase64(banner.ImageUrl);
            });
        await Task.WhenAll(tasks);

        return result;
    }

    public async Task<BannerDto> GetByIdAsync(int id)
    {
        var banner = await _unitOfWork.Banners.GetByIdWithPlacesAsync(id);
        if (banner == null)
            throw new KeyNotFoundException($"Banner with id {id} was not found.");
        return _mapper.Map<BannerDto>(banner);
    }

    public async Task<IEnumerable<BannerDto>> GetActiveAsync()
    {
        var banners = await _unitOfWork.Banners.GetAllActiveAsync();
        return _mapper.Map<IEnumerable<BannerDto>>(banners);
    }

    public async Task<IEnumerable<BannerDto>> GetByPlacementAsync(BannerPageCode placementKey)
    {
        var banners = await _unitOfWork.Banners
            .GetActiveBannersByPlacementAsync(placementKey);
        return _mapper.Map<IEnumerable<BannerDto>>(banners);
    }

    public async Task<BannerDto> UpdateStatusAsync(int id, bool isActive)
    {
        var banner = await _unitOfWork.Banners.GetByIdWithPlacesAsync(id);
        if (banner == null)
            throw new KeyNotFoundException($"Banner with id {id} was not found.");

        banner.IsActive = isActive;
        banner.ModifiedTime = DateTime.UtcNow;

        await _unitOfWork.Banners.UpdateAsync(banner);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<BannerDto>(banner);
    }

    public async Task UpdateAsync(UpdateBannerDto dto)
    {
        var banner = await _unitOfWork.Banners.GetByIdWithPlacesAsync(dto.Id);
        if (banner == null)
            throw new KeyNotFoundException($"Banner with id {dto.Id} was not found.");

        if (dto.PlacementIds is null || dto.PlacementIds.Count == 0)
            throw new ArgumentException("At least one Placement is required.");
        var oldImage = banner.ImageUrl;

        _mapper.Map(dto, banner);

        const string subFolder = "images/banners";
        if (!string.IsNullOrEmpty(dto.ImageUrl) &&
            dto.ImageUrl.StartsWith("data:image"))
        {
            banner.ImageUrl = await _imageHelper.SaveBase64Image(dto.ImageUrl, subFolder, "banner");
        }
        else
        {
            banner.ImageUrl = oldImage;
        }

        await UpdateBannerPlacementMapsAsync(banner, dto.PlacementIds);

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task UpdateBannerPlacementMapsAsync(Banner banner, IEnumerable<int> placementIds)
    {
        var requestedPlacementIds = placementIds.Distinct().ToList();
        if (requestedPlacementIds.Count == 0)
            throw new ArgumentException("At least one Placement is required.");

        var placements = await _unitOfWork.BannerPlacements.GetAllByIdsAsync(requestedPlacementIds);
        if (placements.Count != requestedPlacementIds.Count)
            throw new Exception("Some placements not found.");

        var requestedPlacementIdSet = requestedPlacementIds.ToHashSet();
        var existingPlacementIdSet = banner.BannerPlacementMaps
            .Select(map => map.PlacementId)
            .ToHashSet();

        var mapsToRemove = banner.BannerPlacementMaps
            .Where(map => !requestedPlacementIdSet.Contains(map.PlacementId))
            .ToList();

        foreach (var map in mapsToRemove)
        {
            banner.BannerPlacementMaps.Remove(map);
            await _unitOfWork.BannerPlacementMaps.DeleteAsync(map);
        }

        var placementIdsToAdd = requestedPlacementIds
            .Where(placementId => !existingPlacementIdSet.Contains(placementId));

        foreach (var placementId in placementIdsToAdd)
        {
            banner.BannerPlacementMaps.Add(new BannerPlacementMap
            {
                BannerId = banner.Id,
                PlacementId = placementId,
            });
        }
    }
}
