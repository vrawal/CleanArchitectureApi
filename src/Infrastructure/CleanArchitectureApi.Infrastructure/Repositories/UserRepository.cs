using CleanArchitectureApi.Application.Interfaces;
using CleanArchitectureApi.Domain.Entities;
using CleanArchitectureApi.Domain.Specifications;
using CleanArchitectureApi.Domain.ValueObjects;
using CleanArchitectureApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureApi.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var spec = new UserByEmailSpecification(email);
        return await GetBySpecAsync(spec, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        var spec = new UserByEmailSpecification(email);
        return await ExistsAsync(spec, cancellationToken);
    }

    public async Task<List<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        var spec = new ActiveUsersSpecification();
        return await GetListAsync(spec, cancellationToken);
    }

    public async Task<List<User>> GetUsersByRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        var spec = new UsersByRoleSpecification(role);
        return await GetListAsync(spec, cancellationToken);
    }
}

