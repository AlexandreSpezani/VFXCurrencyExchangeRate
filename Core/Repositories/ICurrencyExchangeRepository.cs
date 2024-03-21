using Core.Dtos;
using Core.Features.CurrencyExchangeRate.Commands;

namespace Core.Repositories;

public interface ICurrencyExchangeRepository
{
    public Task<CurrencyExchangeRateDto?> GetCurrencyExchangeRate(CurrencyExchangeRateFilter filter);

    public Task<CurrencyExchangeRateDto> CreateCurrencyExchangeRate(CurrencyExchangeRateCreateDto rate);

    public Task UpdateCurrencyExchangeRate(UpdateCurrencyExchangeRate.Command  rate);

    public Task DeleteCurrencyExchangeRate(DeleteCurrencyExchangeRate.Command rate);
}