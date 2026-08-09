namespace Infrastructure.Repositories;


public class ProductRepository : Repository<Product, long>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }


    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, int page = 1, int pageSize = 10)
    {
        return await _context.Products
            .Where(p => p.Name.Contains(searchTerm))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    //public async Task<IEnumerable<ProductDto>> SearchProductsAsync(SearchProductsDto searchDto)
    //{
    //    var query = _unitOfWork.Products.GetAllQueryable()
    //        .Include(p => p.Category)
    //        .Include(p => p.Brand)
    //        .Include(p => p.Stock)
    //        .Include(p => p.UnitPrice);

    //    if (!string.IsNullOrEmpty(searchDto.SearchTerm))
    //    {
    //        query = query.Where(p => p.Name.Contains(searchDto.SearchTerm) || p.Description.Contains(searchDto.SearchTerm));
    //    }
    //    if (searchDto.CategoryId.HasValue)
    //    {
    //        query = query.Where(p => p.CategoryId == searchDto.CategoryId.Value);
    //    }
    //    if (searchDto.BrandId.HasValue)
    //    {
    //        query = query.Where(p => p.BrandId == searchDto.BrandId.Value);
    //    }
    //    if (searchDto.SupplierId.HasValue)
    //    {
    //        query = query.Where(p => p.ProductSuppliers.Any(ps => ps.SupplierId == searchDto.SupplierId.Value));
    //    }
    //    if (searchDto.MinPrice.HasValue)
    //    {
    //        query = query.Where(p => p.UnitPrice.Price >= searchDto.MinPrice.Value);
    //    }
    //    if (searchDto.MaxPrice.HasValue)
    //    {
    //        query = query.Where(p => p.UnitPrice.Price <= searchDto.MaxPrice.Value);
    //    }
    //    if (searchDto.InStock.HasValue && searchDto.InStock.Value)
    //    {
    //        query = query.Where(p => p.Stock.Quantity > 0);
    //    }

    //    var products = await query.ToListAsync();
    //    return _mapper.Map<IEnumerable<ProductDto>>(products);
    //}


    //public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    //{
    //    return await _context.Products
    //        .Where(p => p.Name.Contains(searchTerm))
    //        .AsNoTracking()
    //        .ToListAsync();
    //}

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _context.Products
            .Where(p => p.CategoryId == categoryId && p.IsActive == true)
            .AsNoTracking()
            .ToListAsync();
    }
    
    public async Task<List<Product>> GetMostSellingProductsInCategoryAsync(string categoryName, int count = 10)
    {
        return await _context.Products
            .Where(p => p.Category.Name == categoryName && p.IsActive)
            .Select(p => new
            {
                Product = p,
                TotalSold = p.OrderItems.Sum(oi => oi.Quantity)
            })
            .OrderByDescending(x => x.TotalSold)
            .Take(count)
            .Select(x => x.Product)
            .Include(p => p.Category)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId)
    {
        return await _context.Products
            .Where(p => p.BrandId == brandId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsBySupplierAsync(int supplierId)
    {
        return await _context.Products
            .Where(p => p.Suppliers.Any(z => z.SupplierId == supplierId))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByTagAsync(int tagId)
    {
        return await _context.Products
            .Where(p => p.Tags.Any(t => t.Id == tagId))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _context.Products
            .Where(p => p.Batches.Select(z => z.SellingPrice).FirstOrDefault() >= minPrice && p.Batches.Select(z => z.SellingPrice).FirstOrDefault() <= maxPrice)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string name)
    {
        return await _context.Products
            .Where(p => p.Name.Contains(name))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductAttributeValue>> GetAttributeValuesByProductIdAsync(long productId)
    {
        return await _context.ProductAttributeValues
            .Where(value => value.ProductId == productId && !value.IsDeleted)
            .Include(value => value.AttributeDefinition)
            .OrderBy(value => value.AttributeDefinition.SortOrder)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Product?> GetProductWithAttributeValuesAsync(long productId)
    {
        return await _context.Products
            .Include(product => product.AttributeValues)
            .FirstOrDefaultAsync(product => product.Id == productId && !product.IsDeleted);
    }

    public async Task<Product?> GetProductDetailsByIdAsync(long productId)
    {
        return await _context.Products
            .Where(product => product.Id == productId && !product.IsDeleted)
            .Include(product => product.Category)
            .Include(product => product.Brand)
            .Include(product => product.Suppliers).ThenInclude(productSupplier => productSupplier.Supplier)
            .Include(product => product.Batches)
            .Include(product => product.Stocks).ThenInclude(stock => stock.Space)
            .Include(product => product.Stocks).ThenInclude(stock => stock.Zone)
            .Include(product => product.Stocks).ThenInclude(stock => stock.Shelf)
            .Include(product => product.Stocks).ThenInclude(stock => stock.Warehouse)
            .Include(product => product.Attributes)
            .Include(product => product.AttributeValues).ThenInclude(value => value.AttributeDefinition)
            .Include(product => product.Dimensions)
            .Include(product => product.Images)
            .Include(product => product.VariantDefinitions).ThenInclude(variant => variant.Options)
            .Include(product => product.Tags).ThenInclude(productTag => productTag.Tag)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

}
