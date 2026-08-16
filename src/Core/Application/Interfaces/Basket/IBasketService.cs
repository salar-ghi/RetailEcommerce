namespace Application.Interfaces;

public interface IBasketService
{
    Task<IEnumerable<BasketDto>> ListBasketsAsync(string? status = null, string? userId = null);
    Task<BasketDto> GetBasketByIdAsync(string basketId);
    Task<BasketDto> GetBasketAsync(string userId);
    Task AddItemToBasketAsync(string userId, int productId, int quantity);
    Task UpdateItemQuantityAsync(string userId, int productId, int quantity);
    Task RemoveItemFromBasketAsync(string userId, int productId);
    Task ClearBasketAsync(string userId);
    Task<BasketDto> UpdateBasketAsync(string basketId, UpdateBasketRequest request);
    Task<BasketActionResultDto> DeleteBasketAsync(string basketId);
    Task<BasketActionResultDto> ConvertBasketAsync(string basketId);
    Task<BasketActionResultDto> RemindBasketOwnerAsync(string basketId);
}
