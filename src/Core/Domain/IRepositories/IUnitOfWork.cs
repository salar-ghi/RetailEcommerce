namespace Domain.IRepositories;

// Domain Layer
public interface IUnitOfWork : IDisposable
{
    ICategoryRepository Categories { get; }
    IBannerRepository Banners { get; }
    IBannerPlacementRepository BannerPlacements { get; }
    ICategoryAttributeRepository CategoryAttributes { get; }
    IBrandRepository Brands { get; }
    IBasketRepository Baskets { get; }
    IBasketItemRepository BasketItems { get; }
    ISupplierRepository Suppliers { get; }
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IUserRoleRepository UserRoles { get; }
    IUserAddressRepository UserAddresses { get; }
    IProductRepository Products { get; }
    IProductAttributeRepository ProductAttributes { get; }
    IProductDimensionsRepository ProductDimensions { get; }
    IProductImageRepository ProductImages { get; }
    IProductReviewRepository ProductReviews { get; }
    IProductStockRepository ProductStocks { get; }
    IProductSupplierRepository ProductSuppliers { get; }
    ITagRepository Tags { get; }
    IProductTagRepository ProductTags { get; }
    IProductUnitPriceRepository ProductUnitPrices { get; }
    IProductVariantRepository ProductVariants { get; }

    IOrderRepository Orders { get; }
    IOrderItemRepository OrderItems { get; }

    IPromotionRepository Promotions { get; }
    IPaymentRepository Payments { get; }

    Task<int> SaveChangesAsync();
}