using CleanArchitectureApi.Domain.ValueObjects;
using FluentAssertions;

namespace CleanArchitectureApi.UnitTests.Domain.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("test+tag@example.org")]
    [InlineData("123@example.com")]
    public void Constructor_ShouldCreateEmail_WithValidEmailAddress(string emailAddress)
    {
        // Act
        var email = new Email(emailAddress);

        // Assert
        email.Value.Should().Be(emailAddress.ToLowerInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test..test@example.com")]
    [InlineData("test@example")]
    public void Constructor_ShouldThrowException_WithInvalidEmailAddress(string invalidEmail)
    {
        // Act & Assert
        var act = () => new Email(invalidEmail);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Invalid email address*");
    }

    [Fact]
    public void Equals_ShouldReturnTrue_ForSameEmailAddresses()
    {
        // Arrange
        var email1 = new Email("test@example.com");
        var email2 = new Email("TEST@EXAMPLE.COM");

        // Act & Assert
        email1.Should().Be(email2);
        (email1 == email2).Should().BeTrue();
        (email1 != email2).Should().BeFalse();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_ForDifferentEmailAddresses()
    {
        // Arrange
        var email1 = new Email("test1@example.com");
        var email2 = new Email("test2@example.com");

        // Act & Assert
        email1.Should().NotBe(email2);
        (email1 == email2).Should().BeFalse();
        (email1 != email2).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameValue_ForEqualEmails()
    {
        // Arrange
        var email1 = new Email("test@example.com");
        var email2 = new Email("TEST@EXAMPLE.COM");

        // Act & Assert
        email1.GetHashCode().Should().Be(email2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnEmailValue()
    {
        // Arrange
        var emailAddress = "test@example.com";
        var email = new Email(emailAddress);

        // Act
        var result = email.ToString();

        // Assert
        result.Should().Be(emailAddress.ToLowerInvariant());
    }

    [Fact]
    public void GetDomain_ShouldReturnDomainPart()
    {
        // Arrange
        var email = new Email("test@example.com");

        // Act
        var domain = email.GetDomain();

        // Assert
        domain.Should().Be("example.com");
    }

    [Fact]
    public void GetLocalPart_ShouldReturnLocalPart()
    {
        // Arrange
        var email = new Email("test@example.com");

        // Act
        var localPart = email.GetLocalPart();

        // Assert
        localPart.Should().Be("test");
    }

    [Fact]
    public void IsFromDomain_ShouldReturnTrue_WhenEmailIsFromSpecifiedDomain()
    {
        // Arrange
        var email = new Email("test@example.com");

        // Act
        var result = email.IsFromDomain("example.com");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsFromDomain_ShouldReturnFalse_WhenEmailIsNotFromSpecifiedDomain()
    {
        // Arrange
        var email = new Email("test@example.com");

        // Act
        var result = email.IsFromDomain("other.com");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsFromDomain_ShouldBeCaseInsensitive()
    {
        // Arrange
        var email = new Email("test@EXAMPLE.COM");

        // Act
        var result = email.IsFromDomain("example.com");

        // Assert
        result.Should().BeTrue();
    }
}

