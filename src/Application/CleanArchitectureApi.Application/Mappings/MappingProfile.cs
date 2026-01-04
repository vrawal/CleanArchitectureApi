using AutoMapper;
using CleanArchitectureApi.Application.DTOs;
using CleanArchitectureApi.Domain.Entities;

namespace CleanArchitectureApi.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value));

        CreateMap<User, UserWithProductsDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value));

        CreateMap<CreateUserDto, User>()
            .ConstructUsing(src => new User(src.FirstName, src.LastName, new Domain.ValueObjects.Email(src.Email), src.Password));

        // Product mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Price.Currency));

        CreateMap<Product, ProductWithUserDto>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Price.Currency));

        CreateMap<CreateProductDto, Product>()
            .ConstructUsing((src, context) => 
            {
                var userId = context.Items.ContainsKey("UserId") ? (Guid)context.Items["UserId"] : Guid.Empty;
                return new Product(
                    src.Name, 
                    src.Description, 
                    new Domain.ValueObjects.Money(src.Price, src.Currency), 
                    src.Sku, 
                    src.StockQuantity, 
                    src.Category, 
                    userId);
            });
    }
}

