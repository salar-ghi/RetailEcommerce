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

public class OrderFinanceServiceTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly UnitOfWork _unitOfWork;
    private readonly FinanceService _financeService;
    private readonly OrderService _orderService;
    private readonly ITestOutputHelper _output;

    public OrderFinanceServiceTests(ITestOutputHelper output)
    {
        _output = output;

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);

        var mockCache = new Mock<IDistributedCache>();
        _unitOfWork = new UnitOfWork(_dbContext, mockCache.Object);

        _financeService = new FinanceService(_unitOfWork);

        var mockBasket = new Mock<IBasketService>();
        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(m => m.Map<ShippingAddress>(It.IsAny<ShippingAddressDto>()))
            .Returns((ShippingAddressDto dto) => new ShippingAddress
            {
                AddressLine1 = dto.AddressLine1 ?? "",
                Street = dto.Street ?? "",
                City = dto.City ?? "",
                State = dto.State ?? "",
                PostalCode = dto.PostalCode ?? "",
                Country = dto.Country ?? ""
            });

        var mockRedis = new Mock<IRedisCacheService>();

        _orderService = new OrderService(_unitOfWork, mockBasket.Object, mockMapper.Object, mockRedis.Object, _financeService);
    }

    [Fact]
    public async Task CreateManualOrder_WithMultiplePaymentSplits_BatchSyncsToFinanceCorrectly()
    {
        // Arrange
        await _financeService.EnsureDefaultsAsync();

        var customer = new User
        {
            Id = "cust-100",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PhoneNumber = "1234567890",
            Username = "johndoe",
            PasswordHash = "",
            IsActive = true
        };
        _dbContext.Users.Add(customer);

        var product = new Product
        {
            Id = 1L,
            Name = "Test Item",
            Description = "Desc",
            StorageLocationNote = "",
            IsActive = true
        };
        _dbContext.Products.Add(product);

        _dbContext.ProductStocks.Add(new ProductStock
        {
            Id = 1L,
            ProductId = 1L,
            Quantity = 100,
            RowVersion = new byte[] { 0, 0, 0, 1 },
            CreatedTime = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        var request = new CreateManualOrderRequest
        {
            CustomerId = customer.Id,
            Customer = customer.FirstName,
            DiscountAmount = 0m,
            FinalTotal = 300m,
            Notes = "",
            ShippingAddress = new ShippingAddressDto
            {
                AddressLine1 = "123 Main St",
                Street = "123 Main St",
                City = "Tehran",
                State = "Tehran",
                PostalCode = "12345",
                Country = "Iran"
            },
            Items = new List<CreateOrderItemRequest>
            {
                new CreateOrderItemRequest { ProductId = 1L, Quantity = 3, UnitPrice = 100m, TotalPrice = 300m }
            },
            Payments = new List<OrderPaymentSplitDto>
            {
                new OrderPaymentSplitDto { Amount = 100m, Method = "cash", Status = PaymentStatus.Completed, FinanceAccountId = "default-cash", BranchId = "default-branch" },
                new OrderPaymentSplitDto { Amount = 100m, Method = "card", Status = PaymentStatus.Completed, FinanceAccountId = "default-cash", BranchId = "default-branch" },
                new OrderPaymentSplitDto { Amount = 100m, Method = "bank_transfer", Status = PaymentStatus.Completed, FinanceAccountId = "default-cash", BranchId = "default-branch" }
            }
        };

        // Act
        var result = await _orderService.CreateManualOrderAsync(request);

        // Assert
        var transactions = await _financeService.GetTransactionsAsync();
        Assert.Equal(3, transactions.Count);
        Assert.All(transactions, tx => Assert.Equal(FinancialTransactionStatus.Completed, tx.Status));
        Assert.All(transactions, tx => Assert.NotNull(tx.JournalEntryId));
    }

    [Fact]
    public async Task RecordOrderPaymentsAsync_BenchmarkBatchVsSequential()
    {
        // Arrange
        await _financeService.EnsureDefaultsAsync();

        var product = new Product
        {
            Id = 2L,
            Name = "Test Item 2",
            Description = "Desc",
            StorageLocationNote = "",
            IsActive = true
        };
        _dbContext.Products.Add(product);

        var orderId = Guid.NewGuid().ToString();
        var order = new Order
        {
            Id = orderId,
            CustomerId = "cust-10",
            CreatedTime = DateTime.UtcNow,
            RowVersion = new byte[] { 0, 0, 0, 1 },
            ShippingAddress = new ShippingAddress
            {
                AddressLine1 = "123 Main St",
                Street = "123 Main St",
                City = "Tehran",
                State = "Tehran",
                PostalCode = "12345",
                Country = "Iran"
            },
            Items = new List<OrderItem>
            {
                new OrderItem { ProductId = 2L, Quantity = 5, UnitPrice = 100m }
            }
        };
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        int splitCount = 20;
        var requests = Enumerable.Range(1, splitCount).Select(i => new RecordOrderFinanceDto
        {
            OrderId = order.Id,
            FinanceAccountId = "default-cash",
            BranchId = "default-branch",
            PaymentMethod = FinancePaymentMethod.Cash,
            CounterpartyId = order.CustomerId
        }).ToList();

        // Benchmark Batch Execution
        var sw = Stopwatch.StartNew();
        var batchResults = await _financeService.RecordOrderPaymentsAsync(requests);
        sw.Stop();
        var batchTimeMs = sw.ElapsedMilliseconds;

        _output.WriteLine($"Batch RecordOrderPaymentsAsync for {splitCount} splits took: {batchTimeMs} ms ({sw.Elapsed.TotalMicroseconds} us)");

        Assert.Equal(splitCount, batchResults.Count);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}
