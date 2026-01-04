using CleanArchitectureApi.Domain.ValueObjects;
using FluentAssertions;

namespace CleanArchitectureApi.UnitTests.Domain.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Constructor_ShouldCreateMoney_WithValidParameters()
    {
        // Arrange
        var amount = 100.50m;
        var currency = "USD";

        // Act
        var money = new Money(amount, currency);

        // Assert
        money.Amount.Should().Be(amount);
        money.Currency.Should().Be(currency);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void Constructor_ShouldThrowException_WithNegativeAmount(decimal negativeAmount)
    {
        // Act & Assert
        var act = () => new Money(negativeAmount, "USD");
        act.Should().Throw<ArgumentException>()
           .WithMessage("Amount cannot be negative*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("US")]
    [InlineData("USDD")]
    public void Constructor_ShouldThrowException_WithInvalidCurrency(string invalidCurrency)
    {
        // Act & Assert
        var act = () => new Money(100m, invalidCurrency);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Currency must be a valid 3-letter ISO code*");
    }

    [Fact]
    public void Equals_ShouldReturnTrue_ForSameAmountAndCurrency()
    {
        // Arrange
        var money1 = new Money(100.50m, "USD");
        var money2 = new Money(100.50m, "USD");

        // Act & Assert
        money1.Should().Be(money2);
        (money1 == money2).Should().BeTrue();
        (money1 != money2).Should().BeFalse();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_ForDifferentAmounts()
    {
        // Arrange
        var money1 = new Money(100.50m, "USD");
        var money2 = new Money(200.50m, "USD");

        // Act & Assert
        money1.Should().NotBe(money2);
        (money1 == money2).Should().BeFalse();
        (money1 != money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_ForDifferentCurrencies()
    {
        // Arrange
        var money1 = new Money(100.50m, "USD");
        var money2 = new Money(100.50m, "EUR");

        // Act & Assert
        money1.Should().NotBe(money2);
        (money1 == money2).Should().BeFalse();
        (money1 != money2).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameValue_ForEqualMoney()
    {
        // Arrange
        var money1 = new Money(100.50m, "USD");
        var money2 = new Money(100.50m, "USD");

        // Act & Assert
        money1.GetHashCode().Should().Be(money2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var money = new Money(100.50m, "USD");

        // Act
        var result = money.ToString();

        // Assert
        result.Should().Be("100.50 USD");
    }

    [Fact]
    public void Add_ShouldReturnSum_ForSameCurrency()
    {
        // Arrange
        var money1 = new Money(100.50m, "USD");
        var money2 = new Money(50.25m, "USD");

        // Act
        var result = money1.Add(money2);

        // Assert
        result.Amount.Should().Be(150.75m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Add_ShouldThrowException_ForDifferentCurrencies()
    {
        // Arrange
        var money1 = new Money(100.50m, "USD");
        var money2 = new Money(50.25m, "EUR");

        // Act & Assert
        var act = () => money1.Add(money2);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Cannot perform operations on different currencies*");
    }

    [Fact]
    public void Subtract_ShouldReturnDifference_ForSameCurrency()
    {
        // Arrange
        var money1 = new Money(100.50m, "USD");
        var money2 = new Money(50.25m, "USD");

        // Act
        var result = money1.Subtract(money2);

        // Assert
        result.Amount.Should().Be(50.25m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Subtract_ShouldThrowException_ForDifferentCurrencies()
    {
        // Arrange
        var money1 = new Money(100.50m, "USD");
        var money2 = new Money(50.25m, "EUR");

        // Act & Assert
        var act = () => money1.Subtract(money2);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Cannot perform operations on different currencies*");
    }

    [Fact]
    public void Subtract_ShouldThrowException_WhenResultWouldBeNegative()
    {
        // Arrange
        var money1 = new Money(50.25m, "USD");
        var money2 = new Money(100.50m, "USD");

        // Act & Assert
        var act = () => money1.Subtract(money2);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Subtraction would result in negative amount*");
    }

    [Fact]
    public void Multiply_ShouldReturnProduct()
    {
        // Arrange
        var money = new Money(100.50m, "USD");
        var multiplier = 2.5m;

        // Act
        var result = money.Multiply(multiplier);

        // Assert
        result.Amount.Should().Be(251.25m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Multiply_ShouldThrowException_WithNegativeMultiplier()
    {
        // Arrange
        var money = new Money(100.50m, "USD");
        var multiplier = -2.5m;

        // Act & Assert
        var act = () => money.Multiply(multiplier);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Multiplier cannot be negative*");
    }

    [Fact]
    public void Divide_ShouldReturnQuotient()
    {
        // Arrange
        var money = new Money(100.50m, "USD");
        var divisor = 2m;

        // Act
        var result = money.Divide(divisor);

        // Assert
        result.Amount.Should().Be(50.25m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Divide_ShouldThrowException_WithZeroDivisor()
    {
        // Arrange
        var money = new Money(100.50m, "USD");
        var divisor = 0m;

        // Act & Assert
        var act = () => money.Divide(divisor);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Divisor cannot be zero*");
    }

    [Fact]
    public void Divide_ShouldThrowException_WithNegativeDivisor()
    {
        // Arrange
        var money = new Money(100.50m, "USD");
        var divisor = -2m;

        // Act & Assert
        var act = () => money.Divide(divisor);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Divisor cannot be negative*");
    }

    [Fact]
    public void IsZero_ShouldReturnTrue_WhenAmountIsZero()
    {
        // Arrange
        var money = new Money(0m, "USD");

        // Act
        var result = money.IsZero();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsZero_ShouldReturnFalse_WhenAmountIsNotZero()
    {
        // Arrange
        var money = new Money(100.50m, "USD");

        // Act
        var result = money.IsZero();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsGreaterThan_ShouldReturnTrue_WhenAmountIsGreater()
    {
        // Arrange
        var money1 = new Money(100.50m, "USD");
        var money2 = new Money(50.25m, "USD");

        // Act
        var result = money1.IsGreaterThan(money2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsGreaterThan_ShouldReturnFalse_WhenAmountIsLessOrEqual()
    {
        // Arrange
        var money1 = new Money(50.25m, "USD");
        var money2 = new Money(100.50m, "USD");

        // Act
        var result = money1.IsGreaterThan(money2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsGreaterThan_ShouldThrowException_ForDifferentCurrencies()
    {
        // Arrange
        var money1 = new Money(100.50m, "USD");
        var money2 = new Money(50.25m, "EUR");

        // Act & Assert
        var act = () => money1.IsGreaterThan(money2);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Cannot compare different currencies*");
    }
}

