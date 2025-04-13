namespace Domain.IRepositories;

public interface IPaymentRepository : IRepository<Payment, string>
{
    Task<Payment> GetByTransactionIdAsync(string transactionId);
    Task<IEnumerable<Payment>> GetByOrderIdAsync(string orderId);
}
