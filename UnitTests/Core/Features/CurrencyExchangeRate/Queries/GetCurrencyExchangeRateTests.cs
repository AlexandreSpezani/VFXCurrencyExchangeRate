using AutoFixture;
using Core.Dtos;
using Core.Exceptions;
using Core.Features.CurrencyExchangeRate.Queries;
using Core.Gateways;
using Core.Repositories;
using Core.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace UnitTests.Core.Features.CurrencyExchangeRate.Queries;

[TestFixture]
public class GetCurrencyExchangeRateTests
{
    private GetCurrencyExchangeRate.Handler handler = null!;
    private Mock<ICurrencyExchangeRepository> repositoryMock = null!;
    private Mock<IForeignExchangeGateway> gatewayMock = null!;
    private Mock<IMessageProducer> producerMock = null!;
    private Mock<ILogger<GetCurrencyExchangeRate.Handler>> loggerMock = null!;
    private IFixture fixture = null!;

    [SetUp]
    public void Setup()
    {
        repositoryMock = new Mock<ICurrencyExchangeRepository>();
        gatewayMock = new Mock<IForeignExchangeGateway>();
        producerMock = new Mock<IMessageProducer>();
        loggerMock = new Mock<ILogger<GetCurrencyExchangeRate.Handler>>();
        fixture = new Fixture();

        handler = new GetCurrencyExchangeRate.Handler(
            repositoryMock.Object,
            gatewayMock.Object,
            producerMock.Object,
            loggerMock.Object);
    }

    [Test]
    public async Task Handle_ExistingRateInRepository_ReturnsRateFromRepository()
    {
        // Arrange
        var filter = this.fixture.Create<CurrencyExchangeRateFilter>();

        var existingRate = this.fixture.Build<CurrencyExchangeRateDto>()
            .With(c => c.FromCurrencyCode, filter.FromCurrencyCode)
            .With(c => c.ToCurrencyCode, filter.ToCurrencyCode)
            .Create();

        repositoryMock.Setup(r => r.GetCurrencyExchangeRate(filter)).ReturnsAsync(existingRate);

        var command = new GetCurrencyExchangeRate.Command(filter);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        ClassicAssert.IsNotNull(result);
        ClassicAssert.AreEqual(existingRate, result);
    }

    [Test]
    public void Handle_RateNotFoundInDatabaseAndGateway_ReturnsNotFoundException()
    {
        // Arrange
        var filter = this.fixture.Create<CurrencyExchangeRateFilter>();

        repositoryMock.Setup(r => r.GetCurrencyExchangeRate(filter))
            .ReturnsAsync((CurrencyExchangeRateDto)null!);

        gatewayMock.Setup(g => g.GetForeignExchangeGateway(filter))
            .ReturnsAsync((CurrencyExchangeRateDto)null!);

        var command = new GetCurrencyExchangeRate.Command(filter);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() => this.handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public async Task Handle_RateNotFoundInDatabaseButFoundInGateway_ReturnsRateFromGateway()
    {
        // Arrange
        var filter = this.fixture.Create<CurrencyExchangeRateFilter>();

        var rateFromGateway = this.fixture.Build<CurrencyExchangeRateDto>()
            .With(c => c.FromCurrencyCode, filter.FromCurrencyCode)
            .With(c => c.ToCurrencyCode, filter.ToCurrencyCode)
            .Create();

        repositoryMock.Setup(r => r.GetCurrencyExchangeRate(filter))
            .ReturnsAsync((CurrencyExchangeRateDto)null!);

        gatewayMock.Setup(g => g.GetForeignExchangeGateway(filter))
            .ReturnsAsync(rateFromGateway);

        repositoryMock.Setup(r => r.CreateCurrencyExchangeRate(rateFromGateway))
            .ReturnsAsync(rateFromGateway);

        var command = new GetCurrencyExchangeRate.Command(filter);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        ClassicAssert.IsNotNull(result);
        ClassicAssert.AreEqual(rateFromGateway, result);

        producerMock.Verify(x => x.ProduceAsync(result), Times.Once);
    }
}