using Core.Dtos;
using Newtonsoft.Json;

namespace Infra.Gateways.ExternalModels;

public class ApiCurrencyExchangeRate
{
    [JsonProperty("Realtime Currency Exchange Rate")]
    public CurrencyExchangeData ExchangeData { get; set; } = new();
}

public class CurrencyExchangeData
{
    [JsonProperty("1. From_Currency Code")]
    public string FromCurrencyCode { get; set; }  = null!;

    [JsonProperty("2. From_Currency Name")]
    public string FromCurrencyName { get; set; }  = null!;

    [JsonProperty("3. To_Currency Code")] public string ToCurrencyCode { get; set; } = null!;

    [JsonProperty("4. To_Currency Name")] public string ToCurrencyName { get; set; } = null!;

    [JsonProperty("5. Exchange Rate")] public decimal ExchangeRate { get; set; }

    [JsonProperty("6. Last Refreshed")] public DateTime LastRefreshed { get; set; }

    [JsonProperty("7. Time Zone")] public string TimeZone { get; set; } = null!;

    [JsonProperty("8. Bid Price")] public decimal BidPrice { get; set; }

    [JsonProperty("9. Ask Price")] public decimal AskPrice { get; set; }
}

public static class ApiCurrencyExchangeRateExtensions
{
    public static CurrencyExchangeRateDto ToDto(this ApiCurrencyExchangeRate instance)
    {
        return new CurrencyExchangeRateDto()
        {
            ToCurrencyCode = instance.ExchangeData.ToCurrencyCode,
            FromCurrencyCode = instance.ExchangeData.FromCurrencyCode,
            ExchangeRate = instance.ExchangeData.ExchangeRate,
            AskPrice = instance.ExchangeData.AskPrice,
            BidPrice = instance.ExchangeData.BidPrice,
            LastRefreshed = instance.ExchangeData.LastRefreshed
        };
    }
}