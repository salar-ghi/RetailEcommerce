namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageHelper _imageHelper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IImageHelper imageHelper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageHelper = imageHelper;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await BuildProductSummaryQuery()
            .OrderByDescending(product => product.Id)
            .ToListAsync();

        return await ConvertProductsImagesToBase64Async(_mapper.Map<IEnumerable<ProductDto>>(products));
    }

    public async Task<ProductDto> GetProductByIdAsync(int id)
    {
        var product = await BuildProductSummaryQuery()
            .FirstOrDefaultAsync(product => product.Id == id);

        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found.");

        var productDto = _mapper.Map<ProductDto>(product);
        await ConvertProductImagesToBase64Async(productDto, includeCoverImage: false);

        return productDto;
    }

    public async Task<ProductDto> GetProductDetailByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.GetProductDetailsByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found.");

        var productDto = _mapper.Map<ProductDto>(product);
        productDto.Attributes = BuildDisplayAttributes(product.AttributeValues);
        productDto.AttributeValues = new List<ProductAttributeValueDto>();
        await ConvertProductImagesToBase64Async(productDto, includeCoverImage: false);

        return productDto;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategory(string categoryName)
    {
        var categoryIds = await GetCategoryAndChildIdsByNameAsync(categoryName);
        if (!categoryIds.Any())
            return Enumerable.Empty<ProductDto>();

        var products = await BuildProductSummaryQuery()
            .Where(product => categoryIds.Contains(product.CategoryId))
            .OrderByDescending(product => product.Id)
            .Take(10)
            .ToListAsync();

        return await ConvertProductsImagesToBase64Async(_mapper.Map<List<ProductDto>>(products));
    }

    public async Task<IEnumerable<ProductDto>> GetDiscountedProductsAsync(int take = 10)
    {
        var products = await BuildProductSummaryQuery()
            .Where(product => product.Batches.Any(batch => batch.CostPrice > batch.SellingPrice && batch.SellingPrice > 0))
            .OrderByDescending(product => product.Batches
                .Where(batch => batch.CostPrice > batch.SellingPrice && batch.SellingPrice > 0)
                .Max(batch => (batch.CostPrice - batch.SellingPrice) / batch.CostPrice))
            .Take(take)
            .ToListAsync();

        return await ConvertProductsImagesToBase64Async(_mapper.Map<List<ProductDto>>(products));
    }

    public async Task<ProductDto> AddProductAsync(CreateProductRequest dto)
    {
        var product = new Product();
        var resolvedLocation = await ResolveStockLocationAsync(dto.Stock);

        ApplyProductScalars(product, dto, resolvedLocation);
        ApplyDimensions(product, dto.Dimensions);
        await ReplaceImagesAsync(product, dto.Images, dto.CoverImage);
        ReplaceTags(product, dto.Tags);
        ReplaceSupplier(product, dto.SupplierId);
        ReplaceBatches(product, dto.Prices, dto.Stock);
        ReplaceStock(product, dto.Stock, resolvedLocation);
        ReplaceAttributes(product, dto.Attributes);
        ReplaceAttributeValues(product, dto.AttributeValues);
        ReplaceVariants(product, dto.Variants);

        await ApplyStorageUsageAsync(dto.Stock?.Quantity ?? 0, resolvedLocation);
        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductRequest dto)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, include: q => q
            .Include(p => p.Dimensions)
            .Include(p => p.Images)
            .Include(p => p.Tags)
            .Include(p => p.Suppliers)
            .Include(p => p.Batches)
            .Include(p => p.Stocks)
            .Include(p => p.Attributes)
            .Include(p => p.AttributeValues).ThenInclude(v => v.AttributeDefinition).ThenInclude(d => d.Options)
            .Include(p => p.AttributeValues).ThenInclude(v => v.AttributeOption)
            .Include(p => p.VariantDefinitions).ThenInclude(v => v.Options));

        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found.");

        var oldStockQuantity = product.Stocks.Sum(s => s.Quantity);
        var resolvedLocation = await ResolveStockLocationAsync(dto.Stock);

        ApplyProductScalars(product, dto, resolvedLocation);
        ApplyDimensions(product, dto.Dimensions);
        await ReplaceImagesAsync(product, dto.Images, dto.CoverImage);
        ReplaceTags(product, dto.Tags);
        ReplaceSupplier(product, dto.SupplierId);
        ReplaceBatches(product, dto.Prices, dto.Stock);
        ReplaceStock(product, dto.Stock, resolvedLocation);
        ReplaceAttributes(product, dto.Attributes);
        ReplaceAttributeValues(product, dto.AttributeValues);
        ReplaceVariants(product, dto.Variants);

        await ApplyStorageUsageAsync((dto.Stock?.Quantity ?? 0) - oldStockQuantity, resolvedLocation);
        await _unitOfWork.Products.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProductDto>(product);
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
        return await ConvertProductsImagesToBase64Async(_mapper.Map<IEnumerable<ProductDto>>(products));
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsByNameAsync(string name, int page = 1, int pageSize = 10)
    {
        var products = await _unitOfWork.Products.SearchByNameAsync(name);
        return await ConvertProductsImagesToBase64Async(_mapper.Map<IEnumerable<ProductDto>>(products.Skip((page - 1) * pageSize).Take(pageSize)));
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        var products = await _unitOfWork.Products.GetProductsByPriceRangeAsync(minPrice, maxPrice);
        return await ConvertProductsImagesToBase64Async(_mapper.Map<IEnumerable<ProductDto>>(products));
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsByCategoryAsync(int categoryId)
    {
        var products = await _unitOfWork.Products.GetProductsByCategoryAsync(categoryId);
        return await ConvertProductsImagesToBase64Async(_mapper.Map<IEnumerable<ProductDto>>(products));
    }

    public async Task<IEnumerable<ProductAttributeValueDto>> GetProductAttributeValuesAsync(long productId)
    {
        var values = await _unitOfWork.Products.GetAttributeValuesByProductIdAsync(productId);
        return _mapper.Map<IEnumerable<ProductAttributeValueDto>>(values);
    }

    public async Task SaveProductAttributeValuesAsync(long productId, List<ProductAttributeValueDto> values)
    {
        var product = await _unitOfWork.Products.GetProductWithAttributeValuesAsync(productId);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {productId} not found.");

        ReplaceAttributeValues(product, values);
        await _unitOfWork.Products.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();
    }

    private IQueryable<Product> BuildProductSummaryQuery() => _unitOfWork.Products.GetAll(product => !product.IsDeleted)
        .Where(product => product.IsActive)
        .Include(product => product.Category)
        .Include(product => product.Brand)
        .Include(product => product.Batches)
        .Include(product => product.Stocks)
        .Include(product => product.Images);

    private async Task<HashSet<int>> GetCategoryAndChildIdsByNameAsync(string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            return new HashSet<int>();

        var categories = await _unitOfWork.Categories.GetAll(category => !category.IsDeleted)
            .Select(category => new { category.Id, category.Name, category.ParentId })
            .ToListAsync();

        var rootCategory = categories.FirstOrDefault(category => string.Equals(
            category.Name.Trim(),
            categoryName.Trim(),
            StringComparison.OrdinalIgnoreCase));

        if (rootCategory == null)
            return new HashSet<int>();

        var categoryIds = new HashSet<int> { rootCategory.Id };
        var queue = new Queue<int>();
        queue.Enqueue(rootCategory.Id);

        while (queue.Count > 0)
        {
            var parentId = queue.Dequeue();
            foreach (var child in categories.Where(category => category.ParentId == parentId))
            {
                if (categoryIds.Add(child.Id))
                    queue.Enqueue(child.Id);
            }
        }

        return categoryIds;
    }



    private static List<AttributeDto> BuildDisplayAttributes(IEnumerable<ProductAttributeValue>? attributeValues)
    {
        return (attributeValues ?? Enumerable.Empty<ProductAttributeValue>())
            .Where(value => value.AttributeDefinition != null)
            .Select(value => new AttributeDto
            {
                Key = string.IsNullOrWhiteSpace(value.AttributeDefinition.Code)
                    ? value.AttributeDefinitionId.ToString()
                    : value.AttributeDefinition.Code,
                Name = value.AttributeDefinition.Name,
                Value = ResolveAttributeDisplayValue(value)
            })
            .Where(attribute => !string.IsNullOrWhiteSpace(attribute.Value))
            .OrderBy(attribute => attribute.Name ?? attribute.Key)
            .ThenBy(attribute => attribute.Key)
            .ToList();
    }

    private static string? ResolveAttributeDisplayValue(ProductAttributeValue value)
    {
        return value.AttributeDefinition.DataType switch
        {
            AttributeDataType.Integer => value.IntValue?.ToString(),
            AttributeDataType.Decimal => value.DecimalValue?.ToString(),
            AttributeDataType.Boolean => value.BoolValue?.ToString().ToLowerInvariant(),
            AttributeDataType.Date => value.DateValue?.ToString("yyyy-MM-dd"),
            AttributeDataType.Select => ResolveSingleOptionValue(value),
            AttributeDataType.MultiSelect => ResolveMultipleOptionValues(value),
            _ => value.StringValue
        };
    }

    private static string? ResolveSingleOptionValue(ProductAttributeValue value)
    {
        var option = value.AttributeOption
            ?? value.AttributeDefinition.Options.FirstOrDefault(option => option.Id == value.AttributeOptionId);

        return option?.Label ?? option?.Value ?? value.StringValue ?? value.AttributeOptionId?.ToString();
    }

    private static string? ResolveMultipleOptionValues(ProductAttributeValue value)
    {
        var optionIds = DeserializeAttributeOptionIds(value.AttributeOptionIds);
        if (!optionIds.Any())
            return value.StringValue;

        var labels = value.AttributeDefinition.Options
            .Where(option => optionIds.Contains(option.Id))
            .OrderBy(option => option.SortOrder)
            .ThenBy(option => option.Label ?? option.Value)
            .Select(option => option.Label ?? option.Value)
            .Where(label => !string.IsNullOrWhiteSpace(label))
            .ToList();

        return labels.Any() ? string.Join(", ", labels) : string.Join(", ", optionIds);
    }

    private static int[] DeserializeAttributeOptionIds(string? optionIds)
    {
        if (string.IsNullOrWhiteSpace(optionIds))
            return Array.Empty<int>();

        try
        {
            return JsonSerializer.Deserialize<int[]>(optionIds) ?? Array.Empty<int>();
        }
        catch (JsonException)
        {
            return Array.Empty<int>();
        }
    }

    private async Task ConvertProductImagesToBase64Async(ProductDto product, bool includeCoverImage = true)
    {
        product.Images = await ConvertStoredImagesToBase64Async(product.Images);
        product.CoverImage = includeCoverImage
            ? await ConvertStoredImageToBase64Async(product.CoverImage)
            : string.Empty;
    }

    private async Task<IEnumerable<ProductDto>> ConvertProductsImagesToBase64Async(IEnumerable<ProductDto> products)
    {
        var productList = products.ToList();

        foreach (var product in productList)
        {
            await ConvertProductImagesToBase64Async(product);
        }

        return productList;
    }

    private async Task<string> ConvertStoredImageToBase64Async(string? image)
    {
        if (string.IsNullOrWhiteSpace(image))
            return string.Empty;

        return await _imageHelper.GetImageBase64(image) ?? string.Empty;
    }

    private async Task<List<string>> ConvertStoredImagesToBase64Async(List<string>? images)
    {
        var conversionTasks = (images ?? new List<string>())
            .Where(image => !string.IsNullOrWhiteSpace(image))
            .Select(image => _imageHelper.GetImageBase64(image))
            .ToArray();

        var converted = await Task.WhenAll(conversionTasks);
        return converted.Where(image => !string.IsNullOrWhiteSpace(image)).ToList();
    }

    private static void ApplyProductScalars(Product product, CreateProductRequest dto, (int? SpaceId, int? ZoneId, int? ShelfId, Shelf? Shelf) resolvedLocation)
    {
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.CategoryId = dto.CategoryId;
        product.BrandId = dto.BrandId;
        product.IsActive = !string.Equals(dto.Status, "inactive", StringComparison.OrdinalIgnoreCase);
        product.PricingStrategy = NormalizePricingStrategy(dto.PricingStrategy);
        product.SalesUnitMode = dto.SalesUnit?.Mode;
        product.SalesUnitWeightUnit = dto.SalesUnit?.WeightUnit;
        product.SalesUnitPricePerWeightUnit = dto.SalesUnit?.PricePerWeightUnit;
        product.SalesUnitPackWeight = dto.SalesUnit?.PackWeight;
        product.SalesUnitPackLabel = dto.SalesUnit?.PackLabel;
        product.Status = Enum.TryParse<ProductStatus>(dto.Status, true, out var status) ? status : ProductStatus.Active;
        product.Availability = Enum.TryParse<ProductAvailability>(dto.Availability, true, out var availability) ? availability : ProductAvailability.Draft;
        product.StorageLocationNote = dto.Location ?? dto.Stock?.Location;
        product.SpaceId = resolvedLocation.SpaceId;
        product.ZoneId = resolvedLocation.ZoneId;
        product.ShelfId = resolvedLocation.ShelfId;
    }


    private static string NormalizePricingStrategy(string? pricingStrategy)
    {
        return pricingStrategy?.Trim().ToLowerInvariant() switch
        {
            "latest" => "latest",
            "average" => "average",
            _ => "fifo"
        };
    }

    private static void ApplyDimensions(Product product, DimensionDto? dimensions)
    {
        if (dimensions == null)
        {
            product.Dimensions = null;
            return;
        }

        product.Dimensions ??= new ProductDimensions();
        product.Dimensions.Length = dimensions.Length;
        product.Dimensions.Width = dimensions.Width;
        product.Dimensions.Height = dimensions.Height;
        product.Dimensions.Weight = dimensions.Weight;
        product.Dimensions.DimensionUnit = dimensions.DimensionUnit;
        product.Dimensions.WeightUnit = dimensions.WeightUnit;
    }

    private async Task ReplaceImagesAsync(Product product, List<string>? images, string? coverImage)
    {
        product.Images.Clear();
        if (images == null)
            return;

        foreach (var image in images.Where(i => !string.IsNullOrWhiteSpace(i)))
        {
            var imageUrl = image.StartsWith("data:", StringComparison.OrdinalIgnoreCase)
                ? await _imageHelper.SaveBase64Image(image, "images/products", "product")
                : image;

            product.Images.Add(new ProductImage
            {
                ImageUrl = imageUrl,
                IsPrimary = string.Equals(image, coverImage, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(imageUrl, coverImage, StringComparison.OrdinalIgnoreCase)
            });
        }
    }

    private static void ReplaceTags(Product product, List<string>? tags)
    {
        product.Tags.Clear();
        if (tags == null)
            return;

        foreach (var tag in tags)
        {
            if (int.TryParse(tag, out var tagId))
                product.Tags.Add(new ProductTag { TagId = tagId });
        }
    }

    private static void ReplaceSupplier(Product product, int supplierId)
    {
        product.Suppliers.Clear();
        product.Suppliers.Add(new ProductSupplier { SupplierId = supplierId });
    }

    private static void ReplaceBatches(Product product, List<PriceDto>? prices, StockDto? stock)
    {
        product.Batches.Clear();
        if (prices?.Any() == true)
        {
            foreach (var price in prices)
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
        else if (stock?.Quantity > 0)
        {
            product.Batches.Add(new ProductInventoryBatch
            {
                BatchNumber = $"BATCH-{DateTime.UtcNow:yyyyMMddHHmmss}",
                CostPrice = 0,
                SellingPrice = 0,
                Currency = "IRR",
                PricingTier = "retail",
                EffectiveDate = DateTime.UtcNow,
                Quantity = stock.Quantity.Value,
                SoldQuantity = 0
            });
        }
    }

    private static void ReplaceStock(Product product, StockDto? stock, (int? SpaceId, int? ZoneId, int? ShelfId, Shelf? Shelf) resolvedLocation)
    {
        product.Stocks.Clear();
        AddInitialStock(product, stock, resolvedLocation);
    }

    private static void ReplaceAttributes(Product product, List<AttributeDto>? attributes)
    {
        product.Attributes.Clear();
        if (attributes == null)
            return;

        foreach (var attr in attributes)
            product.Attributes.Add(new ProductAttribute { Key = attr.Key, Value = attr.Value });
    }

    private static void ReplaceAttributeValues(Product product, List<ProductAttributeValueDto>? values)
    {
        product.AttributeValues.Clear();
        if (values == null)
            return;

        foreach (var value in values.Where(v => v.AttributeDefinitionId > 0))
        {
            product.AttributeValues.Add(new ProductAttributeValue
            {
                Product = product,
                AttributeDefinitionId = value.AttributeDefinitionId,
                StringValue = value.StringValue,
                IntValue = value.IntValue,
                DecimalValue = value.DecimalValue,
                BoolValue = value.BoolValue,
                DateValue = value.DateValue,
                AttributeOptionId = value.AttributeOptionId,
                AttributeOptionIds = value.AttributeOptionIds == null ? null : System.Text.Json.JsonSerializer.Serialize(value.AttributeOptionIds)
            });
        }
    }

    private static void ReplaceVariants(Product product, List<VariantDto>? variants)
    {
        product.VariantDefinitions.Clear();
        if (variants == null)
            return;

        foreach (var variant in variants)
        {
            var definition = new ProductVariantDefinition
            {
                Name = variant.Name,
                Type = variant.Type,
                Required = variant.Required ?? false,
                DisplayOrder = variant.DisplayOrder ?? 0
            };

            foreach (var option in variant.Options ?? Enumerable.Empty<VariantOptionDto>())
            {
                definition.Options.Add(new ProductVariantOption
                {
                    DisplayValue = option.Name ?? option.Value,
                    ActualValue = option.Value,
                    DisplayOrder = 0,
                    PriceAdjustment = option.PriceAdjustment,
                    StockQuantity = option.StockQuantity,
                    Sku = option.Sku,
                    IsAvailable = option.IsAvailable
                });
            }

            product.VariantDefinitions.Add(definition);
        }
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

    private async Task ApplyStorageUsageAsync(int quantityDelta, (int? SpaceId, int? ZoneId, int? ShelfId, Shelf? Shelf) resolvedLocation)
    {
        if (quantityDelta == 0)
            return;

        if (resolvedLocation.SpaceId.HasValue)
        {
            var space = await _unitOfWork.StorageSpaces.GetByIdAsync(resolvedLocation.SpaceId.Value);
            if (space != null)
            {
                space.Used += quantityDelta;
                await _unitOfWork.StorageSpaces.UpdateAsync(space);
            }
        }

        if (resolvedLocation.Shelf != null)
        {
            resolvedLocation.Shelf.Used += quantityDelta;
            await _unitOfWork.Shelves.UpdateAsync(resolvedLocation.Shelf);
        }
    }
}
