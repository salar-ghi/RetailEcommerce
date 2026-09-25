using System.Diagnostics;
using Application.Configuration;
using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests;

public class OrderServiceTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly UnitOfWork _unitOfWork;
    private readonly OrderService _orderService;
    private readonly ITestOutputHelper _output;

    public OrderServiceTests(ITestOutputHelper output)
    {
        _output = output;

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);

        var mockDistributedCache = new Mock<IDistributedCache>();
        _unitOfWork = new UnitOfWork(_dbContext, mockDistributedCache.Object);

        var mockBasketService = new Mock<IBasketService>();
        var mockCacheService = new Mock<IRedisCacheService>();
        var mockFinanceService = new Mock<IFinanceService>();
        var mockMapper = new Mock<IMapper>();

        mockMapper.Setup(m => m.Map<OrderDto>(It.IsAny<Order>()))
            .Returns((Order src) => new OrderDto { Id = src.Id, Status = src.Status.ToString() });
        mockMapper.Setup(m => m.Map<IEnumerable<OrderDto>>(It.IsAny<IEnumerable<Order>>()))
            .Returns((IEnumerable<Order> src) => src.Select(o => new OrderDto { Id = o.Id, Status = o.Status.ToString() }).ToList());

        _orderService = new OrderService(
            _unitOfWork,
            mockBasketService.Object,
            mockMapper.Object,
            mockCacheService.Object,
            mockFinanceService.Object
        );
    }

    [Fact]
    public async Task DeductInventory_DeductsStock_UpdatesSpaceAndShelfAndBatch()
    {
        // Arrange
        var space1 = new StorageSpace { Id = 1, Name = "Space 1", Code = "SPC1", Address = "Addr 1", Description = "Desc 1", Used = 100, Capacity = 1000 };
        var space2 = new StorageSpace { Id = 2, Name = "Space 2", Code = "SPC2", Address = "Addr 2", Description = "Desc 2", Used = 100, Capacity = 1000 };
        var shelf1 = new Shelf { Id = 1, SpaceId = 1, Name = "Shelf 1", Code = "A1", Used = 100, Capacity = 1000 };
        var shelf2 = new Shelf { Id = 2, SpaceId = 2, Name = "Shelf 2", Code = "A2", Used = 100, Capacity = 1000 };
        var batch = new ProductInventoryBatch { Id = 1, BatchNumber = "BATCH1", Quantity = 500, SoldQuantity = 0, Currency = "USD", PricingTier = "Standard", Notes = "" };

        _dbContext.StorageSpaces.AddRange(space1, space2);
        _dbContext.Shelves.AddRange(shelf1, shelf2);
        _dbContext.InventoryBatch.Add(batch);

        var product = new Product { Id = 1, Name = "Test Product 1", Description = "Desc", IsActive = true, StorageLocationNote = "" };
        _dbContext.Products.Add(product);

        var stock1 = new ProductStock
        {
            Id = 1,
            ProductId = 1,
            Quantity = 10,
            ReservedQuantity = 0,
            SpaceId = 1,
            ShelfId = 1,
            ProductInventoryBatchId = 1,
            RowVersion = Array.Empty<byte>(),
            CreatedTime = DateTime.UtcNow.AddHours(-2)
        };
        var stock2 = new ProductStock
        {
            Id = 2,
            ProductId = 1,
            Quantity = 20,
            ReservedQuantity = 0,
            SpaceId = 2,
            ShelfId = 2,
            ProductInventoryBatchId = 1,
            RowVersion = Array.Empty<byte>(),
            CreatedTime = DateTime.UtcNow.AddHours(-1)
        };

        _dbContext.ProductStocks.AddRange(stock1, stock2);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var request = new CreateStorefrontOrderRequest
        {
            UserId = "user1",
            Items = new List<CreateStorefrontOrderItemRequest>
            {
                new() { ProductId = 1, Quantity = 15, Price = 10m }
            }
        };

        // Act
        var result = await _orderService.CreateStorefrontOrderAsync(request);

        // Assert
        Assert.NotNull(result);
        var updatedStock1 = await _dbContext.ProductStocks.FindAsync(1L);
        var updatedStock2 = await _dbContext.ProductStocks.FindAsync(2L);
        var updatedSpace1 = await _dbContext.StorageSpaces.FindAsync(1);
        var updatedSpace2 = await _dbContext.StorageSpaces.FindAsync(2);

        Assert.NotNull(updatedStock1);
        Assert.NotNull(updatedStock2);
        Assert.NotNull(updatedSpace1);
        Assert.NotNull(updatedSpace2);

        // Stock 1 (created earlier) should be completely depleted (10 - 10 = 0)
        Assert.Equal(0, updatedStock1.Quantity);
        // Stock 2 should be partially depleted (20 - 5 = 15)
        Assert.Equal(15, updatedStock2.Quantity);
        // Space 1 Used should be reduced by 10 (100 - 10 = 90)
        Assert.Equal(90, updatedSpace1.Used);
        // Space 2 Used should be reduced by 5 (100 - 5 = 95)
        Assert.Equal(95, updatedSpace2.Used);
    }

    [Fact]
    public async Task DeductInventory_ThrowsException_WhenStockIsInsufficient()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Scarce Product", Description = "Desc", IsActive = true, StorageLocationNote = "" };
        _dbContext.Products.Add(product);

        var stock = new ProductStock
        {
            Id = 1,
            ProductId = 1,
            Quantity = 5,
            ReservedQuantity = 0,
            RowVersion = Array.Empty<byte>()
        };
        _dbContext.ProductStocks.Add(stock);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var request = new CreateStorefrontOrderRequest
        {
            UserId = "user1",
            Items = new List<CreateStorefrontOrderItemRequest>
            {
                new() { ProductId = 1, Quantity = 10, Price = 10m }
            }
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _orderService.CreateStorefrontOrderAsync(request));
        Assert.Contains("Insufficient inventory for product 'Scarce Product'", ex.Message);
    }

    [Fact]
    public async Task DeductInventory_MultiItemOrder_PerformanceBenchmark()
    {
        // Seed 100 products, each with 3 stock entries across different spaces/shelves/batches
        int itemCount = 100;
        var orderItems = new List<CreateStorefrontOrderItemRequest>();

        for (int i = 1; i <= itemCount; i++)
        {
            var p = new Product { Id = i, Name = $"Product {i}", Description = "Desc", IsActive = true, StorageLocationNote = "" };
            _dbContext.Products.Add(p);

            for (int s = 1; s <= 3; s++)
            {
                int spaceId = (i - 1) * 3 + s;
                var space = new StorageSpace { Id = spaceId, Name = $"Space {spaceId}", Code = $"SPC_{spaceId}", Address = $"Addr {spaceId}", Description = "Desc", Used = 500, Capacity = 2000 };
                var shelf = new Shelf { Id = spaceId, SpaceId = spaceId, Name = $"Shelf {spaceId}", Code = $"Shelf_{spaceId}", Used = 500, Capacity = 2000 };
                var batch = new ProductInventoryBatch { Id = (long)spaceId, BatchNumber = $"BATCH_{spaceId}", Quantity = 1000, SoldQuantity = 0, Currency = "USD", PricingTier = "Standard", Notes = "" };

                _dbContext.StorageSpaces.Add(space);
                _dbContext.Shelves.Add(shelf);
                _dbContext.InventoryBatch.Add(batch);

                _dbContext.ProductStocks.Add(new ProductStock
                {
                    Id = spaceId,
                    ProductId = i,
                    Quantity = 50,
                    ReservedQuantity = 0,
                    SpaceId = spaceId,
                    ShelfId = spaceId,
                    ProductInventoryBatchId = (long)spaceId,
                    RowVersion = Array.Empty<byte>(),
                    CreatedTime = DateTime.UtcNow.AddMinutes(-s)
                });
            }

            orderItems.Add(new CreateStorefrontOrderItemRequest
            {
                ProductId = i,
                Quantity = 20,
                Price = 5.0m
            });
        }

        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var request = new CreateStorefrontOrderRequest
        {
            UserId = "benchmarkUser",
            Items = orderItems
        };

        var sw = Stopwatch.StartNew();
        var result = await _orderService.CreateStorefrontOrderAsync(request);
        sw.Stop();

        _output.WriteLine($"DeductInventoryForOrderAsync (100 items x 3 stocks = 300 stocks) took: {sw.ElapsedMilliseconds} ms ({sw.Elapsed.TotalMicroseconds} us)");

        Assert.NotNull(result);

        // Verify stock deduction across all 100 products
        for (int i = 1; i <= itemCount; i++)
        {
            var productStocks = await _dbContext.ProductStocks.Where(s => s.ProductId == i).ToListAsync();
            int totalRemaining = productStocks.Sum(s => s.Quantity);
            // Originally 150 total (3 x 50), deducted 20 => 130 remaining
            Assert.Equal(130, totalRemaining);
        }
    }

    [Fact]
    public async Task ListReturnsAsync_ReturnsOnlyReturnedAndPartiallyReturnedOrders()
    {
        // Arrange
        var customer = new User
        {
            Id = "cust1",
            Username = "cust1",
            Email = "cust1@example.com",
            PhoneNumber = "1234567890",
            FirstName = "Customer 1",
            LastName = "User",
            PasswordHash = "hash",
            IsActive = true
        };
        _dbContext.Users.Add(customer);

        var shippingAddress = new ShippingAddress
        {
            AddressLine1 = "",
            City = "",
            Country = "",
            PostalCode = "",
            State = "",
            Street = ""
        };

        var order1 = new Order
        {
            Id = "order-1",
            CustomerId = "cust1",
            Status = OrderStatus.Returned,
            ShippingAddress = shippingAddress,
            RowVersion = Array.Empty<byte>()
        };
        var order2 = new Order
        {
            Id = "order-2",
            CustomerId = "cust1",
            Status = OrderStatus.PartiallyReturned,
            ShippingAddress = shippingAddress,
            RowVersion = Array.Empty<byte>()
        };
        var order3 = new Order
        {
            Id = "order-3",
            CustomerId = "cust1",
            Status = OrderStatus.Processing,
            ShippingAddress = shippingAddress,
            RowVersion = Array.Empty<byte>()
        };

        _dbContext.Orders.AddRange(order1, order2, order3);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        // Act
        var results = (await _orderService.ListReturnsAsync()).ToList();

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Contains(results, o => o.Id == "order-1" && o.Status == OrderStatus.Returned.ToString());
        Assert.Contains(results, o => o.Id == "order-2" && o.Status == OrderStatus.PartiallyReturned.ToString());
        Assert.DoesNotContain(results, o => o.Id == "order-3");
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}
