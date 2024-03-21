using Core.Dtos;
using Core.Gateways;
using Infra.Configuration;
using Infra.Gateways.ExternalModels;
using Newtonsoft.Json;

namespace Infra.Gateways;

public class ForeignExchangeGateway(HttpClient httpClient, ForeignExchangeApiConfiguration apiConfiguration)
    : IForeignExchangeGateway
{
    public async Task<CurrencyExchangeRateDto?> GetForeignExchangeGateway(CurrencyExchangeRateFilter filter)
    {
        var uri =
            $"{apiConfiguration.Url}/query?function=CURRENCY_EXCHANGE_RATE&from_currency={filter.FromCurrencyCode}" +
            $"&to_currency={filter.ToCurrencyCode}&apikey={apiConfiguration.Apikey}";

        var response = await httpClient.GetAsync(uri);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Fail to call API: {response.StatusCode}");
        }

        var content = await response.Content.ReadAsStringAsync();

        if (content.Contains("Error"))
        {
            return null;
        }

        var rate = JsonConvert.DeserializeObject<ApiCurrencyExchangeRate>(content);

        return rate!.ToDto();
    }
}