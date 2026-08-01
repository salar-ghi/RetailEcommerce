namespace Application.DTOs;

public class ProductAttributeValueDto
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public int AttributeDefinitionId { get; set; }
    public string? AttributeCode { get; set; }
    public string? AttributeName { get; set; }
    public string? StringValue { get; set; }
    public int? IntValue { get; set; }
    public decimal? DecimalValue { get; set; }
    public bool? BoolValue { get; set; }
    public DateTime? DateValue { get; set; }
    public int? AttributeOptionId { get; set; }
    public int[]? AttributeOptionIds { get; set; }
}
