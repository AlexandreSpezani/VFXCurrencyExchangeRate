using Core.Dtos;
using Core.Features.CurrencyExchangeRate.Commands;
using Core.Repositories;
using Infra.DataContracts;
using MongoDB.Driver;

namespace Infra.Repositories;

public class CurrencyExchangeRepository(IMongoDatabase database) : ICurrencyExchangeRepository
{
    private const string CollectionName = "CurrencyExchangeRate";

    private readonly IMongoCollection<CurrencyExchangeRate> collection =
        database.GetCollection<CurrencyExchangeRate>(CollectionName);

    public async Task<CurrencyExchangeRateDto?> GetCurrencyExchangeRate(CurrencyExchangeRateFilter filter)
    {
        var filterDefinition = Builders<CurrencyExchangeRate>.Filter.And(
            Builders<CurrencyExchangeRate>.Filter.Eq(x => x.FromCurrencyCode, filter.FromCurrencyCode),
            Builders<CurrencyExchangeRate>.Filter.Eq(x => x.ToCurrencyCode, filter.ToCurrencyCode));

        var result = await this.collection.Find(filterDefinition).FirstOrDefaultAsync();

        return result?.ToDto();
    }

    public async Task<CurrencyExchangeRateDto> CreateCurrencyExchangeRate(CurrencyExchangeRateCreateDto rate)
    {
        var dataContract = rate.ToEntity();

        dataContract.LastRefreshed = DateTime.Now;

        await this.collection.InsertOneAsync(dataContract);

        return dataContract.ToDto();
    }

    public async Task UpdateCurrencyExchangeRate(UpdateCurrencyExchangeRate.Command rate)
    {
        var filterDefinition = Builders<CurrencyExchangeRate>.Filter.And(
            Builders<CurrencyExchangeRate>.Filter.Eq(x => x.FromCurrencyCode, rate.FromCurrencyCode),
            Builders<CurrencyExchangeRate>.Filter.Eq(x => x.ToCurrencyCode, rate.ToCurrencyCode));

        var updateDefinition =
            Builders<CurrencyExchangeRate>.Update.Combine();

        if (rate.ExchangeRate is not null)
        {
            updateDefinition = updateDefinition.Set(x => x.ExchangeRate, rate.ExchangeRate);
        }

        if (rate.BidPrice is not null)
        {
            updateDefinition = updateDefinition.Set(x => x.BidPrice, rate.BidPrice);
        }

        if (rate.AskPrice is not null)
        {
            updateDefinition = updateDefinition.Set(x => x.AskPrice, rate.AskPrice);
        }

        await this.collection.UpdateOneAsync(
            filterDefinition,
            updateDefinition,
            options: new UpdateOptions { IsUpsert = false });
    }

    public async Task DeleteCurrencyExchangeRate(DeleteCurrencyExchangeRate.Command rate)
    {
        var filterDefinition = Builders<CurrencyExchangeRate>.Filter.And(
            Builders<CurrencyExchangeRate>.Filter.Eq(x => x.FromCurrencyCode, rate.FromCurrencyCode),
            Builders<CurrencyExchangeRate>.Filter.Eq(x => x.ToCurrencyCode, rate.ToCurrencyCode));

        await this.collection.DeleteOneAsync(filterDefinition);
    }
}