namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HomeController : ControllerBase
{
    private const int SectionProductCount = 10;
    private const string DigitalCategoryName = "کالای دیجیتال";
    private const string PerfumeCategoryName = "عطر و ادکلن";
    private const string SportTravelCategoryName = "ورزش و سفر";
    private const string CoffeeDrinkCategoryName = "قهوه و نوشیدنی";

    private readonly IBannerService _bannerService;
    private readonly IProductService _productService;

    public HomeController(IBannerService bannerService, IProductService productService)
    {
        _bannerService = bannerService;
        _productService = productService;
    }

    [HttpGet("Index")]
    public async Task<ActionResult<HomeIndexDto>> Index()
    {
        var topBannersTask = _bannerService.GetByPlacementAsync(BannerPageCode.HOME_TOP);
        var bottomBannersTask = _bannerService.GetByPlacementAsync(BannerPageCode.HOME_BOTTOM);
        var productsTask = _productService.GetAllProductsAsync();

        await Task.WhenAll(topBannersTask, bottomBannersTask, productsTask);

        var allProducts = productsTask.Result.ToList();
        var digitalProducts = ProductsByCategory(allProducts, DigitalCategoryName).ToList();

        var model = new HomeIndexDto(
            HeroBanners: topBannersTask.Result.Select(ToHomeBanner).ToList(),
            PromoBanners: bottomBannersTask.Result.Select(ToHomeBanner).ToList(),
            ProductCarousels: new List<HomeProductCarouselDto>
            {
                new(
                    Id: 1,
                    Title: "کالای دیجیتال",
                    ViewAllLink: CategoryLink(DigitalCategoryName),
                    Variant: "default",
                    Products: digitalProducts
                        .Take(SectionProductCount)
                        .Select(product => ToHomeProduct(product))
                        .ToList()),
                new(
                    Id: 2,
                    Title: "جدیدترین کالای دیجیتال",
                    ViewAllLink: CategoryLink(DigitalCategoryName),
                    Variant: "default",
                    Products: digitalProducts
                        .OrderByDescending(product => product.Id)
                        .Take(SectionProductCount)
                        .Select(product => ToHomeProduct(product, isNew: true))
                        .ToList()),
                new(
                    Id: 3,
                    Title: "محصولات تخفیف‌دار",
                    ViewAllLink: "/offers",
                    Variant: "special",
                    Products: allProducts
                        .Select(product => new { Product = product, Discount = GetBestDiscount(product) })
                        .Where(item => item.Discount is not null)
                        .OrderByDescending(item => item.Discount!.Percentage)
                        .Take(SectionProductCount)
                        .Select(item => ToHomeProduct(item.Product, item.Discount))
                        .ToList()),
                CategoryCarousel(4, "عطر و ادکلن", PerfumeCategoryName, allProducts),
                CategoryCarousel(5, "ورزش و سفر", SportTravelCategoryName, allProducts),
                CategoryCarousel(6, "قهوه و نوشیدنی", CoffeeDrinkCategoryName, allProducts)
            });

        return Ok(model);
    }

    private static HomeProductCarouselDto CategoryCarousel(
        int id,
        string title,
        string categoryName,
        IEnumerable<ProductDto> allProducts)
    {
        return new HomeProductCarouselDto(
            Id: id,
            Title: title,
            ViewAllLink: CategoryLink(categoryName),
            Variant: "default",
            Products: ProductsByCategory(allProducts, categoryName)
                .Take(SectionProductCount)
                .Select(product => ToHomeProduct(product))
                .ToList());
    }

    private static IEnumerable<ProductDto> ProductsByCategory(IEnumerable<ProductDto> products, string categoryName)
    {
        return products.Where(product => string.Equals(
            product.CategoryName?.Trim(),
            categoryName,
            StringComparison.OrdinalIgnoreCase));
    }

    private static HomeBannerDto ToHomeBanner(BannerDto banner)
    {
        return new HomeBannerDto(
            Id: banner.Id,
            Title: banner.Name,
            Description: banner.Description,
            Link: banner.Link ?? "/",
            ImageBase64: banner.ImageUrl,
            ImageUrl: banner.ImageUrl,
            MimeType: "image/jpeg",
            Position: banner.Placements.FirstOrDefault()?.Code,
            BackgroundColor: null);
    }

    private static HomeProductDto ToHomeProduct(
        ProductDto product,
        ProductDiscountDto? discount = null,
        bool isNew = false)
    {
        discount ??= GetBestDiscount(product);
        var image = product.Images?.FirstOrDefault() ?? product.CoverImage;

        return new HomeProductDto(
            Id: product.Id,
            Name: product.Name,
            Price: discount?.OriginalPrice ?? product.Price,
            DiscountPrice: discount?.DiscountPrice,
            DiscountPercentage: discount?.Percentage,
            ImageBase64: image,
            ImageUrl: image,
            MimeType: "image/png",
            Rating: 0,
            Category: product.CategoryName,
            IsFavorite: false,
            IsNew: isNew);
    }

    private static ProductDiscountDto? GetBestDiscount(ProductDto product)
    {
        return product.Prices?
            .Where(price => price.CostPrice > price.Amount && price.Amount > 0)
            .Select(price => new ProductDiscountDto(
                OriginalPrice: price.CostPrice,
                DiscountPrice: price.Amount,
                Percentage: (int)Math.Round((price.CostPrice - price.Amount) * 100 / price.CostPrice)))
            .OrderByDescending(discount => discount.Percentage)
            .FirstOrDefault();
    }

    private static string CategoryLink(string categoryName)
    {
        return $"/category/{Uri.EscapeDataString(categoryName)}";
    }
}

public record HomeIndexDto(
    IReadOnlyList<HomeBannerDto> HeroBanners,
    IReadOnlyList<HomeBannerDto> PromoBanners,
    IReadOnlyList<HomeProductCarouselDto> ProductCarousels);

public record HomeBannerDto(
    int Id,
    string Title,
    string? Description,
    string Link,
    string? ImageBase64,
    string? ImageUrl,
    string MimeType,
    string? Position,
    string? BackgroundColor);

public record HomeProductCarouselDto(
    int Id,
    string Title,
    string ViewAllLink,
    string Variant,
    IReadOnlyList<HomeProductDto> Products);

public record HomeProductDto(
    int Id,
    string Name,
    decimal Price,
    decimal? DiscountPrice,
    int? DiscountPercentage,
    string? ImageBase64,
    string? ImageUrl,
    string MimeType,
    decimal Rating,
    string Category,
    bool IsFavorite,
    bool IsNew);

public record ProductDiscountDto(decimal OriginalPrice, decimal DiscountPrice, int Percentage);
