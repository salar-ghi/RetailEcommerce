using Domain.Enums;

namespace Domain.Entities;

public class Product : BaseModel<long>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ProductStatus Status { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public int BrandId { get; set; }
    public Brand Brand { get; set; }

    // One to one relation
    public ProductDimensions Dimensions { get; set; }
    public ProductStock Stock { get; set; }

    // One to many relation
    public ICollection<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
    public ICollection<ProductUnitPrice> UnitPrices { get; set; } = new List<ProductUnitPrice>();
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public virtual ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();


    // many to many relation
    public ICollection<ProductSupplier> Suppliers { get; set; } = new List<ProductSupplier>();
    public ICollection<ProductTag> Tags { get; set; } = new List<ProductTag>();



}
