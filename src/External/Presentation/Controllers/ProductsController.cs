namespace Presentation.Controllers;

// Presentation/Controllers/ProductController.cs
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly ProductAttributeService _attributeService;
    private readonly ProductDimensionsService _dimensionsService;
    private readonly ProductReviewService _reviewService;
    private readonly ProductStockService _stockService;
    private readonly ProductVariantService _variantService;
    private readonly ProductImageService _imageService;
    private readonly ProductSupplierService _supplierService;
    private readonly ProductTagService _tagService;
    private readonly ProductUnitPriceService _unitPriceService;

    public ProductController(
        ProductService productService,
        ProductAttributeService attributeService,
        ProductDimensionsService dimensionsService,
        ProductReviewService reviewService,
        ProductStockService stockService,
        ProductVariantService variantService,
        ProductImageService imageService,
        ProductSupplierService supplierService,
        ProductTagService tagService,
        ProductUnitPriceService unitPriceService)
    {
        _productService = productService;
        _attributeService = attributeService;
        _dimensionsService = dimensionsService;
        _reviewService = reviewService;
        _stockService = stockService;
        _variantService = variantService;
        _imageService = imageService;
        _supplierService = supplierService;
        _tagService = tagService;
        _unitPriceService = unitPriceService;
    }

    // Product CRUD Operations
    [HttpGet("products")]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("products/{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        return Ok(product);
    }

    [HttpPost("products")]
    public async Task<IActionResult> AddProduct(ProductDto productDto)
    {
        await _productService.AddProductAsync(productDto);
        return CreatedAtAction(nameof(GetProductById), new { id = productDto.Id }, productDto);
    }

    [HttpPut("products/{id}")]
    public async Task<IActionResult> UpdateProduct(int id, ProductDto productDto)
    {
        if (id != productDto.Id) return BadRequest();
        await _productService.UpdateProductAsync(productDto);
        return NoContent();
    }

    [HttpDelete("products/{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }

    // Product Search Operations
    [HttpGet("products/search/name")]
    public async Task<IActionResult> SearchProductsByName(string name)
    {
        var products = await _productService.SearchProductsByNameAsync(name);
        return Ok(products);
    }

    [HttpGet("products/search/price")]
    public async Task<IActionResult> SearchProductsByPriceRange(decimal minPrice, decimal maxPrice)
    {
        var products = await _productService.SearchProductsByPriceRangeAsync(minPrice, maxPrice);
        return Ok(products);
    }

    [HttpGet("products/search/category")]
    public async Task<IActionResult> SearchProductsByCategory(int categoryId)
    {
        var products = await _productService.SearchProductsByCategoryAsync(categoryId);
        return Ok(products);
    }

    // ProductAttribute CRUD Operations
    [HttpGet("attributes")]
    public async Task<IActionResult> GetAllAttributes()
    {
        var attributes = await _attributeService.GetAllAttributesAsync();
        return Ok(attributes);
    }

    [HttpGet("attributes/{id}")]
    public async Task<IActionResult> GetAttributeById(int id)
    {
        var attribute = await _attributeService.GetAttributeByIdAsync(id);
        return Ok(attribute);
    }

    [HttpPost("attributes")]
    public async Task<IActionResult> AddAttribute(ProductAttributeDto attributeDto)
    {
        await _attributeService.AddAttributeAsync(attributeDto);
        return CreatedAtAction(nameof(GetAttributeById), new { id = attributeDto.Id }, attributeDto);
    }

    [HttpPut("attributes/{id}")]
    public async Task<IActionResult> UpdateAttribute(int id, ProductAttributeDto attributeDto)
    {
        if (id != attributeDto.Id) return BadRequest();
        await _attributeService.UpdateAttributeAsync(attributeDto);
        return NoContent();
    }

    [HttpDelete("attributes/{id}")]
    public async Task<IActionResult> DeleteAttribute(int id)
    {
        await _attributeService.DeleteAttributeAsync(id);
        return NoContent();
    }

    // ProductAttribute Search Operations
    [HttpGet("attributes/search/product")]
    public async Task<IActionResult> SearchAttributesByProductId(int productId)
    {
        var attributes = await _attributeService.SearchAttributesByProductIdAsync(productId);
        return Ok(attributes);
    }

    [HttpGet("attributes/search/key")]
    public async Task<IActionResult> SearchAttributesByKey(string key)
    {
        var attributes = await _attributeService.SearchAttributesByKeyAsync(key);
        return Ok(attributes);
    }

    // ProductDimensions CRUD Operations
    [HttpGet("dimensions/{id}")]
    public async Task<IActionResult> GetDimensionsById(int id)
    {
        var dimensions = await _dimensionsService.GetDimensionsByIdAsync(id);
        return Ok(dimensions);
    }

    [HttpPost("dimensions")]
    public async Task<IActionResult> AddDimensions(ProductDimensionsDto dimensionsDto)
    {
        await _dimensionsService.AddDimensionsAsync(dimensionsDto);
        return CreatedAtAction(nameof(GetDimensionsById), new { id = dimensionsDto.Id }, dimensionsDto);
    }

    [HttpPut("dimensions/{id}")]
    public async Task<IActionResult> UpdateDimensions(int id, ProductDimensionsDto dimensionsDto)
    {
        if (id != dimensionsDto.Id) return BadRequest();
        await _dimensionsService.UpdateDimensionsAsync(dimensionsDto);
        return NoContent();
    }

    [HttpDelete("dimensions/{id}")]
    public async Task<IActionResult> DeleteDimensions(int id)
    {
        await _dimensionsService.DeleteDimensionsAsync(id);
        return NoContent();
    }

    // ProductDimensions Search Operation
    [HttpGet("dimensions/search/product")]
    public async Task<IActionResult> SearchDimensionsByProductId(int productId)
    {
        var dimensions = await _dimensionsService.SearchDimensionsByProductIdAsync(productId);
        return Ok(dimensions);
    }

    // ProductReview CRUD Operations
    [HttpGet("reviews")]
    public async Task<IActionResult> GetAllReviews()
    {
        var reviews = await _reviewService.GetAllReviewsAsync();
        return Ok(reviews);
    }

    [HttpGet("reviews/{id}")]
    public async Task<IActionResult> GetReviewById(int id)
    {
        var review = await _reviewService.GetReviewByIdAsync(id);
        return Ok(review);
    }

    [HttpPost("reviews")]
    public async Task<IActionResult> AddReview(ProductReviewDto reviewDto)
    {
        await _reviewService.AddReviewAsync(reviewDto);
        return CreatedAtAction(nameof(GetReviewById), new { id = reviewDto.Id }, reviewDto);
    }

    [HttpPut("reviews/{id}")]
    public async Task<IActionResult> UpdateReview(int id, ProductReviewDto reviewDto)
    {
        if (id != reviewDto.Id) return BadRequest();
        await _reviewService.UpdateReviewAsync(reviewDto);
        return NoContent();
    }

    [HttpDelete("reviews/{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        await _reviewService.DeleteReviewAsync(id);
        return NoContent();
    }

    // ProductReview Search Operations
    [HttpGet("reviews/search/product")]
    public async Task<IActionResult> SearchReviewsByProductId(int productId)
    {
        var reviews = await _reviewService.SearchReviewsByProductIdAsync(productId);
        return Ok(reviews);
    }

    [HttpGet("reviews/search/rating")]
    public async Task<IActionResult> SearchReviewsByRating(int minRating, int maxRating)
    {
        var reviews = await _reviewService.SearchReviewsByRatingAsync(minRating, maxRating);
        return Ok(reviews);
    }

    // ProductStock CRUD Operations
    [HttpGet("stocks/{id}")]
    public async Task<IActionResult> GetStockById(int id)
    {
        var stock = await _stockService.GetStockByIdAsync(id);
        return Ok(stock);
    }

    [HttpPost("stocks")]
    public async Task<IActionResult> AddStock(ProductStockDto stockDto)
    {
        await _stockService.AddStockAsync(stockDto);
        return CreatedAtAction(nameof(GetStockById), new { id = stockDto.Id }, stockDto);
    }

    [HttpPut("stocks/{id}")]
    public async Task<IActionResult> UpdateStock(int id, ProductStockDto stockDto)
    {
        if (id != stockDto.Id) return BadRequest();
        await _stockService.UpdateStockAsync(stockDto);
        return NoContent();
    }

    [HttpDelete("stocks/{id}")]
    public async Task<IActionResult> DeleteStock(int id)
    {
        await _stockService.DeleteStockAsync(id);
        return NoContent();
    }

    // ProductStock Search Operations
    [HttpGet("stocks/search/product")]
    public async Task<IActionResult> SearchStockByProductId(int productId)
    {
        var stock = await _stockService.SearchStockByProductIdAsync(productId);
        return Ok(stock);
    }

    [HttpGet("stocks/search/low")]
    public async Task<IActionResult> SearchLowStock(int threshold)
    {
        var stocks = await _stockService.SearchLowStockAsync(threshold);
        return Ok(stocks);
    }

    // ProductVariant CRUD Operations
    [HttpGet("variants")]
    public async Task<IActionResult> GetAllVariants()
    {
        var variants = await _variantService.GetAllVariantsAsync();
        return Ok(variants);
    }

    [HttpGet("variants/{id}")]
    public async Task<IActionResult> GetVariantById(int id)
    {
        var variant = await _variantService.GetVariantByIdAsync(id);
        return Ok(variant);
    }

    [HttpPost("variants")]
    public async Task<IActionResult> AddVariant(ProductVariantDto variantDto)
    {
        await _variantService.AddVariantAsync(variantDto);
        return CreatedAtAction(nameof(GetVariantById), new { id = variantDto.Id }, variantDto);
    }

    [HttpPut("variants/{id}")]
    public async Task<IActionResult> UpdateVariant(int id, ProductVariantDto variantDto)
    {
        if (id != variantDto.Id) return BadRequest();
        await _variantService.UpdateVariantAsync(variantDto);
        return NoContent();
    }

    [HttpDelete("variants/{id}")]
    public async Task<IActionResult> DeleteVariant(int id)
    {
        await _variantService.DeleteVariantAsync(id);
        return NoContent();
    }

    // ProductVariant Search Operations
    [HttpGet("variants/search/product")]
    public async Task<IActionResult> SearchVariantsByProductId(int productId)
    {
        var variants = await _variantService.SearchVariantsByProductIdAsync(productId);
        return Ok(variants);
    }

    [HttpGet("variants/search/name")]
    public async Task<IActionResult> SearchVariantsByName(string variantName)
    {
        var variants = await _variantService.SearchVariantsByNameAsync(variantName);
        return Ok(variants);
    }


    //**************//
    [HttpGet("images")]
    public async Task<IActionResult> GetAllImages()
    {
        var images = await _imageService.GetAllImagesAsync();
        return Ok(images);
    }

    [HttpGet("images/{id}")]
    public async Task<IActionResult> GetImageById(int id)
    {
        var image = await _imageService.GetImageByIdAsync(id);
        return Ok(image);
    }


    [HttpPost("images")]
    public async Task<IActionResult> AddImage(ProductImageDto imageDto)
    {
        await _imageService.AddImageAsync(imageDto);
        return CreatedAtAction(nameof(GetImageById), new { id = imageDto.Id }, imageDto);
    }

    [HttpPut("images/{id}")]
    public async Task<IActionResult> UpdateImage(int id, ProductImageDto imageDto)
    {
        if (id != imageDto.Id) return BadRequest();
        await _imageService.UpdateImageAsync(imageDto);
        return NoContent();
    }

    [HttpDelete("images/{id}")]
    public async Task<IActionResult> DeleteImage(int id)
    {
        await _imageService.DeleteImageAsync(id);
        return NoContent();
    }

    [HttpGet("images/search/product")]
    public async Task<IActionResult> SearchImagesByProductId(int productId)
    {
        var images = await _imageService.SearchImagesByProductIdAsync(productId);
        return Ok(images);
    }

    [HttpGet("images/search/primary")]
    public async Task<IActionResult> SearchImagesByPrimary(bool isPrimary)
    {
        var images = await _imageService.SearchImagesByPrimaryAsync(isPrimary);
        return Ok(images);
    }

    // ProductSupplier CRUD Operations
    [HttpGet("product-suppliers")]
    public async Task<IActionResult> GetAllProductSuppliers()
    {
        var productSuppliers = await _supplierService.GetAllProductSuppliersAsync();
        return Ok(productSuppliers);
    }

    [HttpGet("product-suppliers/{productId}/{supplierId}")]
    public async Task<IActionResult> GetProductSupplierById(int productId, int supplierId)
    {
        var productSupplier = await _supplierService.GetProductSupplierByIdAsync(productId, supplierId);
        return Ok(productSupplier);
    }

    [HttpPost("product-suppliers")]
    public async Task<IActionResult> AddProductSupplier(ProductSupplierDto productSupplierDto)
    {
        await _supplierService.AddProductSupplierAsync(productSupplierDto);
        return CreatedAtAction(nameof(GetProductSupplierById), new { productId = productSupplierDto.ProductId, supplierId = productSupplierDto.SupplierId }, productSupplierDto);
    }

    [HttpPut("product-suppliers/{productId}/{supplierId}")]
    public async Task<IActionResult> UpdateProductSupplier(int productId, int supplierId, ProductSupplierDto productSupplierDto)
    {
        if (productId != productSupplierDto.ProductId || supplierId != productSupplierDto.SupplierId) return BadRequest();
        await _supplierService.UpdateProductSupplierAsync(productSupplierDto);
        return NoContent();
    }

    [HttpDelete("product-suppliers/{productId}/{supplierId}")]
    public async Task<IActionResult> DeleteProductSupplier(int productId, int supplierId)
    {
        await _supplierService.DeleteProductSupplierAsync(productId, supplierId);
        return NoContent();
    }

    [HttpGet("product-suppliers/search/product")]
    public async Task<IActionResult> SearchProductSuppliersByProductId(int productId)
    {
        var productSuppliers = await _supplierService.SearchProductSuppliersByProductIdAsync(productId);
        return Ok(productSuppliers);
    }

    [HttpGet("product-suppliers/search/supplier")]
    public async Task<IActionResult> SearchProductSuppliersBySupplierId(int supplierId)
    {
        var productSuppliers = await _supplierService.SearchProductSuppliersBySupplierIdAsync(supplierId);
        return Ok(productSuppliers);
    }


    [HttpGet("product-tags")]
    public async Task<IActionResult> GetAllProductTags()
    {
        var productTags = await _tagService.GetAllProductTagsAsync();
        return Ok(productTags);
    }

    [HttpGet("product-tags/{productId}/{tagId}")]
    public async Task<IActionResult> GetProductTagById(int productId, int tagId)
    {
        var productTag = await _tagService.GetProductTagByIdAsync(productId, tagId);
        return Ok(productTag);
    }

    [HttpPost("product-tags")]
    public async Task<IActionResult> AddProductTag(ProductTagDto productTagDto)
    {
        await _tagService.AddProductTagAsync(productTagDto);
        return CreatedAtAction(nameof(GetProductTagById), new { productId = productTagDto.ProductId, tagId = productTagDto.TagId }, productTagDto);
    }

    [HttpPut("product-tags/{productId}/{tagId}")]
    public async Task<IActionResult> UpdateProductTag(int productId, int tagId, ProductTagDto productTagDto)
    {
        if (productId != productTagDto.ProductId || tagId != productTagDto.TagId) return BadRequest();
        await _tagService.UpdateProductTagAsync(productTagDto);
        return NoContent();
    }

    [HttpDelete("product-tags/{productId}/{tagId}")]
    public async Task<IActionResult> DeleteProductTag(int productId, int tagId)
    {
        await _tagService.DeleteProductTagAsync(productId, tagId);
        return NoContent();
    }


    [HttpGet("product-tags/search/product")]
    public async Task<IActionResult> SearchProductTagsByProductId(int productId)
    {
        var productTags = await _tagService.SearchProductTagsByProductIdAsync(productId);
        return Ok(productTags);
    }

    [HttpGet("product-tags/search/tag")]
    public async Task<IActionResult> SearchProductTagsByTagId(int tagId)
    {
        var productTags = await _tagService.SearchProductTagsByTagIdAsync(tagId);
        return Ok(productTags);
    }

    // ProductUnitPrice CRUD Operations
    [HttpGet("unit-prices")]
    public async Task<IActionResult> GetAllUnitPrices()
    {
        var unitPrices = await _unitPriceService.GetAllUnitPricesAsync();
        return Ok(unitPrices);
    }

    [HttpGet("unit-prices/{id}")]
    public async Task<IActionResult> GetUnitPriceById(int id)
    {
        var unitPrice = await _unitPriceService.GetUnitPriceByIdAsync(id);
        return Ok(unitPrice);
    }

    [HttpPost("unit-prices")]
    public async Task<IActionResult> AddUnitPrice(ProductUnitPriceDto unitPriceDto)
    {
        await _unitPriceService.AddUnitPriceAsync(unitPriceDto);
        return CreatedAtAction(nameof(GetUnitPriceById), new { id = unitPriceDto.Id }, unitPriceDto);
    }

    [HttpPut("unit-prices/{id}")]
    public async Task<IActionResult> UpdateUnitPrice(int id, ProductUnitPriceDto unitPriceDto)
    {
        if (id != unitPriceDto.Id) return BadRequest();
        await _unitPriceService.UpdateUnitPriceAsync(unitPriceDto);
        return NoContent();
    }

    [HttpDelete("unit-prices/{id}")]
    public async Task<IActionResult> DeleteUnitPrice(int id)
    {
        await _unitPriceService.DeleteUnitPriceAsync(id);
        return NoContent();
    }

    [HttpGet("unit-prices/search/product")]
    public async Task<IActionResult> SearchUnitPricesByProductId(int productId)
    {
        var unitPrices = await _unitPriceService.SearchUnitPricesByProductIdAsync(productId);
        return Ok(unitPrices);
    }

    [HttpGet("unit-prices/search/price")]
    public async Task<IActionResult> SearchUnitPricesByPriceRange(decimal minPrice, decimal maxPrice)
    {
        var unitPrices = await _unitPriceService.SearchUnitPricesByPriceRangeAsync(minPrice, maxPrice);
        return Ok(unitPrices);
    }

}