namespace Infrastructure.Repositories;

public class BasketRepository : Repository<Basket, string>, IBasketRepository
{
    public BasketRepository(AppDbContext context) : base(context) { }

    public async Task AddItemAsync(Basket basket, Guid productId, int quantity)
    {
        ArgumentNullException.ThrowIfNull(basket);
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        if (!long.TryParse(productId.ToString("N")[..12], System.Globalization.NumberStyles.HexNumber, null, out var numericProductId))
            throw new ArgumentException("Product id could not be mapped to the current numeric product key.", nameof(productId));

        var product = await _context.Products.FindAsync(numericProductId) ?? throw new KeyNotFoundException($"Product with ID {numericProductId} not found.");
        var existingItem = basket.Items.FirstOrDefault(i => i.ProductId == numericProductId);
        if (existingItem is null)
        {
            basket.Items.Add(new BasketItem { BasketId = basket.Id, ProductId = numericProductId, Quantity = quantity, UnitPrice = product.Batches?.FirstOrDefault()?.SellingPrice ?? 0m });
        }
        else
        {
            existingItem.Quantity += quantity;
        }
        basket.ModifiedTime = DateTime.UtcNow;
        _context.Baskets.Update(basket);
    }

    public Task ApplyPromotionAsync(Basket basket, string promotionCode)
    {
        ArgumentNullException.ThrowIfNull(basket);
        foreach (var item in basket.Items)
        {
            item.AppliedPromotionCode = promotionCode;
        }
        basket.ModifiedTime = DateTime.UtcNow;
        _context.Baskets.Update(basket);
        return Task.CompletedTask;
    }

    public async Task<Basket> GetByUserIdAsync(string userId)
    {
        return await QueryBasket()
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.UserId == userId);
    }

    public async Task<Basket> GetByUserIdAsync(string userId, string? guestId)
    {
        return await QueryBasket()
            .FirstOrDefaultAsync(b => (!string.IsNullOrWhiteSpace(userId) && b.UserId == userId) || (!string.IsNullOrWhiteSpace(guestId) && b.GuestId == guestId));
    }

    public async Task MergeBasketsAsync(string userBasketId, string guestBasketId)
    {
        var userBasket = await QueryBasket().FirstOrDefaultAsync(b => b.Id == userBasketId) ?? throw new KeyNotFoundException($"Basket {userBasketId} not found.");
        var guestBasket = await QueryBasket().FirstOrDefaultAsync(b => b.Id == guestBasketId) ?? throw new KeyNotFoundException($"Basket {guestBasketId} not found.");

        foreach (var guestItem in guestBasket.Items.ToList())
        {
            var userItem = userBasket.Items.FirstOrDefault(i => i.ProductId == guestItem.ProductId);
            if (userItem is null)
            {
                userBasket.Items.Add(new BasketItem { BasketId = userBasket.Id, ProductId = guestItem.ProductId, Quantity = guestItem.Quantity, UnitPrice = guestItem.UnitPrice, DiscountedPrice = guestItem.DiscountedPrice, AppliedPromotionCode = guestItem.AppliedPromotionCode });
            }
            else
            {
                userItem.Quantity += guestItem.Quantity;
            }
        }

        userBasket.ModifiedTime = DateTime.UtcNow;
        _context.Baskets.Remove(guestBasket);
        _context.Baskets.Update(userBasket);
    }

    public Task RemoveItemAsync(Basket basket, Guid basketItemId)
    {
        ArgumentNullException.ThrowIfNull(basket);
        var item = basket.Items.FirstOrDefault(i => i.Id.ToString() == basketItemId.ToString() || Guid.TryParse(i.Id.ToString(), out var parsed) && parsed == basketItemId);
        if (item is not null)
        {
            basket.Items.Remove(item);
            _context.BasketItems.Remove(item);
            basket.ModifiedTime = DateTime.UtcNow;
            _context.Baskets.Update(basket);
        }
        return Task.CompletedTask;
    }

    public Task UpdateItemQuantityAsync(Basket basket, Guid basketItemId, int newQuantity)
    {
        ArgumentNullException.ThrowIfNull(basket);
        if (newQuantity < 0) throw new ArgumentOutOfRangeException(nameof(newQuantity), "Quantity cannot be negative.");
        var item = basket.Items.FirstOrDefault(i => i.Id.ToString() == basketItemId.ToString() || Guid.TryParse(i.Id.ToString(), out var parsed) && parsed == basketItemId);
        if (item is null) throw new KeyNotFoundException($"Basket item {basketItemId} not found.");
        if (newQuantity == 0) basket.Items.Remove(item); else item.Quantity = newQuantity;
        basket.ModifiedTime = DateTime.UtcNow;
        _context.Baskets.Update(basket);
        return Task.CompletedTask;
    }

    private IQueryable<Basket> QueryBasket() => _context.Baskets
        .Include(b => b.User)
        .Include(b => b.Items)
            .ThenInclude(i => i.Product);
}
