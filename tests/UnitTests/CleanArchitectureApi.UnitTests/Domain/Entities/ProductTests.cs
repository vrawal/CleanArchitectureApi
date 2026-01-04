using CleanArchitectureApi.Domain.Entities;
using CleanArchitectureApi.Domain.Events;
using CleanArchitectureApi.Domain.ValueObjects;
using FluentAssertions;

namespace CleanArchitectureApi.UnitTests.Domain.Entities;

public class ProductTests
{
    [Fact]
    public void Constructor_ShouldCreateProduct_WithValidParameters()
    {
        // Arrange
        var name = "Test Product";
        var description = "Test Description";
        var price = new Money(99.99m, "USD");
        var sku = "TEST-001";
        var stockQuantity = 10;
        var category = "Electronics";
        var userId = Guid.NewGuid();

        // Act
        var product = new Product(name, description, price, sku, stockQuantity, category, userId);

        // Assert
        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.Price.Should().Be(price);
        product.Sku.Should().Be(sku);
        product.StockQuantity.Should().Be(stockQuantity);
        product.Category.Should().Be(category);
        product.UserId.Should().Be(userId);
        product.IsActive.Should().BeTrue();
        product.Tags.Should().BeEmpty();
        product.DomainEvents.Should().ContainSingle(e => e is ProductCreatedEvent);
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateProductDetails_AndRaiseDomainEvent()
    {
        // Arrange
        var product = CreateTestProduct();
        var newName = "Updated Product";
        var newDescription = "Updated Description";
        var newCategory = "Updated Category";

        // Act
        product.UpdateDetails(newName, newDescription, newCategory);

        // Assert
        product.Name.Should().Be(newName);
        product.Description.Should().Be(newDescription);
        product.Category.Should().Be(newCategory);
        product.UpdatedAt.Should().NotBeNull();
        product.DomainEvents.Should().Contain(e => e is ProductUpdatedEvent);
    }

    [Fact]
    public void UpdatePrice_ShouldUpdatePrice_AndRaiseDomainEvent()
    {
        // Arrange
        var product = CreateTestProduct();
        var oldPrice = product.Price;
        var newPrice = new Money(149.99m, "USD");

        // Act
        product.UpdatePrice(newPrice);

        // Assert
        product.Price.Should().Be(newPrice);
        product.UpdatedAt.Should().NotBeNull();
        
        var priceChangedEvent = product.DomainEvents.OfType<ProductPriceChangedEvent>().FirstOrDefault();
        priceChangedEvent.Should().NotBeNull();
        priceChangedEvent!.OldPrice.Should().Be(oldPrice);
        priceChangedEvent.NewPrice.Should().Be(newPrice);
    }

    [Fact]
    public void UpdateStock_ShouldUpdateStock_AndRaiseDomainEvent()
    {
        // Arrange
        var product = CreateTestProduct();
        var oldQuantity = product.StockQuantity;
        var newQuantity = 25;

        // Act
        product.UpdateStock(newQuantity);

        // Assert
        product.StockQuantity.Should().Be(newQuantity);
        product.UpdatedAt.Should().NotBeNull();
        
        var stockUpdatedEvent = product.DomainEvents.OfType<ProductStockUpdatedEvent>().FirstOrDefault();
        stockUpdatedEvent.Should().NotBeNull();
        stockUpdatedEvent!.OldQuantity.Should().Be(oldQuantity);
        stockUpdatedEvent.NewQuantity.Should().Be(newQuantity);
    }

    [Fact]
    public void UpdateStock_WithNegativeQuantity_ShouldThrowException()
    {
        // Arrange
        var product = CreateTestProduct();

        // Act & Assert
        var act = () => product.UpdateStock(-1);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Stock quantity cannot be negative*");
    }

    [Fact]
    public void ReduceStock_ShouldReduceStock_AndRaiseDomainEvent()
    {
        // Arrange
        var product = CreateTestProduct();
        var oldQuantity = product.StockQuantity;
        var reduceBy = 5;

        // Act
        product.ReduceStock(reduceBy);

        // Assert
        product.StockQuantity.Should().Be(oldQuantity - reduceBy);
        product.UpdatedAt.Should().NotBeNull();
        
        var stockReducedEvent = product.DomainEvents.OfType<ProductStockReducedEvent>().FirstOrDefault();
        stockReducedEvent.Should().NotBeNull();
        stockReducedEvent!.OldQuantity.Should().Be(oldQuantity);
        stockReducedEvent.NewQuantity.Should().Be(oldQuantity - reduceBy);
        stockReducedEvent.ReducedBy.Should().Be(reduceBy);
    }

    [Fact]
    public void ReduceStock_WithInsufficientStock_ShouldThrowException()
    {
        // Arrange
        var product = CreateTestProduct();
        var reduceBy = product.StockQuantity + 1;

        // Act & Assert
        var act = () => product.ReduceStock(reduceBy);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Insufficient stock");
    }

    [Fact]
    public void ReduceStock_WithZeroOrNegativeQuantity_ShouldThrowException()
    {
        // Arrange
        var product = CreateTestProduct();

        // Act & Assert
        var act = () => product.ReduceStock(0);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Quantity must be positive*");
    }

    [Fact]
    public void AddStock_ShouldAddStock_AndRaiseDomainEvent()
    {
        // Arrange
        var product = CreateTestProduct();
        var oldQuantity = product.StockQuantity;
        var addQuantity = 15;

        // Act
        product.AddStock(addQuantity);

        // Assert
        product.StockQuantity.Should().Be(oldQuantity + addQuantity);
        product.UpdatedAt.Should().NotBeNull();
        
        var stockAddedEvent = product.DomainEvents.OfType<ProductStockAddedEvent>().FirstOrDefault();
        stockAddedEvent.Should().NotBeNull();
        stockAddedEvent!.OldQuantity.Should().Be(oldQuantity);
        stockAddedEvent.NewQuantity.Should().Be(oldQuantity + addQuantity);
        stockAddedEvent.AddedQuantity.Should().Be(addQuantity);
    }

    [Fact]
    public void AddStock_WithZeroOrNegativeQuantity_ShouldThrowException()
    {
        // Arrange
        var product = CreateTestProduct();

        // Act & Assert
        var act = () => product.AddStock(-1);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Quantity must be positive*");
    }

    [Fact]
    public void Activate_ShouldActivateProduct_AndRaiseDomainEvent()
    {
        // Arrange
        var product = CreateTestProduct();
        product.Deactivate();

        // Act
        product.Activate();

        // Assert
        product.IsActive.Should().BeTrue();
        product.UpdatedAt.Should().NotBeNull();
        product.DomainEvents.Should().Contain(e => e is ProductActivatedEvent);
    }

    [Fact]
    public void Deactivate_ShouldDeactivateProduct_AndRaiseDomainEvent()
    {
        // Arrange
        var product = CreateTestProduct();

        // Act
        product.Deactivate();

        // Assert
        product.IsActive.Should().BeFalse();
        product.UpdatedAt.Should().NotBeNull();
        product.DomainEvents.Should().Contain(e => e is ProductDeactivatedEvent);
    }

    [Fact]
    public void AddTag_ShouldAddTag_WhenNotExists()
    {
        // Arrange
        var product = CreateTestProduct();
        var tag = "bestseller";

        // Act
        product.AddTag(tag);

        // Assert
        product.Tags.Should().Contain(tag);
        product.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void AddTag_ShouldNotAddTag_WhenAlreadyExists()
    {
        // Arrange
        var product = CreateTestProduct();
        var tag = "bestseller";
        product.AddTag(tag);
        var initialCount = product.Tags.Count;

        // Act
        product.AddTag(tag);

        // Assert
        product.Tags.Should().HaveCount(initialCount);
        product.Tags.Should().ContainSingle(t => t == tag);
    }

    [Fact]
    public void RemoveTag_ShouldRemoveTag_WhenExists()
    {
        // Arrange
        var product = CreateTestProduct();
        var tag = "bestseller";
        product.AddTag(tag);

        // Act
        product.RemoveTag(tag);

        // Assert
        product.Tags.Should().NotContain(tag);
        product.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void IsInStock_ShouldReturnTrue_WhenStockGreaterThanZero()
    {
        // Arrange
        var product = CreateTestProduct();

        // Act
        var result = product.IsInStock();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsInStock_ShouldReturnFalse_WhenStockIsZero()
    {
        // Arrange
        var product = CreateTestProduct();
        product.UpdateStock(0);

        // Act
        var result = product.IsInStock();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsLowStock_ShouldReturnTrue_WhenStockBelowThreshold()
    {
        // Arrange
        var product = CreateTestProduct();
        product.UpdateStock(5);
        var threshold = 10;

        // Act
        var result = product.IsLowStock(threshold);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsLowStock_ShouldReturnFalse_WhenStockAboveThreshold()
    {
        // Arrange
        var product = CreateTestProduct();
        product.UpdateStock(15);
        var threshold = 10;

        // Act
        var result = product.IsLowStock(threshold);

        // Assert
        result.Should().BeFalse();
    }

    private static Product CreateTestProduct()
    {
        return new Product(
            "Test Product",
            "Test Description",
            new Money(99.99m, "USD"),
            "TEST-001",
            10,
            "Electronics",
            Guid.NewGuid());
    }
}

