using CleanArchitectureApi.Application.DTOs;

namespace CleanArchitectureApi.Application.Interfaces;

public interface IAuthenticationService
{
    Task<string> GenerateJwtTokenAsync(UserDto user);
    Task<string> HashPasswordAsync(string password);
    Task<bool> VerifyPasswordAsync(string password, string hashedPassword);
    Task<UserDto?> AuthenticateAsync(string email, string password);
    Task<bool> ValidateTokenAsync(string token);
    Task<UserDto?> GetUserFromTokenAsync(string token);
}

