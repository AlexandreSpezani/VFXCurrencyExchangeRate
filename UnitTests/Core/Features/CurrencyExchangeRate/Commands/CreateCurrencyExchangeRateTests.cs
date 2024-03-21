using AutoFixture;
using Core.Dtos;
using Core.Exceptions;
using Core.Features.CurrencyExchangeRate.Commands;
using Core.Repositories;
using Core.Services;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace UnitTests.Core.Features.CurrencyExchangeRate.Commands;

[TestFixture]
public class CreateCurrencyExchangeRateTests
{
    private CreateCurrencyExchangeRate.Handler handler = null!;
    private Mock<ICurrencyExchangeRepository> repositoryMock = null!;
    private Mock<IMessageProducer> producerMock = null!;
    private Mock<ILogger<CreateCurrencyExchangeRate.Handler>> loggerMock = null!;
    private IFixture fixture = null!;

    [SetUp]
    public void Setup()
    {
        repositoryMock = new Mock<ICurrencyExchangeRepository>();
        producerMock = new Mock<IMessageProducer>();
        loggerMock = new Mock<ILogger<CreateCurrencyExchangeRate.Handler>>();
        fixture = new Fixture();

        handler = new CreateCurrencyExchangeRate.Handler(
            repositoryMock.Object,
            producerMock.Object,
            loggerMock.Object);
    }

    [Test]
    public async Task Validator_ValidDto_ShouldInsertOnDatabase()
    {
        // Arrange
        var filter = this.fixture.Create<CurrencyExchangeRateFilter>();

        var dto = this.fixture.Build<CurrencyExchangeRateCreateDto>()
            .With(c => c.FromCurrencyCode, filter.FromCurrencyCode)
            .With(c => c.ToCurrencyCode, filter.ToCurrencyCode)
            .Create();

        var command = new CreateCurrencyExchangeRate.Command(dto);

        repositoryMock.Setup(r => r.GetCurrencyExchangeRate(filter))
            .ReturnsAsync((CurrencyExchangeRateDto)null!);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        this.repositoryMock.Verify(x => x.CreateCurrencyExchangeRate(dto), Times.Once);
        this.producerMock.Verify(x => x.ProduceAsync(
                It.IsAny<CurrencyExchangeRateDto>()),
            Times.Once);
    }

    [Test]
    public void Validator_InvalidDto_ShouldHaveValidationErrors()
    {
        // Arrange
        var validator = new CreateCurrencyExchangeRate.Validator();

        var dto = this.fixture.Build<CurrencyExchangeRateCreateDto>()
            .Without(c => c.ExchangeRate)
            .Without(c => c.AskPrice)
            .Without(c => c.BidPrice)
            .Create();

        var command = new CreateCurrencyExchangeRate.Command(dto);

        // Act
        var result = validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(e => e.Dto.ExchangeRate);
        result.ShouldHaveValidationErrorFor(e => e.Dto.AskPrice);
        result.ShouldHaveValidationErrorFor(e => e.Dto.BidPrice);
    }

    [Test]
    public void Handler_ExistingRate_ShouldThrowAlreadyExistsException()
    {
        // Arrange
        var existingRate = this.fixture.Create<CurrencyExchangeRateDto>();

        repositoryMock.Setup(r => r.GetCurrencyExchangeRate(It.IsAny<CurrencyExchangeRateFilter>()))
            .ReturnsAsync(existingRate);

        var command = new CreateCurrencyExchangeRate.Command(existingRate);

        // Act & Assert
        Assert.ThrowsAsync<AlreadyExistsException>(() => this.handler.Handle(command, CancellationToken.None));
    }
}