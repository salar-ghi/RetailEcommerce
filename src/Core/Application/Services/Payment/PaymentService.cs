namespace Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaymentDto> CreatePaymentAsync(PaymentDto paymentDto)
    {
        var payment = _mapper.Map<Payment>(paymentDto);
        payment.Id = Guid.NewGuid().ToString();
        await _unitOfWork.Payments.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();
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
}
