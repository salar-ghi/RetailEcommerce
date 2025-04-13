namespace Infrastructure.Repositories;

public class PaymentRepository : Repository<Payment, string>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context) { }

    public async Task<Payment> GetByTransactionIdAsync(string transactionId)
    {
        return await _context
            .Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
    }

    public async Task<IEnumerable<Payment>> GetByOrderIdAsync(string orderId)
    {
        return await _context
            .Payments
            .AsNoTracking()
            .Where(p => p.OrderId == orderId)
            .ToListAsync();
    }
}
