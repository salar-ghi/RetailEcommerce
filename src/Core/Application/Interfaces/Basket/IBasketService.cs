namespace Application.Interfaces;

public interface IBasketService
{
    Task<BasketDto> GetBasketAsync(string userId);
    Task AddItemToBasketAsync(string userId, int productId, int quantity);
    Task UpdateItemQuantityAsync(string userId, int productId, int quantity);
    Task RemoveItemFromBasketAsync(string userId, int productId);
    Task ClearBasketAsync(string userId);
}
