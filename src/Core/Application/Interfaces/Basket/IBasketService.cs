namespace Application.Interfaces;

/// <summary>
/// Manages a customer's active basket in Redis. Basket data is never persisted
/// to the application database.
/// </summary>
public interface IBasketService
{
    Task<BasketDto> GetBasketAsync(string ownerId);
    Task<BasketDto> AddItemToBasketAsync(string ownerId, long productId, int quantity);
    Task<BasketDto> UpdateItemQuantityAsync(string ownerId, long productId, int quantity);
    Task<BasketDto> RemoveItemFromBasketAsync(string ownerId, long productId);
    Task ClearBasketAsync(string ownerId);
}
