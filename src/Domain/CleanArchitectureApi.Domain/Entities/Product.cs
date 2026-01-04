using CleanArchitectureApi.Domain.Common;
using CleanArchitectureApi.Domain.Events;
using CleanArchitectureApi.Domain.ValueObjects;

namespace CleanArchitectureApi.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Money Price { get; private set; } = null!;
    public string Sku { get; private set; } = string.Empty;
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string Category { get; private set; } = string.Empty;
    public List<string> Tags { get; private set; } = new();
    
    // Foreign key
    public Guid UserId { get; private set; }
    
    // Navigation properties
    public virtual User User { get; private set; } = null!;

    private Product() { } // EF Core constructor

    public Product(string name, string description, Money price, string sku, int stockQuantity, string category, Guid userId)
    {
        Name = name;
        Description = description;
        Price = price;
        Sku = sku;
        StockQuantity = stockQuantity;
        Category = category;
        UserId = userId;
        
        AddDomainEvent(new ProductCreatedEvent(this));
    }

    public void UpdateDetails(string name, string description, string category)
    {
        Name = name;
        Description = description;
        Category = category;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new ProductUpdatedEvent(this));
    }

    public void UpdatePrice(Money newPrice)
    {
        var oldPrice = Price;
        Price = newPrice;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new ProductPriceChangedEvent(this, oldPrice, newPrice));
    }

    public void UpdateStock(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(quantity));
            
        var oldQuantity = StockQuantity;
        StockQuantity = quantity;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new ProductStockUpdatedEvent(this, oldQuantity, quantity));
    }

    public void ReduceStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));
            
        if (StockQuantity < quantity)
            throw new InvalidOperationException("Insufficient stock");
            
        var oldQuantity = StockQuantity;
        StockQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new ProductStockReducedEvent(this, oldQuantity, StockQuantity, quantity));
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));
            
        var oldQuantity = StockQuantity;
        StockQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new ProductStockAddedEvent(this, oldQuantity, StockQuantity, quantity));
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new ProductActivatedEvent(this));
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new ProductDeactivatedEvent(this));
    }

    public void AddTag(string tag)
    {
        if (!Tags.Contains(tag))
        {
            Tags.Add(tag);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveTag(string tag)
    {
        if (Tags.Contains(tag))
        {
            Tags.Remove(tag);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public bool IsInStock() => StockQuantity > 0;
    public bool IsLowStock(int threshold = 10) => StockQuantity <= threshold;
}

