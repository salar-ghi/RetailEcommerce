namespace Domain.IRepositories;

public interface IBasketItemRepository : IRepository<BasketItem, int>
{
    Task<IEnumerable<BasketItem>> GetByBasketIdAsync(string basketId);
}
