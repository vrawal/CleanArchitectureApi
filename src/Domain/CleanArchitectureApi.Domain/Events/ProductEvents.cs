using CleanArchitectureApi.Domain.Common;
using CleanArchitectureApi.Domain.Entities;
using CleanArchitectureApi.Domain.ValueObjects;

namespace CleanArchitectureApi.Domain.Events;

public class ProductCreatedEvent : BaseDomainEvent
{
    public Product Product { get; }

    public ProductCreatedEvent(Product product)
    {
        Product = product;
    }
}

public class ProductUpdatedEvent : BaseDomainEvent
{
    public Product Product { get; }

    public ProductUpdatedEvent(Product product)
    {
        Product = product;
    }
}

public class ProductPriceChangedEvent : BaseDomainEvent
{
    public Product Product { get; }
    public Money OldPrice { get; }
    public Money NewPrice { get; }

    public ProductPriceChangedEvent(Product product, Money oldPrice, Money newPrice)
    {
        Product = product;
        OldPrice = oldPrice;
        NewPrice = newPrice;
    }
}

public class ProductStockUpdatedEvent : BaseDomainEvent
{
    public Product Product { get; }
    public int OldQuantity { get; }
    public int NewQuantity { get; }

    public ProductStockUpdatedEvent(Product product, int oldQuantity, int newQuantity)
    {
        Product = product;
        OldQuantity = oldQuantity;
        NewQuantity = newQuantity;
    }
}

public class ProductStockReducedEvent : BaseDomainEvent
{
    public Product Product { get; }
    public int OldQuantity { get; }
    public int NewQuantity { get; }
    public int ReducedBy { get; }

    public ProductStockReducedEvent(Product product, int oldQuantity, int newQuantity, int reducedBy)
    {
        Product = product;
        OldQuantity = oldQuantity;
        NewQuantity = newQuantity;
        ReducedBy = reducedBy;
    }
}

public class ProductStockAddedEvent : BaseDomainEvent
{
    public Product Product { get; }
    public int OldQuantity { get; }
    public int NewQuantity { get; }
    public int AddedQuantity { get; }

    public ProductStockAddedEvent(Product product, int oldQuantity, int newQuantity, int addedQuantity)
    {
        Product = product;
        OldQuantity = oldQuantity;
        NewQuantity = newQuantity;
        AddedQuantity = addedQuantity;
    }
}

public class ProductActivatedEvent : BaseDomainEvent
{
    public Product Product { get; }

    public ProductActivatedEvent(Product product)
    {
        Product = product;
    }
}

public class ProductDeactivatedEvent : BaseDomainEvent
{
    public Product Product { get; }

    public ProductDeactivatedEvent(Product product)
    {
        Product = product;
    }
}

