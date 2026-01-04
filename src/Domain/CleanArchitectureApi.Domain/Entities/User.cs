using CleanArchitectureApi.Domain.Common;
using CleanArchitectureApi.Domain.Events;
using CleanArchitectureApi.Domain.ValueObjects;

namespace CleanArchitectureApi.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsEmailConfirmed { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public bool IsActive { get; private set; } = true;
    public List<string> Roles { get; private set; } = new();

    // Navigation properties
    public virtual ICollection<Product> Products { get; private set; } = new List<Product>();

    private User() { } // EF Core constructor

    public User(string firstName, string lastName, Email email, string passwordHash)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        IsEmailConfirmed = false;
        
        AddDomainEvent(new UserCreatedEvent(this));
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new UserProfileUpdatedEvent(this));
    }

    public void ConfirmEmail()
    {
        if (IsEmailConfirmed)
            throw new InvalidOperationException("Email is already confirmed");
            
        IsEmailConfirmed = true;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new UserEmailConfirmedEvent(this));
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new UserDeactivatedEvent(this));
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new UserActivatedEvent(this));
    }

    public void AddRole(string role)
    {
        if (!Roles.Contains(role))
        {
            Roles.Add(role);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveRole(string role)
    {
        if (Roles.Contains(role))
        {
            Roles.Remove(role);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public string GetFullName() => $"{FirstName} {LastName}";
}

