using CleanArchitectureApi.Application.DTOs;
using MediatR;

namespace CleanArchitectureApi.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<UserDto?>
{
    public Guid Id { get; set; }

    public GetUserByIdQuery(Guid id)
    {
        Id = id;
    }
}

