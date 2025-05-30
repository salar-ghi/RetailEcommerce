namespace Application.Interfaces;

public interface ISupplierService
{
    Task<SupplierDto> RegisterSupplierAsync(SupplierRegistrationDto request, string userId = null);
    Task<SupplierDto> ApproveSupplierAsync(ApproveSupplierDto request);
    Task<ProductDto> AddProductAsync(int supplierId, CreateProductDto productDto);
    Task<IEnumerable<OrderDto>> GetSupplierOrdersAsync(int supplierId);
    Task<IEnumerable<PaymentDto>> GetSupplierPaymentsAsync(int supplierId);
    Task<SupplierDto> UpdateSupplierStatusAsync(int supplierId, SupplierStatus status);
}
