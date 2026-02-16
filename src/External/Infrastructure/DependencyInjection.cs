using Application.Helper;
using Infrastructure.Persistence;
using Infrastructure.Services;

namespace Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString,
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromMilliseconds(10),
                        errorNumbersToAdd: null);
                });
            options.EnableThreadSafetyChecks(true);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        // Redis Distributed Cache - Singleton (shared across app lifetime)
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection");
        });

        services.AddSingleton<IRedisCacheService, RedisCacheService>();

        services.AddAutoMapper(typeof(BasketMappingProfile));
        services.AddAutoMapper(typeof(BrandMappingProfile));
        services.AddAutoMapper(typeof(CategoryMappingProfile));
        services.AddAutoMapper(typeof(OrderMappingProfile));
        services.AddAutoMapper(typeof(ProductMappingProfile));
        services.AddAutoMapper(typeof(BannerMappingProfile));
       
        services.AddAutoMapper(typeof(SupplierMappingProfile));
        services.AddAutoMapper(typeof(TagMappingProfile));
        services.AddAutoMapper(typeof(UserMappingProfile));
        services.AddAutoMapper(typeof(PromotionMappingProfile));
        services.AddAutoMapper(typeof(PaymentMappingProfile));

        // Logging - Singleton (default ILogger<T> is fine)
        services.AddLogging();

        // Unit of Work - Scoped (shared per request)
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Generic Repository - Scoped (shared per request)
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

        // Specific Repositories - Scoped (shared per request)
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryAttributeRepository, CategoryAttributeRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
        services.AddScoped<IProductDimensionsRepository, ProductDimensionsRepository>();
        services.AddScoped<IProductImageRepository, ProductImageRepository>();
        services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
        services.AddScoped<IProductStockRepository, ProductStockRepository>();
        services.AddScoped<IProductSupplierRepository, ProductSupplierRepository>();
        services.AddScoped<IProductTagRepository, ProductTagRepository>();
        services.AddScoped<IProductUnitPriceRepository, ProductUnitPriceRepository>();
        services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserAddressRepository, UserAddressRepository>();
        services.AddScoped<IBasketRepository, BasketRepository>();
        services.AddScoped<IBasketItemRepository, BasketItemRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IPromotionRepository, PromotionRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IBannerRepository, BannerRepository>();
        services.AddScoped<IBannerPlacementRepository, BannerPlacementRepository>();

        // Register Services from Application Layer

        services.AddScoped<IBasketService, BasketService>();
        services.AddScoped<IBasketItemService, BasketItemService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPromotionService, PromotionService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBannerService, BannerService>();
        services.AddScoped<IBannerPlacementService, BannerPlacementService>();

        services.AddScoped<ProductService>();
        services.AddScoped<ProductAttributeService>();
        services.AddScoped<ProductDimensionsService>();
        services.AddScoped<ProductReviewService>();
        services.AddScoped<ProductStockService>();
        services.AddScoped<ProductVariantService>();
        services.AddScoped<ProductImageService>();
        services.AddScoped<ProductSupplierService>();
        services.AddScoped<ProductTagService>();
        services.AddScoped<ProductUnitPriceService>();
        services.AddScoped<BrandService>();
        services.AddScoped<CategoryService>();
        services.AddScoped<CategoryAttributeService>();
        services.AddScoped<SupplierService>();
        services.AddScoped<TagService>();
        services.AddScoped<RoleService>();
        services.AddScoped<UserRoleService>();
        services.AddScoped<UserAddressService>();

        // Add other services as needed: services.AddScoped<CategoryService>();

        // Register Caching Service
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddScoped<DatabaseSeeder>();

        services.AddScoped<RedisCacheService>();

        services.AddScoped<IImageHelper, ImageHelper>();
        services.AddHostedService<DatabaseSeederHostedService>();
        return services;
    }
}