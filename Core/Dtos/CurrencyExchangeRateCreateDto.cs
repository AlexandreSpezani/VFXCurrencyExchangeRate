namespace Core.Dtos;

public class CurrencyExchangeRateCreateDto
{
    public string FromCurrencyCode { get; set; } = null!;

    public string ToCurrencyCode { get; set; } = null!;

    public decimal ExchangeRate { get; set; }

    public decimal BidPrice { get; set; }

    public decimal AskPrice { get; set; }
}