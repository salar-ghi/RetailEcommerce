namespace Application.Services;

public class BannerService : IBannerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IImageHelper _imageHelper;

    public BannerService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService, IImageHelper imageHelper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _imageHelper = imageHelper;
    }

    public Task<int> CreateAsync(CreateBannerDto dto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<BannerDto>> GetActiveAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<BannerDto>> GetAllAsync()
    {
        var banners = await _unitOfWork.Banners.GetAllAsync();
        return _mapper.Map<IEnumerable<BannerDto>>(banners);
    }

    public async Task<BannerDto> GetByIdAsync(int id)
    {
        var banner = await _unitOfWork.Banners.GetWithDetailsAsync(id);
        if (banner == null)
            throw new KeyNotFoundException($"Banner with id {id} was not found.");

        return _mapper.Map<BannerDto>(banner);
    }

    public Task<IEnumerable<BannerDto>> GetByPlacementAsync(string placementKey)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(UpdateBannerDto dto)
    {
        throw new NotImplementedException();
    }
}
