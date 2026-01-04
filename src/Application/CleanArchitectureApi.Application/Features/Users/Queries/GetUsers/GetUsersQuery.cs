using CleanArchitectureApi.Application.DTOs;
using CleanArchitectureApi.Application.DTOs.Common;
using MediatR;

namespace CleanArchitectureApi.Application.Features.Users.Queries.GetUsers;

public class GetUsersQuery : PaginationQuery, IRequest<PaginatedResult<UserDto>>
{
    public bool? IsActive { get; set; }
    public bool? IsEmailConfirmed { get; set; }
    public string? Role { get; set; }
}

