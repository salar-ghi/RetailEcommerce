namespace Domain.Entities;

public class AttributeOption : BaseModel<int>
{
    public int AttributeDefinitionId { get; set; }
    public AttributeDefinition AttributeDefinition { get; set; }
    public string Value { get; set; }
    public string? Label { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
