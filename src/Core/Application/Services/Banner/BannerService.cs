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

        foreach (var placementId in dto.PlacementIds.Distinct())
        {
            var placement = await _unitOfWork.BannerPlacements.GetByIdAsync(placementId);
            if (placement == null)
                throw new KeyNotFoundException($"Placement with id {placementId} was not found.");

            banner.Placements.Add(placement);
        }

        banner.CreatedBy = _currentUserService.UserId; // Or username
        banner.CreatedTime = DateTime.UtcNow;

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
        banner.ModifiedBy = _currentUserService.UserId;
        banner.ModifiedTime = DateTime.UtcNow;
        await _unitOfWork.Banners.UpdateAsync(banner);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<BannerDto>> GetAllAsync()
    {
        var banners = await _unitOfWork.Banners.GetAllAsync();
        return _mapper.Map<IEnumerable<BannerDto>>(banners);
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

    public async Task<IEnumerable<BannerDto>> GetByPlacementAsync(string placementKey)
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
        banner.Placements.Clear();
        foreach (var placementId in dto.PlacementIds.Distinct())
        {
            var placement = await _unitOfWork.BannerPlacements.GetByIdAsync(placementId);
            if (placement == null)
                throw new KeyNotFoundException($"Placement with id {placementId} was not found.");

            banner.Placements.Add(placement);
        }

        banner.ModifiedBy = _currentUserService.UserId;
        banner.ModifiedTime = DateTime.UtcNow;
        await _unitOfWork.Banners.UpdateAsync(banner);
        await _unitOfWork.SaveChangesAsync();
    }
}
