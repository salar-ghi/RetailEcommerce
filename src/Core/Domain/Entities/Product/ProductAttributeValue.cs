namespace Domain.Entities;

public class ProductAttributeValue : BaseModel<long>
{
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public int AttributeDefinitionId { get; set; }
    public AttributeDefinition AttributeDefinition { get; set; }
    public string? StringValue { get; set; }
    public int? IntValue { get; set; }
    public decimal? DecimalValue { get; set; }
    public bool? BoolValue { get; set; }
    public DateTime? DateValue { get; set; }
    public int? AttributeOptionId { get; set; }
    public AttributeOption? AttributeOption { get; set; }
    public string? AttributeOptionIds { get; set; }
}
