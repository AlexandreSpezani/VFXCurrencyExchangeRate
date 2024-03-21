namespace Core.Dtos;

public class CurrencyExchangeRateFilter
{
    public string FromCurrencyCode { get; set; } = null!;

    public string ToCurrencyCode { get; set; } = null!;
}