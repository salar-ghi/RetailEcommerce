namespace Application.DTOs;

public class PaymentDto
{
    public string Id { get; set; }
    public string OrderId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public string Status { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string TransactionId { get; set; }
}
