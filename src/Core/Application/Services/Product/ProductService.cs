namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageHelper _imageHelper;

    public ProductService(IUnitOfWork unitOfWork, 
        IMapper mapper, IImageHelper imageHelper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageHelper = imageHelper;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _unitOfWork.Products.GetAllAsync(
            include: q => q
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Suppliers).ThenInclude(ps => ps.Supplier)
                .Include(p => p.Batches)
                .Include(p => p.Attributes)
                .Include(p => p.Dimensions)
                .Include(p => p.Images)
                .Include(p => p.Tags).ThenInclude(pt => pt.Tag));
        
        var productDto = _mapper.Map<IEnumerable<ProductDto>>(products);
        foreach (var dto in productDto)
        {
            var images = new List<string>();
            foreach (var item in dto.Images)
            {
                if (string.IsNullOrEmpty(item))
                    continue;
                images.Add(await _imageHelper.GetImageBase64(item));
            }
            dto.Images = images;
        }
        
        return productDto;
    }

    public async Task<ProductDto> GetProductByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id,
        include: q => q
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Suppliers).ThenInclude(ps => ps.Supplier)
            .Include(p => p.Batches)
            .Include(p => p.Attributes)
            .Include(p => p.Dimensions)
            .Include(p => p.Images)
            .Include(p => p.Tags).ThenInclude(pt => pt.Tag));
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found.");
        return _mapper.Map<ProductDto>(product);
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

        var resolvedLocation = await ResolveStockLocationAsync(dto.Stock);
        product.SpaceId = resolvedLocation.SpaceId;
        product.ZoneId = resolvedLocation.ZoneId;
        product.ShelfId = resolvedLocation.ShelfId;

        if (dto.Dimensions != null)
        {
            product.Dimensions = new ProductDimensions
            {
                Length = dto.Dimensions.Length,
                Width = dto.Dimensions.Width,
                Height = dto.Dimensions.Height,
                Weight = dto.Dimensions.Weight,
                DimensionUnit = dto.Dimensions.DimensionUnit,
                WeightUnit = dto.Dimensions.WeightUnit
            };
        }

        if (dto.Images != null)
        {
            foreach (var image in dto.Images)
            {
                const string subFolder = "images/products";
                product.Images.Add(new ProductImage
                {
                    ImageUrl = await _imageHelper.SaveBase64Image(image, subFolder, "product"),
                    IsPrimary = string.Equals(image, dto.CoverImage, StringComparison.OrdinalIgnoreCase)
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
                    });
                }
            }
        }

        product.Suppliers.Add(new ProductSupplier
        {
            SupplierId = dto.SupplierId,
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
                    Notes = price.Notes
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
                Quantity = dto.Stock.Quantity.Value,
                SoldQuantity = 0
            });
        }

        if (dto.Attributes != null)
        {
            foreach (var attr in dto.Attributes) 
            {
                product.Attributes.Add(new ProductAttribute
                {
                    Key = attr.Key,
                    Value = attr.Value
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
                    DisplayOrder = variant.DisplayOrder ?? 0
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
                        IsAvailable = opt.IsAvailable
                    });
                }
                product.VariantDefinitions.Add(definition);
            }
        }

        AddInitialStock(product, dto.Stock, resolvedLocation);
        await ApplyStorageUsageAsync(dto.Stock?.Quantity ?? 0, resolvedLocation);

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return product;
    }


    private async Task<(int? SpaceId, int? ZoneId, int? ShelfId, Shelf? Shelf)> ResolveStockLocationAsync(StockDto? stock)
    {
        if (stock == null)
            return (null, null, null, null);

        Shelf? shelf = null;
        var resolvedSpaceId = stock.SpaceId;
        var resolvedZoneId = stock.ZoneId;

        if (stock.ShelfId.HasValue)
        {
            shelf = await _unitOfWork.Shelves.GetByIdAsync(stock.ShelfId.Value);
            if (shelf == null)
                throw new KeyNotFoundException($"Shelf with ID {stock.ShelfId.Value} not found.");

            if (resolvedSpaceId.HasValue && shelf.SpaceId != resolvedSpaceId.Value)
                throw new InvalidOperationException("Selected shelf does not belong to selected storage space.");

            if (resolvedZoneId.HasValue && shelf.ZoneId != resolvedZoneId.Value)
                throw new InvalidOperationException("Selected shelf does not belong to selected zone.");

            resolvedSpaceId ??= shelf.SpaceId;
            resolvedZoneId ??= shelf.ZoneId;
        }

        if (resolvedZoneId.HasValue)
        {
            var zone = await _unitOfWork.StorageZones.GetByIdAsync(resolvedZoneId.Value);
            if (zone == null)
                throw new KeyNotFoundException($"Storage zone with ID {resolvedZoneId.Value} not found.");

            if (resolvedSpaceId.HasValue && zone.SpaceId != resolvedSpaceId.Value)
                throw new InvalidOperationException("Selected zone does not belong to selected storage space.");

            resolvedSpaceId ??= zone.SpaceId;
        }

        if (resolvedSpaceId.HasValue)
        {
            var space = await _unitOfWork.StorageSpaces.GetByIdAsync(resolvedSpaceId.Value);
            if (space == null)
                throw new KeyNotFoundException($"Storage space with ID {resolvedSpaceId.Value} not found.");
        }

        return (resolvedSpaceId, resolvedZoneId, stock.ShelfId, shelf);
    }

    private static void AddInitialStock(Product product, StockDto? stock, (int? SpaceId, int? ZoneId, int? ShelfId, Shelf? Shelf) resolvedLocation)
    {
        if (stock?.Quantity is not > 0)
            return;

        product.Stocks.Add(new ProductStock
        {
            Quantity = stock.Quantity.Value,
            ReservedQuantity = 0,
            ReorderThreshold = stock.ReorderThreshold ?? 0,
            MinimumStockLevel = stock.ReorderThreshold ?? 0,
            WarehouseId = stock.WarehouseId,
            SpaceId = resolvedLocation.SpaceId,
            ZoneId = resolvedLocation.ZoneId,
            ShelfId = resolvedLocation.ShelfId,
            LocationNote = stock.Location
        });
    }

    private async Task ApplyStorageUsageAsync(int quantity, (int? SpaceId, int? ZoneId, int? ShelfId, Shelf? Shelf) resolvedLocation)
    {
        if (quantity <= 0)
            return;

        if (resolvedLocation.SpaceId.HasValue)
        {
            var space = await _unitOfWork.StorageSpaces.GetByIdAsync(resolvedLocation.SpaceId.Value);
            if (space != null)
            {
                space.Used += quantity;
                await _unitOfWork.StorageSpaces.UpdateAsync(space);
            }
        }

        if (resolvedLocation.Shelf != null)
        {
            resolvedLocation.Shelf.Used += quantity;
            await _unitOfWork.Shelves.UpdateAsync(resolvedLocation.Shelf);
        }
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