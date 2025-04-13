namespace Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure Logging with Elasticsearch
        //Log.Logger = new LoggerConfiguration()
        //    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
        //    {
        //        AutoRegisterTemplate = true,
        //    })
        //    .CreateLogger();
        //builder.Host.UseSerilog();

        // Database Context - Scoped (shared per request)
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Scoped);

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

        // Register Services from Application Layer

        services.AddScoped<IBasketService, BasketService>();
        services.AddScoped<IBasketItemService, BasketItemService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPromotionService, PromotionService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IUserService, UserService>();

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
        services.AddScoped<RedisCacheService>();

        return services;
    }
}