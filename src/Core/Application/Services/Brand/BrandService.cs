using Application.DTOs;
using Application.Helper;

namespace Application.Services;

public class BrandService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IImageHelper _imageHelper;

    public BrandService(IUnitOfWork unitOfWork, IMapper mapper, IImageHelper imageHelper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageHelper = imageHelper;
    }

    public async Task<IEnumerable<BrandDto>> GetAllBrandsAsync()
    {
        var brands = await _unitOfWork.Brands.GetAllWithCategoryAsync();
        var brandDtos = _mapper.Map<List<BrandDto>>(brands);

        foreach (var dto in brandDtos)
        {
            if (string.IsNullOrEmpty(dto.Logo))
                continue;

            dto.Logo = await _imageHelper.GetImageBase64(dto.Logo);
        }

        return brandDtos;
    }

    public async Task<BrandDto> GetBrandByIdAsync(int id)
    {
        var brand = await _unitOfWork.Brands.GetByIdAsync(id);
        if (brand == null) throw new KeyNotFoundException($"Brand with ID {id} not found.");
        return _mapper.Map<BrandDto>(brand);
    }

    public async Task AddBrandAsync(BrandDto brandDto)
    {
        if (brandDto.Logo != null)
        {
            const string subFolder = "images/brands";
            if (!string.IsNullOrWhiteSpace(brandDto.Logo))
            {
                brandDto.Logo = await _imageHelper.SaveBase64Image(brandDto.Logo, subFolder, "brand");
            }
        }
        var brand = _mapper.Map<Brand>(brandDto);
        var categoryIds = brandDto.CategoryIds.Count > 0
            ? brandDto.CategoryIds
            : brandDto.Categories.Select(category => category.Id);

        foreach (var categoryId in categoryIds.Distinct())
        {
            brand.BrandCategories.Add(new BrandCategory
            {
                CategoryId = categoryId
            });
        }
        await _unitOfWork.Brands.AddAsync(brand);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateBrandAsync(BrandUpdateDto brandDto)
    {
        var brand = await _unitOfWork.Brands.GetByIdAsync(brandDto.Id);
        if (brand == null) throw new KeyNotFoundException($"Brand with ID {brandDto.Id} not found.");

        const string subFolder = "images/brands";
        if (!string.IsNullOrWhiteSpace(brandDto.Logo) &&
            brandDto.Logo.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
        {
            brandDto.Logo = await _imageHelper.SaveBase64Image(brandDto.Logo, subFolder, "brand");
        }
        else if (string.IsNullOrWhiteSpace(brandDto.Logo))
        {
            brandDto.Logo = brand.ImageUrl;
        }

        _mapper.Map(brandDto, brand);
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
