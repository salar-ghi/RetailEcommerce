using Domain.Enums;

namespace Domain.Entities;

public class Product : BaseModel<long>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ProductStatus? Status { get; set; }
    public ProductAvailability Availability { get; set; }
    public bool IsActive { get; set; }
    public int? SpaceId { get; set; }
    public int? ZoneId { get; set; }
    public int? ShelfId { get; set; }
    public string StorageLocationNote { get; set; }
    public int CategoryId { get; set; }
    [ForeignKey(nameof(CategoryId))]
    public Category Category { get; set; }
    public int BrandId { get; set; }
    [ForeignKey(nameof(BrandId))]
    public Brand Brand { get; set; }
    public ProductDimensions Dimensions { get; set; }
    public ICollection<ProductInventoryBatch> Batches { get; set; } = new List<ProductInventoryBatch>();
    public ICollection<ProductVariantDefinition> VariantDefinitions { get; set; } = new List<ProductVariantDefinition>();
    public ICollection<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
    public virtual ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
    public ICollection<ProductTag> Tags { get; set; } = new List<ProductTag>();
    public ICollection<ProductSupplier> Suppliers { get; set; } = new List<ProductSupplier>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
