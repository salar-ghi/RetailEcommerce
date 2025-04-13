namespace Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentDto> CreatePaymentAsync(PaymentDto paymentDto);
    Task<PaymentDto> GetPaymentByIdAsync(string id);
    Task<IEnumerable<PaymentDto>> GetPaymentsByOrderIdAsync(string orderId);
}
