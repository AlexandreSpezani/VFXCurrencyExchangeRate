using AutoFixture;
using Core.Dtos;
using Core.Exceptions;
using Core.Features.CurrencyExchangeRate.Commands;
using Core.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace UnitTests.Core.Features.CurrencyExchangeRate.Commands;

[TestFixture]
public class UpdateCurrencyExchangeRateTests
{
    private UpdateCurrencyExchangeRate.Handler handler = null!;
    private Mock<ICurrencyExchangeRepository> repositoryMock = null!;
    private Mock<ILogger<UpdateCurrencyExchangeRate.Handler>> loggerMock = null!;
    private IFixture fixture = null!;

    [SetUp]
    public void Setup()
    {
        repositoryMock = new Mock<ICurrencyExchangeRepository>();
        loggerMock = new Mock<ILogger<UpdateCurrencyExchangeRate.Handler>>();
        handler = new UpdateCurrencyExchangeRate.Handler(repositoryMock.Object, loggerMock.Object);
        fixture = new Fixture();
    }

    [Test]
    public async Task Handle_ExistingRate_CallsRepositoryUpdate()
    {
        // Arrange
        var command = this.fixture.Create<UpdateCurrencyExchangeRate.Command>();

        repositoryMock.Setup(r => r.GetCurrencyExchangeRate(It.IsAny<CurrencyExchangeRateFilter>()))
            .ReturnsAsync(new CurrencyExchangeRateDto());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        repositoryMock.Verify(x => x.UpdateCurrencyExchangeRate(command), Times.Once);
    }

    [Test]
    public void Handle_NonExistingRate_ThrowsNotFoundException()
    {
        // Arrange
        var command = this.fixture.Create<UpdateCurrencyExchangeRate.Command>();

        repositoryMock.Setup(r => r.GetCurrencyExchangeRate(It.IsAny<CurrencyExchangeRateFilter>()))
            .ReturnsAsync((CurrencyExchangeRateDto)null!);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Validator_ValidRequest_ReturnsNoErrors()
    {
        // Arrange
        var validator = new UpdateCurrencyExchangeRate.Validator();
        var command = this.fixture.Create<UpdateCurrencyExchangeRate.Command>();

        // Act
        var validationResult = validator.Validate(command);

        // Assert
        ClassicAssert.IsTrue(validationResult.IsValid);
    }

    [Test]
    public void Validator_NullFromCurrencyCode_ReturnsValidationError()
    {
        // Arrange
        var validator = new UpdateCurrencyExchangeRate.Validator();

        var command = this.fixture.Build<UpdateCurrencyExchangeRate.Command>()
            .With(c => c.FromCurrencyCode, string.Empty)
            .Create();

        // Act
        var validationResult = validator.Validate(command);

        // Assert
        ClassicAssert.IsFalse(validationResult.IsValid);
    }

    [Test]
    public void Validator_NullToCurrencyCode_ReturnsValidationError()
    {
        // Arrange
        var validator = new UpdateCurrencyExchangeRate.Validator();
        var command = this.fixture.Build<UpdateCurrencyExchangeRate.Command>()
            .With(c => c.ToCurrencyCode, "")
            .Create();
        // Act
        var validationResult = validator.Validate(command);

        // Assert
        ClassicAssert.IsFalse(validationResult.IsValid);
    }
}