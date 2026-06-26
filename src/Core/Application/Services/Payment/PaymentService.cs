namespace Application.Services;

public class PaymentService : IPaymentService
{
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
        payment.Amount = FinanceMoney.Normalize(payment.Amount, FinanceCurrency.IRR);
        await _unitOfWork.Payments.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        if (payment.Status == PaymentStatus.Completed)
        {
            await _financeService.RecordOrderPaymentAsync(new RecordOrderFinanceDto
            {
                OrderId = payment.OrderId,
                FinanceAccountId = "default-cash",
                PaymentMethod = MapPaymentMethod(payment.Method),
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

    private static FinancePaymentMethod MapPaymentMethod(PaymentMethod method) => method switch
    {
        PaymentMethod.Card => FinancePaymentMethod.Card,
        PaymentMethod.DigiPay => FinancePaymentMethod.Wallet,
        PaymentMethod.SnappPay => FinancePaymentMethod.Wallet,
        _ => FinancePaymentMethod.Cash
    };
}
