namespace Domain.Entities;

public class CategoryAttributeDefinition : BaseModel<int>
{
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public int AttributeDefinitionId { get; set; }
    public AttributeDefinition AttributeDefinition { get; set; }
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisibleOnProductPage { get; set; } = true;
    public bool IsFilterable { get; set; } = true;
}
