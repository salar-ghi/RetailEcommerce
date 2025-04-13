namespace Infrastructure.Repositories;

// Infrastructure Layer
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly IDistributedCache _cache;
    public UnitOfWork(AppDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
        Categories = new CategoryRepository(context);
        CategoryAttributes = new CategoryAttributeRepository(context);
        Brands = new BrandRepository(context);
        Suppliers = new SupplierRepository(context);
        Users = new UserRepository(context);
        Roles = new RoleRepository(context);
        UserRoles = new UserRoleRepository(context);
        UserAddresses = new UserAddressRepository(context);
        Products = new ProductRepository(context);
        ProductAttributes = new ProductAttributeRepository(context);
        ProductDimensions = new ProductDimensionsRepository(context);
        ProductImages = new ProductImageRepository(context);
        ProductReviews = new ProductReviewRepository(context);
        ProductStocks = new ProductStockRepository(context);
        ProductSuppliers = new ProductSupplierRepository(context);
        Tags = new TagRepository(context);
        ProductTags = new ProductTagRepository(context);
        ProductUnitPrices = new ProductUnitPriceRepository(context);
        ProductVariants = new ProductVariantRepository(context);
        Baskets = new BasketRepository(context);
        BasketItems = new BasketItemRepository(context);

        Orders = new OrderRepository(context);
        OrderItems = new OrderItemRepository(context);
        Promotions = new PromotionRepository(context);
        Payments = new PaymentRepository(context);
    }

    public ICategoryRepository Categories { get; }
    public ICategoryAttributeRepository CategoryAttributes { get; }
    public IBrandRepository Brands { get; }
    public IBasketRepository Baskets { get; }
    public IBasketItemRepository BasketItems { get; }
    public ISupplierRepository Suppliers { get; }
    public IUserRepository Users { get; }
    public IRoleRepository Roles { get; }
    public IUserRoleRepository UserRoles { get; }
    public IUserAddressRepository UserAddresses { get; }
    public IProductRepository Products { get; }
    public IProductAttributeRepository ProductAttributes { get; }
    public IProductDimensionsRepository ProductDimensions { get; }
    public IProductImageRepository ProductImages { get; }
    public IProductReviewRepository ProductReviews { get; }
    public IProductStockRepository ProductStocks { get; }
    public IProductSupplierRepository ProductSuppliers { get; }
    public ITagRepository Tags { get; }
    public IProductTagRepository ProductTags { get; }
    public IProductUnitPriceRepository ProductUnitPrices { get; }
    public IProductVariantRepository ProductVariants { get; }
    public IOrderRepository Orders { get; }
    public IOrderItemRepository OrderItems { get; }

    public IPromotionRepository Promotions { get; }
    public IPaymentRepository Payments { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}