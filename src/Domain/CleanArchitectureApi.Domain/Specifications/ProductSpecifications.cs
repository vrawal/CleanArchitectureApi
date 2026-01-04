using CleanArchitectureApi.Domain.Common;
using CleanArchitectureApi.Domain.Entities;

namespace CleanArchitectureApi.Domain.Specifications;

public class ProductBySkuSpecification : BaseSpecification<Product>
{
    public ProductBySkuSpecification(string sku) : base(p => p.Sku == sku && !p.IsDeleted)
    {
    }
}

public class ActiveProductsSpecification : BaseSpecification<Product>
{
    public ActiveProductsSpecification() : base(p => p.IsActive && !p.IsDeleted)
    {
        ApplyOrderBy(p => p.Name);
    }
}

public class ProductsByCategorySpecification : BaseSpecification<Product>
{
    public ProductsByCategorySpecification(string category) : base(p => 
        p.Category == category && p.IsActive && !p.IsDeleted)
    {
        ApplyOrderBy(p => p.Name);
    }
}

public class ProductsInStockSpecification : BaseSpecification<Product>
{
    public ProductsInStockSpecification() : base(p => 
        p.StockQuantity > 0 && p.IsActive && !p.IsDeleted)
    {
        ApplyOrderBy(p => p.Name);
    }
}

public class LowStockProductsSpecification : BaseSpecification<Product>
{
    public LowStockProductsSpecification(int threshold = 10) : base(p => 
        p.StockQuantity <= threshold && p.StockQuantity > 0 && p.IsActive && !p.IsDeleted)
    {
        ApplyOrderBy(p => p.StockQuantity);
    }
}

public class ProductsByUserSpecification : BaseSpecification<Product>
{
    public ProductsByUserSpecification(Guid userId) : base(p => 
        p.UserId == userId && !p.IsDeleted)
    {
        AddInclude(p => p.User);
        ApplyOrderByDescending(p => p.CreatedAt);
    }
}

public class ProductsByPriceRangeSpecification : BaseSpecification<Product>
{
    public ProductsByPriceRangeSpecification(decimal minPrice, decimal maxPrice, string currency) : base(p => 
        p.Price.Amount >= minPrice && p.Price.Amount <= maxPrice && 
        p.Price.Currency == currency && p.IsActive && !p.IsDeleted)
    {
        ApplyOrderBy(p => p.Price.Amount);
    }
}

public class ProductsByNameSpecification : BaseSpecification<Product>
{
    public ProductsByNameSpecification(string searchTerm) : base(p => 
        (p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm)) && 
        p.IsActive && !p.IsDeleted)
    {
        ApplyOrderBy(p => p.Name);
    }
}

public class ProductsByTagSpecification : BaseSpecification<Product>
{
    public ProductsByTagSpecification(string tag) : base(p => 
        p.Tags.Contains(tag) && p.IsActive && !p.IsDeleted)
    {
        ApplyOrderBy(p => p.Name);
    }
}

public class RecentlyCreatedProductsSpecification : BaseSpecification<Product>
{
    public RecentlyCreatedProductsSpecification(int days) : base(p => 
        p.CreatedAt >= DateTime.UtcNow.AddDays(-days) && !p.IsDeleted)
    {
        ApplyOrderByDescending(p => p.CreatedAt);
    }
}

