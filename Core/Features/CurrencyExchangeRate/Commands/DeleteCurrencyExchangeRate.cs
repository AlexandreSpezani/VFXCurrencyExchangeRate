using Core.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Features.CurrencyExchangeRate.Commands;

public static class DeleteCurrencyExchangeRate
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

    public record Command(string FromCurrencyCode, string ToCurrencyCode) : IRequest;

    public sealed class Handler(
        ICurrencyExchangeRepository _repository,
        ILogger<Handler> _logger) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _repository.DeleteCurrencyExchangeRate(request);

            _logger.LogInformation(
                $"[DeleteCurrencyExchangeRate] - Currency deleted" +
                $" FromCurrencyCode: {request.FromCurrencyCode},ToCurrencyCode {request.ToCurrencyCode}");
        }
    }
}