using AutoMapper;
using CleanArchitectureApi.Application.DTOs;
using CleanArchitectureApi.Application.Interfaces;
using CleanArchitectureApi.Domain.Entities;
using CleanArchitectureApi.Domain.ValueObjects;
using MediatR;

namespace CleanArchitectureApi.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IAuthenticationService _authenticationService;
    private readonly IEmailService _emailService;

    public CreateUserCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IAuthenticationService authenticationService,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _authenticationService = authenticationService;
        _emailService = emailService;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var email = new Email(request.Email);
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        // Hash password
        var hashedPassword = await _authenticationService.HashPasswordAsync(request.Password);

        // Create user entity
        var user = new User(request.FirstName, request.LastName, email, hashedPassword);

        // Add user to repository
        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send welcome email
        await _emailService.SendWelcomeEmailAsync(user.Email, user.FirstName);

        // Map to DTO and return
        return _mapper.Map<UserDto>(user);
    }
}

