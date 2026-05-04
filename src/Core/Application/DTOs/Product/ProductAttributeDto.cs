namespace Application.DTOs;

public class ProductAttributeDto
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
    public int ProductId { get; set; }
}


public class AttributeDto
{
    public string Key { get; set; }
    public string Value { get; set; }
}

public class VariantDefinitionDto
{
    public string Name { get; set; }
    public string Type { get; set; }
    public List<string> Options { get; set; } = new();
    public bool Required { get; set; }
    public int? DisplayOrder { get; set; }
}