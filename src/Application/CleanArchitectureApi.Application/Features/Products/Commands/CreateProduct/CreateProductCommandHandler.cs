using AutoMapper;
using CleanArchitectureApi.Application.DTOs;
using CleanArchitectureApi.Application.Interfaces;
using CleanArchitectureApi.Domain.Entities;
using CleanArchitectureApi.Domain.ValueObjects;
using MediatR;

namespace CleanArchitectureApi.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Check if SKU already exists
        var existingProduct = await _unitOfWork.Products.GetBySkuAsync(request.Sku, cancellationToken);
        if (existingProduct != null)
        {
            throw new InvalidOperationException("Product with this SKU already exists");
        }

        // Get current user ID from context (this would typically come from the HTTP context)
        // For now, we'll use a placeholder - in real implementation, this would be injected
        var userId = Guid.NewGuid(); // This should come from the authenticated user context

        // Create product entity
        var money = new Money(request.Price, request.Currency);
        var product = new Product(
            request.Name,
            request.Description,
            money,
            request.Sku,
            request.StockQuantity,
            request.Category,
            userId);

        // Add tags
        foreach (var tag in request.Tags)
        {
            product.AddTag(tag);
        }

        // Add product to repository
        await _unitOfWork.Products.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map to DTO and return
        return _mapper.Map<ProductDto>(product);
    }
}

