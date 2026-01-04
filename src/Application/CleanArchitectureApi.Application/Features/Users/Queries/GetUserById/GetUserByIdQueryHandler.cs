using AutoMapper;
using CleanArchitectureApi.Application.DTOs;
using CleanArchitectureApi.Application.Interfaces;
using MediatR;

namespace CleanArchitectureApi.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public GetUserByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"user:{request.Id}";
        
        // Try to get from cache first
        var cachedUser = await _cacheService.GetAsync<UserDto>(cacheKey, cancellationToken);
        if (cachedUser != null)
        {
            return cachedUser;
        }

        // Get from database
        var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            return null;
        }

        var userDto = _mapper.Map<UserDto>(user);
        
        // Cache the result for 30 minutes
        await _cacheService.SetAsync(cacheKey, userDto, TimeSpan.FromMinutes(30), cancellationToken);

        return userDto;
    }
}

