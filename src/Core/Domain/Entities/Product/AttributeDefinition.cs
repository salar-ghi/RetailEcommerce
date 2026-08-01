namespace Domain.Entities;

public class AttributeDefinition : BaseModel<int>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public AttributeDataType DataType { get; set; }
    public string? Unit { get; set; }
    public bool IsFilterable { get; set; } = true;
    public bool IsSearchable { get; set; } = true;
    public bool IsComparable { get; set; }
    public bool IsRequired { get; set; }
    public bool IsVariantAttribute { get; set; }
    public int SortOrder { get; set; }
    public string? ValidationRegex { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public ICollection<AttributeOption> Options { get; set; } = new List<AttributeOption>();
}
