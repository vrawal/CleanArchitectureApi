using CleanArchitectureApi.Application.Interfaces;
using CleanArchitectureApi.Domain.Entities;
using CleanArchitectureApi.Domain.Specifications;
using CleanArchitectureApi.Infrastructure.Data;

namespace CleanArchitectureApi.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        var spec = new ProductBySkuSpecification(sku);
        return await GetBySpecAsync(spec, cancellationToken);
    }

    public async Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        var spec = new ProductBySkuSpecification(sku);
        return await ExistsAsync(spec, cancellationToken);
    }

    public async Task<List<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
    {
        var spec = new ActiveProductsSpecification();
        return await GetListAsync(spec, cancellationToken);
    }

    public async Task<List<Product>> GetProductsByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        var spec = new ProductsByCategorySpecification(category);
        return await GetListAsync(spec, cancellationToken);
    }

    public async Task<List<Product>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default)
    {
        var spec = new LowStockProductsSpecification(threshold);
        return await GetListAsync(spec, cancellationToken);
    }

    public async Task<List<Product>> GetProductsByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var spec = new ProductsByUserSpecification(userId);
        return await GetListAsync(spec, cancellationToken);
    }
}

