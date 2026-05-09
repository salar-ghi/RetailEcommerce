using Application.DTOs;
using Application.Helper;
using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IImageHelper _imageHelper;

    public ProductService(IUnitOfWork unitOfWork, 
        IMapper mapper, IImageHelper imageHelper,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageHelper = imageHelper;
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto> GetProductByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        return product != null ? _mapper.Map<ProductDto>(product) : throw new KeyNotFoundException($"Product with ID {id} not found.");
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategory(string categoryName)
    {
        var categoryId  = await _unitOfWork.Categories.GetByAsync(z => z.Name == categoryName);
        var products = await _unitOfWork
                .Products.GetProductsByCategoryAsync(categoryId.Id);
        var mapProducts = _mapper.Map<List<ProductDto>>(products);
        return mapProducts;
    }

    public async Task<Product> AddProductAsync(CreateProductRequest dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            BrandId = dto.BrandId,
            IsActive = true,
            CreatedTime = DateTime.UtcNow,
            ModifiedTime = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId,
            ModifiedBy = _currentUserService.UserId
        };

        if (Enum.TryParse<ProductStatus>(dto.Status, true, out var status))
            product.Status = status;
        else
            product.Status = ProductStatus.Active;

        if (Enum.TryParse<ProductAvailability>(dto.Availability, true, out var availability))
            product.Availability = availability;
        else
            product.Availability = ProductAvailability.Draft;

        product.StorageLocationNote = dto.Location ?? dto.Stock?.Location;

        if (dto.Stock != null)
        {
            product.SpaceId = dto.Stock.SpaceId;
            product.ZoneId = dto.Stock.ZoneId;
            product.ShelfId = dto.Stock.ShelfId;
        }

        if (dto.Dimensions != null)
        {
            product.Dimensions = new ProductDimensions
            {
                Length = dto.Dimensions.Length,
                Width = dto.Dimensions.Width,
                Height = dto.Dimensions.Height,
                Weight = dto.Dimensions.Weight,
                DimensionUnit = dto.Dimensions.DimensionUnit,
                WeightUnit = dto.Dimensions.WeightUnit,
                CreatedTime = DateTime.UtcNow,
                ModifiedTime = DateTime.UtcNow
            };
        }

        if (dto.Images != null)
        {
            foreach (var image in dto.Images)
            {
                const string subFolder = "images/products";
                product.Images.Add(new ProductImage
                {
                    ImageUrl = await _imageHelper.SaveBase64Image(image, subFolder),
                    IsPrimary = string.Equals(image, dto.CoverImage, StringComparison.OrdinalIgnoreCase),
                    CreatedTime = DateTime.Now,
                    ModifiedTime = DateTime.Now,
                });
            }
        }

        if (dto.Tags != null)
        {
            foreach (var tagStr in dto.Tags) 
            {
                if (int.TryParse(tagStr, out int tagId)) 
                {
                    product.Tags.Add(new ProductTag
                    {
                        TagId = tagId,
                        CreatedTime = DateTime.UtcNow,
                        ModifiedTime = DateTime.UtcNow,
                    });
                }
            }
        }

        product.Suppliers.Add(new ProductSupplier
        {
            SupplierId = dto.SupplierId,
            CreatedTime = DateTime.UtcNow,
            ModifiedTime= DateTime.UtcNow,
        });

        bool hasBatches = dto.Prices != null && dto.Prices.Any();
        if (hasBatches)
        {
            foreach (var price in dto.Prices)
            {
                product.Batches.Add(new ProductInventoryBatch
                {
                    BatchNumber = price.BatchNumber,
                    CostPrice = price.CostPrice,
                    SellingPrice = price.Amount,
                    Currency = price.Currency,
                    PricingTier = price.PricingTier,
                    EffectiveDate = price.EffectiveDate,
                    ExpiryDate = price.ExpiryDate,
                    Quantity = price.Quantity,
                    SoldQuantity = price.SoldQuantity ?? 0,
                    Notes = price.Notes,
                    CreatedTime = DateTime.UtcNow,
                    ModifiedTime = DateTime.UtcNow
                });
            }
        }
        else if (dto.Stock?.Quantity > 0)
        {
            // No explicit batches → create one default batch from the stock quantity
            product.Batches.Add(new ProductInventoryBatch
            {
                BatchNumber = $"BATCH-{DateTime.UtcNow:yyyyMMddHHmmss}",
                CostPrice = 0,   // can be updated later
                SellingPrice = 0,
                Currency = "IRR",
                PricingTier = "retail",
                EffectiveDate = DateTime.UtcNow,
                Quantity = dto.Stock.Quantity,
                SoldQuantity = 0,
                CreatedTime = DateTime.UtcNow,
                ModifiedTime = DateTime.UtcNow
            });
        }

        if (dto.Attributes != null)
        {
            foreach (var attr in dto.Attributes) 
            {
                product.Attributes.Add(new ProductAttribute
                {
                    Key = attr.Key,
                    Value = attr.Value,
                    CreatedTime = DateTime.UtcNow,
                    ModifiedTime = DateTime.UtcNow,
                });
            }
        }

        if (dto.Variants != null)
        {
            foreach(var variant in dto.Variants)
            {
                var definition = new ProductVariantDefinition
                {
                    Name = variant.Name,
                    Type = variant.Type,
                    Required = variant.Required ?? false,
                    DisplayOrder = variant.DisplayOrder ?? 0,
                    CreatedTime = DateTime.UtcNow,
                    ModifiedTime = DateTime.UtcNow
                };
                
                foreach (var opt in variant.Options)
                {
                    definition.Options.Add(new ProductVariantOption
                    {
                        Value = opt.Value,
                        DisplayOrder = 0,
                        OptionValue = opt.Value,
                        PriceAdjustment = opt.PriceAdjustment,
                        StockQuantity = opt.StockQuantity,
                        Sku = opt.Sku,
                        IsAvailable = opt.IsAvailable,
                        CreatedTime = DateTime.UtcNow,
                        ModifiedTime = DateTime.UtcNow,
                    });
                }
                product.VariantDefinitions.Add(definition);
            }
        }

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return product;
    }

    public async Task UpdateProductAsync(ProductDto productDto)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productDto.Id);
        if (product != null)
        {
            _mapper.Map(productDto, product);
            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException($"Product with ID {productDto.Id} not found.");
        }
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product != null)
        {
            await _unitOfWork.Products.DeleteAsync(product);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        var products = await _unitOfWork.Products.SearchProductsAsync(searchTerm);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsByNameAsync(string name, int page = 1, int pageSize = 10)
    {
        var products = await _unitOfWork.Products.SearchByNameAsync(name);
        return _mapper.Map<IEnumerable<ProductDto>>(products.Skip((page - 1) * pageSize).Take(pageSize));
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        var products = await _unitOfWork.Products.GetProductsByPriceRangeAsync(minPrice, maxPrice);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsByCategoryAsync(int categoryId)
    {
        var products = await _unitOfWork.Products.GetProductsByCategoryAsync(categoryId);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }
}