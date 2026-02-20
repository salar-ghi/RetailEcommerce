namespace Application.Services;

public class BannerPlacementService : IBannerPlacementService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BannerPlacementService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }


    public async Task SyncPlacementsWithEnumAsync()
    {
        var existingPlacements = await _unitOfWork.BannerPlacements.GetAllAsync();
        var existingCodes = existingPlacements.Select(x => x.Code).ToList();

        var enumCodes = Enum.GetValues(typeof(BannerPageCode))
            .Cast<BannerPageCode>()
            .ToList();

        foreach (var code in enumCodes)
        {
            if (!existingCodes.Contains(code))
            {
                var placement = new BannerPlacement
                {
                    Name = code.ToString().Replace("_", " "),
                    Code = code,
                    CreatedTime = DateTime.UtcNow
                };

                await _unitOfWork.BannerPlacements.AddAsync(placement);
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<BannerPlacementDto>> GetAllAsync()
    {
        var placements = await _unitOfWork.BannerPlacements.GetAllAsync();
        return _mapper.Map<IEnumerable<BannerPlacementDto>>(placements);
    }

    public async Task<BannerPlacementDto?> GetByIdAsync(int id)
    {
        var placement = await _unitOfWork.BannerPlacements.GetByIdAsync(id);
        return _mapper.Map<BannerPlacementDto>(placement);
    }

    public async Task<BannerPlacementDto> CreateAsync(CreateBannerPlacementDto dto)
    {
        var existing = await _unitOfWork.BannerPlacements.GetByCodeAsync(dto.Code);
        if (existing != null)
            throw new InvalidOperationException($"Banner placement code '{dto.Code}' already exists.");

        var placement = _mapper.Map<BannerPlacement>(dto);
        await _unitOfWork.BannerPlacements.AddAsync(placement);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<BannerPlacementDto>(placement);
    }

    public async Task DeleteAsync(int id)
    {
        var placement = await _unitOfWork.BannerPlacements.GetByIdAsync(id);
        if (placement == null)
            throw new KeyNotFoundException($"Placement with id {id} was not found.");

        // Check if used by banners before delete, or cascade
        await _unitOfWork.BannerPlacements.DeleteAsync(placement);
        await _unitOfWork.SaveChangesAsync();
    }
}