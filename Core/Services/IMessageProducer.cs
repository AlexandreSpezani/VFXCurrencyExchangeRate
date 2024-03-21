using Core.Dtos;

namespace Core.Services;

public interface IMessageProducer
{
    public Task ProduceAsync(CurrencyExchangeRateDto dto);
}