using Core.Dtos;
using MongoDB.Bson;

namespace Infra.DataContracts;

public class CurrencyExchangeRate
{
    public ObjectId Id { get; set; }

    public string FromCurrencyCode { get; set; } = null!;

    public string ToCurrencyCode { get; set; } = null!;

    public decimal ExchangeRate { get; set; }

    public decimal BidPrice { get; set; }

    public decimal AskPrice { get; set; }

    public DateTime LastRefreshed { get; set; }
}

public static class CurrencyExchangeRateExtensions
{
    public static CurrencyExchangeRateDto ToDto(this CurrencyExchangeRate instance)
    {
        return new CurrencyExchangeRateDto()
        {
            ToCurrencyCode = instance.ToCurrencyCode,
            FromCurrencyCode = instance.FromCurrencyCode,
            ExchangeRate = instance.ExchangeRate,
            AskPrice = instance.AskPrice,
            BidPrice = instance.BidPrice,
            LastRefreshed = instance.LastRefreshed
        };
    }

    public static CurrencyExchangeRate ToEntity(this CurrencyExchangeRateCreateDto instance)
    {
        return new CurrencyExchangeRate()
        {
            ToCurrencyCode = instance.ToCurrencyCode,
            FromCurrencyCode = instance.FromCurrencyCode,
            ExchangeRate = instance.ExchangeRate,
            AskPrice = instance.AskPrice,
            BidPrice = instance.BidPrice
        };
    }
}