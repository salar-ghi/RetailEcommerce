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
        var brand = await GetBrandWithCategoriesAsync(id, asNoTracking: true);
        if (brand == null) throw new KeyNotFoundException($"Brand with ID {id} not found.");
        return _mapper.Map<BrandDto>(brand);
    }

    public async Task AddBrandAsync(BrandDto brandDto)
    {
        brandDto.Name = NormalizeName(brandDto.Name, nameof(brandDto.Name));

        if (await _unitOfWork.Brands.ExistsByNameAsync(brandDto.Name.ToLower()))
        {
            throw new ArgumentException($"Brand with name '{brandDto.Name}' already exists.");
        }

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

    public async Task<BrandDto> UpdateBrandAsync(BrandUpdateDto brandDto)
    {
        var brand = await GetBrandWithCategoriesAsync(brandDto.Id);
        if (brand == null) throw new KeyNotFoundException($"Brand with ID {brandDto.Id} not found.");

        brandDto.Name = NormalizeName(brandDto.Name, nameof(brandDto.Name));

        if (await _unitOfWork.Brands.ExistsByNameAsync(brandDto.Name.ToLower(), brandDto.Id))
        {
            throw new ArgumentException($"Brand with name '{brandDto.Name}' already exists.");
        }

        const string subFolder = "images/brands";
        if (!string.IsNullOrWhiteSpace(brandDto.Logo) &&
            brandDto.Logo.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
        {
            brandDto.Logo = await _imageHelper.SaveBase64ImageIfChanged(
                brandDto.Logo,
                brand.ImageUrl,
                subFolder,
                "brand");
        }
        else if (string.IsNullOrWhiteSpace(brandDto.Logo))
        {
            brandDto.Logo = brand.ImageUrl;
        }

        _mapper.Map(brandDto, brand);
        UpdateBrandCategories(brand, brandDto);
        await _unitOfWork.SaveChangesAsync();

        var updatedBrand = await GetBrandWithCategoriesAsync(brandDto.Id, asNoTracking: true)
            ?? throw new KeyNotFoundException($"Brand with ID {brandDto.Id} not found.");

        return _mapper.Map<BrandDto>(updatedBrand);
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

    private static void UpdateBrandCategories(Brand brand, BrandUpdateDto brandDto)
    {
        if (brandDto.CategoryIds == null && brandDto.Categories == null)
        {
            return;
        }

        var requestedCategoryIds = brandDto.CategoryIds is not null
            ? brandDto.CategoryIds
            : brandDto.Categories?.Select(category => category.Id) ?? [];
        var requestedCategoryIdSet = requestedCategoryIds
            .Distinct()
            .ToHashSet();

        var categoriesToRemove = brand.BrandCategories
            .Where(brandCategory => !requestedCategoryIdSet.Contains(brandCategory.CategoryId))
            .ToList();

        foreach (var categoryToRemove in categoriesToRemove)
        {
            brand.BrandCategories.Remove(categoryToRemove);
        }

        var existingCategoryIds = brand.BrandCategories
            .Select(brandCategory => brandCategory.CategoryId)
            .ToHashSet();

        foreach (var categoryId in requestedCategoryIdSet.Where(categoryId => !existingCategoryIds.Contains(categoryId)))
        {
            brand.BrandCategories.Add(new BrandCategory
            {
                BrandId = brand.Id,
                CategoryId = categoryId
            });
        }
    }

    private async Task<Brand?> GetBrandWithCategoriesAsync(int id, bool asNoTracking = false)
    {
        return await _unitOfWork.Brands.GetByIdWithCategoryAsync(id, asNoTracking);
    }

    private static string NormalizeName(string name, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Brand name is required.", parameterName);
        }

        return name.Trim();
    }
}
