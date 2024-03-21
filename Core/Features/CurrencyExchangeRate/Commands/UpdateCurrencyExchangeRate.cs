using Core.Dtos;
using Core.Exceptions;
using Core.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Features.CurrencyExchangeRate.Commands;

public static class UpdateCurrencyExchangeRate
{
    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ToCurrencyCode)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.FromCurrencyCode)
                .NotEmpty()
                .NotNull();
        }
    }

    public record Command(
        string FromCurrencyCode,
        string ToCurrencyCode,
        decimal? ExchangeRate,
        decimal? BidPrice,
        decimal? AskPrice
    ) : IRequest;


    public sealed class Handler(
        ICurrencyExchangeRepository _repository,
        ILogger<Handler> _logger) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var filter = new CurrencyExchangeRateFilter
            {
                FromCurrencyCode = request.FromCurrencyCode,
                ToCurrencyCode = request.ToCurrencyCode
            };

            var existingRate = await _repository.GetCurrencyExchangeRate(filter);

            if (existingRate is null)
            {
                throw new NotFoundException(request.FromCurrencyCode, request.ToCurrencyCode);
            }

            await _repository.UpdateCurrencyExchangeRate(request);

            _logger.LogInformation(
                $"[UpdateCurrencyExchangeRate] - Currency updated" +
                $" FromCurrencyCode: {request.FromCurrencyCode},ToCurrencyCode {request.ToCurrencyCode}");
        }
    }
}