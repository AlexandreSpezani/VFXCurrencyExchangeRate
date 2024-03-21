namespace Core.Dtos;

public class CurrencyExchangeRateDto : CurrencyExchangeRateCreateDto
{
    public DateTime LastRefreshed { get; set; }
}