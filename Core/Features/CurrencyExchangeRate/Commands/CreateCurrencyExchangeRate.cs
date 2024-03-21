using Core.Dtos;
using Core.Exceptions;
using Core.Repositories;
using Core.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Features.CurrencyExchangeRate.Commands;

public static class CreateCurrencyExchangeRate
{
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.ToCurrencyCode)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Dto.FromCurrencyCode)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Dto.ExchangeRate)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(x => x.Dto.BidPrice)
                .NotNull()
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(x => x.Dto.AskPrice)
                .NotNull()
                .NotEmpty()
                .GreaterThan(0);
        }
    }

    public record Command(CurrencyExchangeRateCreateDto Dto) : IRequest;

    public sealed class Handler(
        ICurrencyExchangeRepository _repository,
        IMessageProducer _producer,
        ILogger<Handler> _logger) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var filter = new CurrencyExchangeRateFilter
            {
                FromCurrencyCode = request.Dto.FromCurrencyCode,
                ToCurrencyCode = request.Dto.ToCurrencyCode
            };

            var existingRate = await _repository.GetCurrencyExchangeRate(filter);

            if (existingRate is not null)
            {
                throw new AlreadyExistsException($"{request.Dto.ToCurrencyCode}/{request.Dto.FromCurrencyCode}");
            }

            var exchangeRate = await _repository.CreateCurrencyExchangeRate(request.Dto);

            _logger.LogInformation(
                $"[CreateCurrencyExchangeRate] - Currency created" +
                $" FromCurrencyCode: {request.Dto.FromCurrencyCode},ToCurrencyCode {request.Dto.ToCurrencyCode}");

            //Send Message
            await _producer.ProduceAsync(exchangeRate);
        }
    }
}