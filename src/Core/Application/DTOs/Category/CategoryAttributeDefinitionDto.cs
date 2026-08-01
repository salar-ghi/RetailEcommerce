namespace Application.DTOs;

public class CategoryAttributeDefinitionDto
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public int AttributeDefinitionId { get; set; }
    public AttributeDefinitionDto? AttributeDefinition { get; set; }
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisibleOnProductPage { get; set; } = true;
    public bool IsFilterable { get; set; } = true;
}

public class AssignCategoryAttributeDefinitionDto
{
    public int AttributeDefinitionId { get; set; }
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisibleOnProductPage { get; set; } = true;
    public bool IsFilterable { get; set; } = true;
}

public class UpdateCategoryAttributeDefinitionDto
{
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisibleOnProductPage { get; set; } = true;
    public bool IsFilterable { get; set; } = true;
}
