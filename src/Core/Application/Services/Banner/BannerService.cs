using Application.DTOs;

namespace Application.Services;

public class BannerService : IBannerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IImageHelper _imageHelper;

    public BannerService(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        ICurrentUserService currentUserService, 
        IImageHelper imageHelper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _imageHelper = imageHelper;
    }

    public async Task<int> CreateAsync(CreateBannerDto dto)
    {
        if (dto.PlacementIds is null || dto.PlacementIds.Count == 0)
            throw new ArgumentException("At least one Placement is required.");

        var banner = _mapper.Map<Banner>(dto);
        const string subFolder = "images/banners";
        if (!string.IsNullOrEmpty(dto.ImageUrl))
            banner.ImageUrl = await _imageHelper.SaveBase64Image(dto.ImageUrl, subFolder);

        banner.CreatedBy = _currentUserService.UserId;
        banner.CreatedTime = DateTime.UtcNow;

        await _unitOfWork.Banners.AddAsync(banner);
        await _unitOfWork.SaveChangesAsync();

        var placements = await _unitOfWork.BannerPlacements.GetAllByIdsAsync(dto.PlacementIds);

        if (placements.Count != dto.PlacementIds.Count)
            throw new Exception("Some placements not found.");

        foreach (var placement in placements)
        {
            var map = new BannerPlacementMap
            {
                BannerId = banner.Id,
                PlacementId = placement.Id,
                CreatedTime = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserId
            };

            await _unitOfWork.BannerPlacementMaps.AddAsync(map);
        }
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
        banner.ModifiedBy = _currentUserService.UserId;
        banner.ModifiedTime = DateTime.UtcNow;
        await _unitOfWork.Banners.UpdateAsync(banner);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<BannerDto>> GetAllAsync()
    {
        var banners = await _unitOfWork.Banners.GetAllAsync();
        var result  =  _mapper.Map<IEnumerable<BannerDto>>(banners);

        var tasks = result
            .Where(b => !string.IsNullOrEmpty(b.ImageUrl))
            .Select(async banner =>
            {
                banner.ImageUrl = await _imageHelper.GetImageBase64(banner.ImageUrl);
            });
        await Task.WhenAll(tasks);

        //foreach (var dto in result)
        //{
        //    if (string.IsNullOrEmpty(dto.ImageUrl))
        //        continue;
        //    dto.ImageUrl = await _imageHelper.GetImageBase64(dto.ImageUrl);
        //}

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

    public async Task UpdateAsync(UpdateBannerDto dto)
    {
        var banner = await _unitOfWork.Banners.GetByIdWithPlacesAsync(dto.Id);
        if (banner == null)
            throw new KeyNotFoundException($"Banner with id {dto.Id} was not found.");

        _mapper.Map(dto, banner);

        const string subFolder = "images/banners";
        if (!string.IsNullOrEmpty(dto.ImageUrl) && dto.ImageUrl != banner.ImageUrl)
            banner.ImageUrl = await _imageHelper.SaveBase64Image(dto.ImageUrl, subFolder);

        // Update placements: Clear and re-add
        //banner.Placements.Clear();
        //foreach (var placementId in dto.PlacementIds.Distinct())
        //{
        //    var placement = await _unitOfWork.BannerPlacements.GetByIdAsync(placementId);
        //    if (placement == null)
        //        throw new KeyNotFoundException($"Placement with id {placementId} was not found.");

        //    banner.Placements.Add(placement);
        //}

        banner.ModifiedBy = _currentUserService.UserId;
        banner.ModifiedTime = DateTime.UtcNow;
        await _unitOfWork.Banners.UpdateAsync(banner);
        await _unitOfWork.SaveChangesAsync();
    }
}
