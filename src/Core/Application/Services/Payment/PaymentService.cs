namespace Application.Services;

public class PaymentService : IPaymentService
{
    private const string DefaultBranchId = "default-branch";
    private const string DefaultFinanceAccountId = "default-cash";
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFinanceService _financeService;

    public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IFinanceService financeService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _financeService = financeService;
    }

    public async Task<PaymentDto> CreatePaymentAsync(PaymentDto paymentDto)
    {
        var payment = _mapper.Map<Payment>(paymentDto);
        payment.Id = Guid.NewGuid().ToString();
        payment.BranchId = NormalizeBranchId(payment.BranchId);
        payment.FinanceAccountId = NormalizeFinanceAccountId(payment.FinanceAccountId);
        payment.TransactionId = NormalizeTransactionId(payment.TransactionId);
        if (payment.PaymentDate == default) payment.PaymentDate = DateTime.UtcNow;
        payment.Amount = FinanceMoney.Normalize(payment.Amount, FinanceCurrency.IRR);
        await _unitOfWork.Payments.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        if (payment.Status == PaymentStatus.Completed)
        {
            await _financeService.RecordOrderPaymentAsync(new RecordOrderFinanceDto
            {
                OrderId = payment.OrderId,
                FinanceAccountId = payment.FinanceAccountId,
                PaymentMethod = MapPaymentMethod(payment.Method),
                BranchId = payment.BranchId,
                CounterpartyName = payment.Supplier?.Name
            });
        }

        return _mapper.Map<PaymentDto>(payment);
    }

    public async Task<PaymentDto> GetPaymentByIdAsync(string id)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(id);
        return _mapper.Map<PaymentDto>(payment);
    }

    public async Task<IEnumerable<PaymentDto>> GetPaymentsByOrderIdAsync(string orderId)
    {
        var payments = await _unitOfWork.Payments.GetByOrderIdAsync(orderId);
        return _mapper.Map<IEnumerable<PaymentDto>>(payments);
    }

    private static string NormalizeBranchId(string? branchId) => string.IsNullOrWhiteSpace(branchId) ? DefaultBranchId : branchId.Trim();
    private static string NormalizeFinanceAccountId(string? financeAccountId) => string.IsNullOrWhiteSpace(financeAccountId) ? DefaultFinanceAccountId : financeAccountId.Trim();
    private static string NormalizeTransactionId(string? transactionId) => string.IsNullOrWhiteSpace(transactionId) ? string.Empty : transactionId.Trim();

    private static FinancePaymentMethod MapPaymentMethod(PaymentMethod method) => method switch
    {
        PaymentMethod.Card => FinancePaymentMethod.Card,
        PaymentMethod.DigiPay => FinancePaymentMethod.Wallet,
        PaymentMethod.SnappPay => FinancePaymentMethod.Wallet,
        _ => FinancePaymentMethod.Cash
    };
}
