using Core.Dtos;

namespace Core.Gateways;

public interface IForeignExchangeGateway
{
    Task<CurrencyExchangeRateDto?> GetForeignExchangeGateway(CurrencyExchangeRateFilter filter);
}