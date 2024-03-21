using Core.Features.CurrencyExchangeRate.Commands;
using Core.Repositories;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace UnitTests.Core.Features.CurrencyExchangeRate.Commands;

[TestFixture]
public class DeleteCurrencyExchangeRateTests
{
    private DeleteCurrencyExchangeRate.Handler handler = null!;
    private Mock<ICurrencyExchangeRepository> repositoryMock = null!;
    private Mock<ILogger<DeleteCurrencyExchangeRate.Handler>> loggerMock = null!;

    [SetUp]
    public void Setup()
    {
        repositoryMock = new Mock<ICurrencyExchangeRepository>();
        loggerMock = new Mock<ILogger<DeleteCurrencyExchangeRate.Handler>>();
        handler = new DeleteCurrencyExchangeRate.Handler(repositoryMock.Object, loggerMock.Object);
    }

    [Test]
    public async Task Handle_ValidRequest_CallsRepositoryDelete()
    {
        // Arrange
        var command = new DeleteCurrencyExchangeRate.Command("USD", "EUR");

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        repositoryMock.Verify(x => x.DeleteCurrencyExchangeRate(command), Times.Once);
    }

    [Test]
    public void Validator_ValidRequest_ReturnsNoErrors()
    {
        // Arrange
        var validator = new DeleteCurrencyExchangeRate.Validator();
        var command = new DeleteCurrencyExchangeRate.Command("USD", "EUR");

        // Act
        var result = validator.Validate(command);

        // Assert
        ClassicAssert.IsTrue(result.IsValid);
    }

    [Test]
    public void Validator_NullFromCurrencyCode_ReturnsValidationError()
    {
        // Arrange
        var validator = new DeleteCurrencyExchangeRate.Validator();
        var command = new DeleteCurrencyExchangeRate.Command("", "EUR");

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(e => e.FromCurrencyCode);
    }

    [Test]
    public void Validator_NullToCurrencyCode_ReturnsValidationError()
    {
        // Arrange
        var validator = new DeleteCurrencyExchangeRate.Validator();
        var command = new DeleteCurrencyExchangeRate.Command("USD", null!);

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(e => e.ToCurrencyCode);
    }
}