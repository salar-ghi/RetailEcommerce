namespace Application.DTOs;

public class AttributeOptionDto
{
    public int Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public string? Label { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AttributeDefinitionDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
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
    public List<AttributeOptionDto> Options { get; set; } = new();
    public int? CategoryId { get; set; }
}

public class UpsertAttributeDefinitionDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
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
    public List<AttributeOptionDto> Options { get; set; } = new();
    public int? CategoryId { get; set; }
}
