namespace Domain.Entities;

public class ProductVariantOption : BaseModel<int>
{
    public string Value { get; set; }           // e.g., "قرمز", "XL"
    public int DisplayOrder { get; set; }

    public int DefinitionId { get; set; }
    public ProductVariantDefinition Definition { get; set; }
}
