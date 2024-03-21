using Core.Dtos;
using Core.Exceptions;
using Core.Gateways;
using Core.Repositories;
using Core.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Features.CurrencyExchangeRate.Queries;

public static class GetCurrencyExchangeRate
{
    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Filter.ToCurrencyCode)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.Filter.FromCurrencyCode)
                .NotEmpty()
                .NotNull();
        }
    }

    public record Command(CurrencyExchangeRateFilter Filter) : IRequest<CurrencyExchangeRateDto>;

    public sealed class Handler(
        ICurrencyExchangeRepository _repository,
        IForeignExchangeGateway _gateway,
        IMessageProducer _producer,
        ILogger<Handler> _logger)
        : IRequestHandler<Command, CurrencyExchangeRateDto>
    {
        public async Task<CurrencyExchangeRateDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var rate = await _repository.GetCurrencyExchangeRate(request.Filter);

            if (rate is null)
            {
                rate = await _gateway.GetForeignExchangeGateway(request.Filter);

                if (rate is null)
                {
                    throw new NotFoundException(request.Filter.FromCurrencyCode, request.Filter.ToCurrencyCode);
                }

                var exchangeRate = await _repository.CreateCurrencyExchangeRate(rate);

                await _producer.ProduceAsync(exchangeRate);

                _logger.LogInformation("[CreateCurrencyExchangeRate] - Currency created", () => new
                {
                    request.Filter.FromCurrencyCode,
                    request.Filter.ToCurrencyCode
                });
            }

            return rate;
        }
    }
}