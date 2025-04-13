namespace Domain.IRepositories;

public interface IBasketRepository : IRepository<Basket, string>
{
    Task<Basket> GetByUserIdAsync(string userId, string? guestId);

    Task AddItemAsync(Basket basket, Guid productId, int quantity);
    Task RemoveItemAsync(Basket basket, Guid basketItemId);
    Task UpdateItemQuantityAsync(Basket basket, Guid basketItemId, int newQuantity);
    Task MergeBasketsAsync(string userBasketId, string guestBasketId);
    Task ApplyPromotionAsync(Basket basket, string promotionCode);
    //Task<BasketCalculationResult> CalculateTotalsAsync(Basket basket);


    //Task<Basket> GetBasketAsync(string basketId);
    //Task AddBasketAsync(Basket basket);
    //Task UpdateBasketAsync(Basket basket);
    //Task DeleteBasketAsync(string basketId);
}
