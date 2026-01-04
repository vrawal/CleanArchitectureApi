using AutoMapper;
using CleanArchitectureApi.Application.DTOs;
using CleanArchitectureApi.Application.Features.Users.Commands.CreateUser;
using CleanArchitectureApi.Application.Interfaces;
using CleanArchitectureApi.Domain.Entities;
using CleanArchitectureApi.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CleanArchitectureApi.UnitTests.Application.Features.Users.Commands;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IAuthenticationService> _authServiceMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _authServiceMock = new Mock<IAuthenticationService>();
        _emailServiceMock = new Mock<IEmailService>();
        
        _handler = new CreateUserCommandHandler(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _authServiceMock.Object,
            _emailServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenValidCommand()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "Password123!"
        };

        var hashedPassword = "hashedPassword";
        var user = new User(command.FirstName, command.LastName, new Email(command.Email), hashedPassword);
        var userDto = new UserDto
        {
            Id = user.Id,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            IsEmailConfirmed = false,
            IsActive = true,
            Roles = new List<string>()
        };

        _unitOfWorkMock.Setup(x => x.Users.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync((User?)null);
        
        _authServiceMock.Setup(x => x.HashPasswordAsync(command.Password))
                       .ReturnsAsync(hashedPassword);
        
        _unitOfWorkMock.Setup(x => x.Users.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(user);
        
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(1);
        
        _mapperMock.Setup(x => x.Map<UserDto>(It.IsAny<User>()))
                  .Returns(userDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be(command.FirstName);
        result.LastName.Should().Be(command.LastName);
        result.Email.Should().Be(command.Email);
        result.IsActive.Should().BeTrue();
        result.IsEmailConfirmed.Should().BeFalse();

        _unitOfWorkMock.Verify(x => x.Users.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Once);
        _authServiceMock.Verify(x => x.HashPasswordAsync(command.Password), Times.Once);
        _unitOfWorkMock.Verify(x => x.Users.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _emailServiceMock.Verify(x => x.SendWelcomeEmailAsync(command.Email, command.FirstName), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenUserAlreadyExists()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "Password123!"
        };

        var existingUser = new User("Jane", "Doe", new Email(command.Email), "hashedPassword");

        _unitOfWorkMock.Setup(x => x.Users.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(existingUser);

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("User with this email already exists");

        _unitOfWorkMock.Verify(x => x.Users.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Once);
        _authServiceMock.Verify(x => x.HashPasswordAsync(It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.Users.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _emailServiceMock.Verify(x => x.SendWelcomeEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldSendWelcomeEmail_AfterUserCreation()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "Password123!"
        };

        var hashedPassword = "hashedPassword";
        var userDto = new UserDto
        {
            Id = Guid.NewGuid(),
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            IsEmailConfirmed = false,
            IsActive = true,
            Roles = new List<string>()
        };

        _unitOfWorkMock.Setup(x => x.Users.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync((User?)null);
        
        _authServiceMock.Setup(x => x.HashPasswordAsync(command.Password))
                       .ReturnsAsync(hashedPassword);
        
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(1);
        
        _mapperMock.Setup(x => x.Map<UserDto>(It.IsAny<User>()))
                  .Returns(userDto);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _emailServiceMock.Verify(x => x.SendWelcomeEmailAsync(command.Email, command.FirstName), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotSendEmail_WhenSaveChangesFails()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "Password123!"
        };

        var hashedPassword = "hashedPassword";

        _unitOfWorkMock.Setup(x => x.Users.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync((User?)null);
        
        _authServiceMock.Setup(x => x.HashPasswordAsync(command.Password))
                       .ReturnsAsync(hashedPassword);
        
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(0); // Simulate save failure

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("Failed to create user");

        _emailServiceMock.Verify(x => x.SendWelcomeEmailAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}

