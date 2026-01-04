using CleanArchitectureApi.Application.DTOs;
using CleanArchitectureApi.Application.Features.Users.Commands.CreateUser;
using CleanArchitectureApi.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureApi.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, IAuthenticationService authenticationService, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _authenticationService = authenticationService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">User registration details</param>
    /// <returns>Created user information</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> Register([FromBody] CreateUserCommand request)
    {
        try
        {
            var user = await _mediator.Send(request);
            _logger.LogInformation("User registered successfully with email: {Email}", request.Email);
            return CreatedAtAction(nameof(GetProfile), new { }, user);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            _logger.LogWarning("Registration attempt with existing email: {Email}", request.Email);
            return Conflict(new { message = "User with this email already exists" });
        }
    }

    /// <summary>
    /// Authenticate user and get JWT token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Login([FromBody] UserLoginDto request)
    {
        var user = await _authenticationService.AuthenticateAsync(request.Email, request.Password);
        if (user == null)
        {
            _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var token = await _authenticationService.GenerateJwtTokenAsync(user);
        
        _logger.LogInformation("User logged in successfully: {Email}", request.Email);
        
        return Ok(new
        {
            token,
            user,
            expiresAt = DateTime.UtcNow.AddHours(1) // Adjust based on your JWT expiry
        });
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("profile")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> GetProfile()
    {
        var authHeader = Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer "))
        {
            return Unauthorized();
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        var user = await _authenticationService.GetUserFromTokenAsync(token);
        
        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(user);
    }

    /// <summary>
    /// Validate JWT token
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>Token validation result</returns>
    [HttpPost("validate-token")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ValidateToken([FromBody] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest(new { message = "Token is required" });
        }

        var isValid = await _authenticationService.ValidateTokenAsync(token);
        
        return Ok(new { isValid });
    }
}

