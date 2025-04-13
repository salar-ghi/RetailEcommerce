namespace Application.Services;

public class BasketService : IBasketService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRedisCacheService _cacheService;
    private readonly IMapper _mapper;

    public BasketService(IUnitOfWork unitOfWork, IRedisCacheService cacheService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<BasketDto> GetBasketAsync(string userId)
    {
        var cachedBasket = await _cacheService.GetCachedDataAsync<Basket>(userId);
        if (cachedBasket != null)
        {
            return _mapper.Map<BasketDto>(cachedBasket);
        }

        var basket = await _unitOfWork.Baskets.GetByUserIdAsync(userId, null);
        if (basket == null)
        {
            basket = new Basket { Id = userId, UserId = userId };
            await _unitOfWork.Baskets.AddAsync(basket);
            await _unitOfWork.SaveChangesAsync();
        }

        await _cacheService.SetCachedDataAsync<Basket>(basket.Id, basket, TimeSpan.FromDays(1));
        return _mapper.Map<BasketDto>(basket);
    }

    public async Task AddItemToBasketAsync(string userId, int productId, int quantity)
    {
        var basket = await GetBasketFromCacheOrDbAsync(userId);
        var product = await _unitOfWork.Products.GetByIdAsync(productId);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {productId} not found.");
        }

        var existingItem = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            basket.Items.Add(new BasketItem
            {
                ProductId = productId,
                Quantity = quantity,
                UnitPrice= product.Price,
                BasketId = basket.Id
            });
        }

        await _unitOfWork.Baskets.UpdateAsync(basket);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.SetCachedDataAsync<Basket>(basket.Id, basket, TimeSpan.FromDays(1));
    }

    public async Task UpdateItemQuantityAsync(string userId, int productId, int quantity)
    {
        var basket = await GetBasketFromCacheOrDbAsync(userId);
        var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
        {
            throw new KeyNotFoundException($"Item with Product ID {productId} not found.");
        }

        item.Quantity = quantity;
        await _unitOfWork.Baskets.UpdateAsync(basket);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.SetCachedDataAsync<Basket>(basket.Id, basket, TimeSpan.FromDays(1));
    }

    public async Task RemoveItemFromBasketAsync(string userId, int productId)
    {
        var basket = await GetBasketFromCacheOrDbAsync(userId);
        var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            basket.Items.Remove(item);
            await _unitOfWork.Baskets.UpdateAsync(basket);
            await _unitOfWork.SaveChangesAsync();
            await _cacheService.SetCachedDataAsync<Basket>(basket.Id, basket, TimeSpan.FromDays(1));
        }
    }

    public async Task ClearBasketAsync(string userId)
    {
        var basket = await _unitOfWork.Baskets.GetByUserIdAsync(userId, string.Empty);
        if (basket != null)
        {
            await _unitOfWork.Baskets.DeleteAsync(basket);
            await _unitOfWork.SaveChangesAsync();
            await _cacheService.RemoveCachedDataAsync(basket.Id);
        }
    }

    private async Task<Basket> GetBasketFromCacheOrDbAsync(string userId)
    {
        var cachedBasket = await _cacheService.GetCachedDataAsync<Basket>(userId);
        if (cachedBasket != null)
        {
            return cachedBasket;
        }

        var basket = await _unitOfWork.Baskets.GetByUserIdAsync(userId, string.Empty);
        if (basket == null)
        {
            basket = new Basket { Id = userId, UserId = userId };
            await _unitOfWork.Baskets.AddAsync(basket);
            await _unitOfWork.SaveChangesAsync();
        }

        await _cacheService.SetCachedDataAsync<Basket>(basket.Id, basket, TimeSpan.FromDays(1));
        return basket;
    }
}