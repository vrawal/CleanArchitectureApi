using CleanArchitectureApi.Domain.Entities;

namespace CleanArchitectureApi.Application.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<List<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
    Task<List<Product>> GetProductsByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<List<Product>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default);
    Task<List<Product>> GetProductsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
}

