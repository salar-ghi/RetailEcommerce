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
        var requestedPlacementIds = NormalizePlacementIds(dto.PlacementIds);
        if (requestedPlacementIds.Count == 0)
            throw new ArgumentException("At least one Placement is required.");

        var banner = _mapper.Map<Banner>(dto);
        const string subFolder = "images/banners";
        if (!string.IsNullOrEmpty(dto.ImageUrl))
            banner.ImageUrl = await _imageHelper.SaveBase64Image(dto.ImageUrl, subFolder, "banner");        

        var placements = await _unitOfWork.BannerPlacements.GetAllByIdsAsync(requestedPlacementIds);

        if (placements.Count != requestedPlacementIds.Count)
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

        var dto = _mapper.Map<BannerDto>(banner);
        if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            dto.ImageUrl = await _imageHelper.GetImageBase64(dto.ImageUrl);

        return dto;
    }

    public async Task<IEnumerable<BannerDto>> GetActiveAsync()
    {
        var banners = await _unitOfWork.Banners.GetAllActiveAsync();
        return _mapper.Map<IEnumerable<BannerDto>>(banners);
    }

    public async Task<IEnumerable<BannerDto>> GetByPlacementAsync(BannerPageCode placementKey)
    {
        var bannerQuery = await _unitOfWork.Banners
            .GetActiveBannersByPlacementAsync(placementKey);
        var banners = _mapper.Map<IEnumerable<BannerDto>>(bannerQuery);
        foreach (var dto in banners)
        {
            if (string.IsNullOrEmpty(dto.ImageUrl))
                continue;
            dto.ImageUrl = await _imageHelper.GetImageBase64(dto.ImageUrl);
        }
        return banners;
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
        var banner = await _unitOfWork.Banners.GetByIdWithAllPlacementMapsAsync(dto.Id);
        if (banner == null)
            throw new KeyNotFoundException($"Banner with id {dto.Id} was not found.");

        var shouldUpdatePlacements = ShouldUpdatePlacements(dto);
        var requestedPlacementIds = shouldUpdatePlacements
            ? GetRequestedPlacementIds(dto)
            : new List<int>();

        if (shouldUpdatePlacements && requestedPlacementIds.Count == 0)
            throw new ArgumentException("At least one Placement is required.");

        var oldImage = banner.ImageUrl;

        ApplyBannerUpdates(banner, dto);
        banner.ImageUrl = await ResolveBannerImageUrlAsync(dto.ImageUrl, oldImage);

        await _unitOfWork.Banners.UpdateAsync(banner);

        if (shouldUpdatePlacements)
            await UpdateBannerPlacementMapsAsync(banner, requestedPlacementIds);

        await _unitOfWork.SaveChangesAsync();
        await DeleteReplacedBannerImageAsync(oldImage, banner.ImageUrl, banner.Id);
    }

    private static void ApplyBannerUpdates(Banner banner, UpdateBannerDto dto)
    {
        banner.Name = dto.Name;
        banner.Description = dto.Description;
        banner.AltText = dto.AltText;
        banner.Link = dto.Link;
        banner.CallToActionText = dto.CallToActionText;
        banner.Type = dto.Type;
        banner.Size = dto.Size;
        banner.StartDate = dto.StartDate;
        banner.EndDate = dto.EndDate;
        banner.IsActive = dto.IsActive;
        banner.Priority = dto.Priority;
        banner.ModifiedTime = DateTime.UtcNow;
    }

    private async Task<string> ResolveBannerImageUrlAsync(string? requestedImageUrl, string? oldImageUrl)
    {
        const string subFolder = "images/banners";

        if (!string.IsNullOrWhiteSpace(requestedImageUrl) &&
            requestedImageUrl.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
        {
            return await _imageHelper.SaveBase64ImageIfChanged(
                requestedImageUrl,
                oldImageUrl,
                subFolder,
                "banner");
        }

        return oldImageUrl ?? string.Empty;
    }

    private async Task DeleteReplacedBannerImageAsync(string? oldImageUrl, string? newImageUrl, int bannerId)
    {
        if (string.IsNullOrWhiteSpace(oldImageUrl) ||
            string.Equals(oldImageUrl, newImageUrl, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (await _unitOfWork.Banners.IsImageUrlUsedByAnotherBannerAsync(oldImageUrl, bannerId))
            return;

        await _imageHelper.DeleteImageAsync(oldImageUrl);
    }

    private static List<int> NormalizePlacementIds(IEnumerable<int>? placementIds)
    {
        return placementIds?
            .Where(placementId => placementId > 0)
            .Distinct()
            .ToList() ?? new List<int>();
    }

    private static bool ShouldUpdatePlacements(UpdateBannerDto dto)
    {
        return dto.PlacementIds is not null || dto.Placements is { Count: > 0 };
    }

    private static List<int> GetRequestedPlacementIds(UpdateBannerDto dto)
    {
        if (dto.PlacementIds is not null)
            return NormalizePlacementIds(dto.PlacementIds);

        return dto.Placements?
            .Select(placement => placement.Id)
            .Where(placementId => placementId > 0)
            .Distinct()
            .ToList() ?? new List<int>();
    }

    private async Task UpdateBannerPlacementMapsAsync(Banner banner, IEnumerable<int> placementIds)
    {
        var requestedPlacementIds = NormalizePlacementIds(placementIds);
        if (requestedPlacementIds.Count == 0)
            throw new ArgumentException("At least one Placement is required.");

        var placements = await _unitOfWork.BannerPlacements.GetAllByIdsAsync(requestedPlacementIds);
        if (placements.Count != requestedPlacementIds.Count)
            throw new Exception("Some placements not found.");

        var requestedPlacementIdSet = requestedPlacementIds.ToHashSet();
        var existingActivePlacementIdSet = banner.BannerPlacementMaps
            .Where(map => !map.IsDeleted)
            .Select(map => map.PlacementId)
            .ToHashSet();

        var mapsToRemove = banner.BannerPlacementMaps
            .Where(map => !map.IsDeleted && !requestedPlacementIdSet.Contains(map.PlacementId))
            .ToList();

        foreach (var map in mapsToRemove)
        {
            await _unitOfWork.BannerPlacementMaps.DeleteAsync(map);
        }

        var placementIdsToAdd = requestedPlacementIds
            .Where(placementId => !existingActivePlacementIdSet.Contains(placementId));

        foreach (var placementId in placementIdsToAdd)
        {
            var deletedMap = banner.BannerPlacementMaps
                .FirstOrDefault(map => map.IsDeleted && map.PlacementId == placementId);

            if (deletedMap is not null)
            {
                deletedMap.IsDeleted = false;
                await _unitOfWork.BannerPlacementMaps.UpdateAsync(deletedMap);
                continue;
            }

            var map = new BannerPlacementMap
            {
                BannerId = banner.Id,
                PlacementId = placementId,
            };

            await _unitOfWork.BannerPlacementMaps.AddAsync(map);
        }
    }
}
