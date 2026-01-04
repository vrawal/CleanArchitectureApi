using CleanArchitectureApi.Domain.Entities;
using CleanArchitectureApi.Domain.Events;
using CleanArchitectureApi.Domain.ValueObjects;
using FluentAssertions;

namespace CleanArchitectureApi.UnitTests.Domain.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldCreateUser_WithValidParameters()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = new Email("john.doe@example.com");
        var passwordHash = "hashedPassword";

        // Act
        var user = new User(firstName, lastName, email, passwordHash);

        // Assert
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
        user.Email.Should().Be(email);
        user.PasswordHash.Should().Be(passwordHash);
        user.IsEmailConfirmed.Should().BeFalse();
        user.IsActive.Should().BeTrue();
        user.Roles.Should().BeEmpty();
        user.DomainEvents.Should().ContainSingle(e => e is UserCreatedEvent);
    }

    [Fact]
    public void UpdateProfile_ShouldUpdateUserProfile_AndRaiseDomainEvent()
    {
        // Arrange
        var user = CreateTestUser();
        var newFirstName = "Jane";
        var newLastName = "Smith";

        // Act
        user.UpdateProfile(newFirstName, newLastName);

        // Assert
        user.FirstName.Should().Be(newFirstName);
        user.LastName.Should().Be(newLastName);
        user.UpdatedAt.Should().NotBeNull();
        user.DomainEvents.Should().Contain(e => e is UserProfileUpdatedEvent);
    }

    [Fact]
    public void ConfirmEmail_ShouldConfirmEmail_AndRaiseDomainEvent()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        user.ConfirmEmail();

        // Assert
        user.IsEmailConfirmed.Should().BeTrue();
        user.UpdatedAt.Should().NotBeNull();
        user.DomainEvents.Should().Contain(e => e is UserEmailConfirmedEvent);
    }

    [Fact]
    public void ConfirmEmail_WhenAlreadyConfirmed_ShouldThrowException()
    {
        // Arrange
        var user = CreateTestUser();
        user.ConfirmEmail();

        // Act & Assert
        var act = () => user.ConfirmEmail();
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Email is already confirmed");
    }

    [Fact]
    public void UpdateLastLogin_ShouldUpdateLastLoginTime()
    {
        // Arrange
        var user = CreateTestUser();
        var beforeUpdate = DateTime.UtcNow;

        // Act
        user.UpdateLastLogin();

        // Assert
        user.LastLoginAt.Should().NotBeNull();
        user.LastLoginAt.Should().BeOnOrAfter(beforeUpdate);
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Deactivate_ShouldDeactivateUser_AndRaiseDomainEvent()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        user.Deactivate();

        // Assert
        user.IsActive.Should().BeFalse();
        user.UpdatedAt.Should().NotBeNull();
        user.DomainEvents.Should().Contain(e => e is UserDeactivatedEvent);
    }

    [Fact]
    public void Activate_ShouldActivateUser_AndRaiseDomainEvent()
    {
        // Arrange
        var user = CreateTestUser();
        user.Deactivate();

        // Act
        user.Activate();

        // Assert
        user.IsActive.Should().BeTrue();
        user.UpdatedAt.Should().NotBeNull();
        user.DomainEvents.Should().Contain(e => e is UserActivatedEvent);
    }

    [Fact]
    public void AddRole_ShouldAddRole_WhenNotExists()
    {
        // Arrange
        var user = CreateTestUser();
        var role = "Admin";

        // Act
        user.AddRole(role);

        // Assert
        user.Roles.Should().Contain(role);
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void AddRole_ShouldNotAddRole_WhenAlreadyExists()
    {
        // Arrange
        var user = CreateTestUser();
        var role = "Admin";
        user.AddRole(role);
        var initialCount = user.Roles.Count;

        // Act
        user.AddRole(role);

        // Assert
        user.Roles.Should().HaveCount(initialCount);
        user.Roles.Should().ContainSingle(r => r == role);
    }

    [Fact]
    public void RemoveRole_ShouldRemoveRole_WhenExists()
    {
        // Arrange
        var user = CreateTestUser();
        var role = "Admin";
        user.AddRole(role);

        // Act
        user.RemoveRole(role);

        // Assert
        user.Roles.Should().NotContain(role);
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void RemoveRole_ShouldDoNothing_WhenRoleNotExists()
    {
        // Arrange
        var user = CreateTestUser();
        var role = "Admin";

        // Act
        user.RemoveRole(role);

        // Assert
        user.Roles.Should().NotContain(role);
    }

    [Fact]
    public void GetFullName_ShouldReturnConcatenatedName()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        var fullName = user.GetFullName();

        // Assert
        fullName.Should().Be($"{user.FirstName} {user.LastName}");
    }

    private static User CreateTestUser()
    {
        return new User(
            "John",
            "Doe",
            new Email("john.doe@example.com"),
            "hashedPassword");
    }
}

