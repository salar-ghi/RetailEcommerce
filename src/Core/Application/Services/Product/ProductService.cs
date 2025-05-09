using Domain.Entities;

namespace Application.Services;

public class ProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto> GetProductByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        return product != null ? _mapper.Map<ProductDto>(product) : throw new KeyNotFoundException($"Product with ID {id} not found.");
    }

    public async Task AddProductAsync(ProductDto productDto)
    {
        var product = _mapper.Map<Product>(productDto);
        product.CreatedBy = "bdfb65f1-9024-4736-846d-df7de909f571";
        product.ModifiedBy = "bdfb65f1-9024-4736-846d-df7de909f571";
        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateProductAsync(ProductDto productDto)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productDto.Id);
        if (product != null)
        {
            _mapper.Map(productDto, product);
            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            throw new KeyNotFoundException($"Product with ID {productDto.Id} not found.");
        }
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product != null)
        {
            await _unitOfWork.Products.DeleteAsync(product);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        var products = await _unitOfWork.Products.SearchProductsAsync(searchTerm);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsByNameAsync(string name, int page = 1, int pageSize = 10)
    {
        var products = await _unitOfWork.Products.SearchByNameAsync(name);
        return _mapper.Map<IEnumerable<ProductDto>>(products.Skip((page - 1) * pageSize).Take(pageSize));
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        var products = await _unitOfWork.Products.GetProductsByPriceRangeAsync(minPrice, maxPrice);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsByCategoryAsync(int categoryId)
    {
        var products = await _unitOfWork.Products.GetProductsByCategoryAsync(categoryId);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }
}