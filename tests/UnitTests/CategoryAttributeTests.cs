using System.Diagnostics;
using Application.DTOs;
using Application.Helper;
using Application.Interfaces;
using Application.Mapping;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace UnitTests;

public class CategoryAttributeTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly UnitOfWork _unitOfWork;
    private readonly CategoryService _categoryService;
    private readonly CategoryAttributeService _categoryAttributeService;
    private readonly AttributeDefinitionService _attributeDefinitionService;
    private readonly IMapper _mapper;

    public CategoryAttributeTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);

        var mockCache = new Mock<IDistributedCache>();
        _unitOfWork = new UnitOfWork(_dbContext, mockCache.Object);

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CategoryMappingProfile>();
            cfg.AddProfile<ProductMappingProfile>();
            cfg.AddProfile<BrandMappingProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        var mockImageHelper = new Mock<IImageHelper>();
        var mockCurrentUserService = new Mock<ICurrentUserService>();

        _categoryAttributeService = new CategoryAttributeService(_unitOfWork, _mapper);
        _categoryService = new CategoryService(_unitOfWork, mockCurrentUserService.Object, _mapper, mockImageHelper.Object, null, _categoryAttributeService);
        _attributeDefinitionService = new AttributeDefinitionService(_unitOfWork);
    }

    [Fact]
    public async Task Category_Navigation_HasCategoryAttributeDefinitions()
    {
        var category = new Category { Name = "Electronics", Description = "Devices" };
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        var attributeDef = new AttributeDefinition
        {
            Code = "brand_attr",
            Name = "Brand Attribute",
            DataType = AttributeDataType.Select,
            IsFilterable = true,
            IsSearchable = true
        };
        _dbContext.AttributeDefinitions.Add(attributeDef);
        await _dbContext.SaveChangesAsync();

        var categoryAttrDef = new CategoryAttributeDefinition
        {
            CategoryId = category.Id,
            AttributeDefinitionId = attributeDef.Id,
            IsFilterable = true,
            SortOrder = 1
        };
        _dbContext.CategoryAttributeDefinitions.Add(categoryAttrDef);
        await _dbContext.SaveChangesAsync();

        var fetchedCategory = await _dbContext.Categories
            .Include(c => c.CategoryAttributeDefinitions)
            .FirstOrDefaultAsync(c => c.Id == category.Id);

        Assert.NotNull(fetchedCategory);
        Assert.Single(fetchedCategory.CategoryAttributeDefinitions);
        Assert.Equal(attributeDef.Id, fetchedCategory.CategoryAttributeDefinitions.First().AttributeDefinitionId);
    }

    [Fact]
    public async Task GetCategoryPageDataAsync_ReturnsFilterableAttributeDefinitionsAndOptions()
    {
        var category = new Category { Name = "Laptops", Description = "Laptops category" };
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        var attributeDef = new AttributeDefinition
        {
            Code = "ram",
            Name = "RAM Size",
            DataType = AttributeDataType.Select,
            IsFilterable = true,
            IsSearchable = true,
            IsVariantAttribute = true,
            Options = new List<AttributeOption>
            {
                new AttributeOption { Value = "8GB", Label = "8 GB", SortOrder = 1, IsActive = true },
                new AttributeOption { Value = "16GB", Label = "16 GB", SortOrder = 2, IsActive = true }
            }
        };
        _dbContext.AttributeDefinitions.Add(attributeDef);
        await _dbContext.SaveChangesAsync();

        var categoryAttr = new CategoryAttributeDefinition
        {
            CategoryId = category.Id,
            AttributeDefinitionId = attributeDef.Id,
            IsFilterable = true,
            SortOrder = 1
        };
        _dbContext.CategoryAttributeDefinitions.Add(categoryAttr);
        await _dbContext.SaveChangesAsync();

        var pageData = await _categoryService.GetCategoryPageDataAsync("Laptops");

        Assert.NotNull(pageData);
        Assert.NotNull(pageData.Category);
        Assert.Equal("Laptops", pageData.Category.Name);
        Assert.Single(pageData.AttributeDefinitions);

        var attrDto = pageData.AttributeDefinitions.First();
        Assert.Equal(categoryAttr.Id, attrDto.Id);
        Assert.NotNull(attrDto.AttributeDefinition);
        Assert.Equal("ram", attrDto.AttributeDefinition.Code);
        Assert.True(attrDto.AttributeDefinition.IsVariantAttribute);
        Assert.True(attrDto.AttributeDefinition.IsFilterable);
        Assert.Equal(2, attrDto.AttributeDefinition.Options.Count);
        Assert.All(attrDto.AttributeDefinition.Options, opt => Assert.Equal(attributeDef.Id, opt.AttributeDefinitionId));
    }

    [Fact]
    public async Task CreateAttributeDefinition_WithCategoryId_AssignsCategoryAttribute()
    {
        var category = new Category { Name = "Smartphones", Description = "Mobiles" };
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        var request = new UpsertAttributeDefinitionDto
        {
            Code = "screen_size",
            Name = "Screen Size",
            DataType = AttributeDataType.String,
            IsFilterable = true,
            IsSearchable = true,
            CategoryId = category.Id,
            Options = new List<AttributeOptionDto>
            {
                new AttributeOptionDto { Value = "6.1 inch", Label = "6.1 inch" }
            }
        };

        var created = await _attributeDefinitionService.CreateAsync(request);

        Assert.NotNull(created);
        Assert.Equal("screen_size", created.Code);

        var assignments = await _categoryAttributeService.GetCategoryAttributeDefinitionsAsync(category.Id);
        Assert.Single(assignments);
        var assignment = assignments.First();
        Assert.Equal(created.Id, assignment.AttributeDefinitionId);
        Assert.True(assignment.IsFilterable);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}
