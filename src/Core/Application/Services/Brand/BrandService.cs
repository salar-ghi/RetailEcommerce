namespace Application.Services;

// Application/Services/BrandService.cs
public class BrandService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BrandService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BrandDto>> GetAllBrandsAsync()
    {
        var brands = await _unitOfWork.Brands.GetAllAsync();
        return _mapper.Map<IEnumerable<BrandDto>>(brands);
    }

    public async Task<BrandDto> GetBrandByIdAsync(int id)
    {
        var brand = await _unitOfWork.Brands.GetByIdAsync(id);
        if (brand == null) throw new KeyNotFoundException($"Brand with ID {id} not found.");
        return _mapper.Map<BrandDto>(brand);
    }

    public async Task AddBrandAsync(BrandDto brandDto)
    {
        var brand = _mapper.Map<Brand>(brandDto);
        brand.CreatedBy = "bdfb65f1-9024-4736-846d-df7de909f571";
        brand.ModifiedBy = "bdfb65f1-9024-4736-846d-df7de909f571";
        brand.CreatedTime = DateTime.Now;
        brand.ModifiedTime = DateTime.Now;
        await _unitOfWork.Brands.AddAsync(brand);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateBrandAsync(BrandUpdateDto brandDto)
    {
        var brand = await _unitOfWork.Brands.GetByIdAsync(brandDto.Id);
        if (brand == null) throw new KeyNotFoundException($"Brand with ID {brandDto.Id} not found.");

        _mapper.Map(brandDto, brand);
        brand.ModifiedBy = "bdfb65f1-9024-4736-846d-df7de909f571";
        await _unitOfWork.Brands.UpdateAsync(brand);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteBrandAsync(int id)
    {
        var brand = await _unitOfWork.Brands.GetByIdAsync(id);
        if (brand != null)
        {
            await _unitOfWork.Brands.DeleteAsync(brand);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<BrandDto>> SearchBrandsByNameAsync(string name)
    {
        var brands = await _unitOfWork.Brands.SearchByNameAsync(name);
        return _mapper.Map<IEnumerable<BrandDto>>(brands);
    }

    public async Task<IEnumerable<BrandDto>> SearchBrandsByDescriptionAsync(string description)
    {
        var brands = await _unitOfWork.Brands.SearchByDescriptionAsync(description);
        return _mapper.Map<IEnumerable<BrandDto>>(brands);
    }
}