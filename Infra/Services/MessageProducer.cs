using System.Text;
using Confluent.Kafka;
using Core.Dtos;
using Core.Services;
using Infra.Configuration;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Infra.Services;

public class MessageProducer(IProducer<string, byte[]> _producer, KafkaConfiguration _kafkaConfiguration)
    : IMessageProducer
{
    public record CurrencyExchangeRateAdded(string FromCurrencyCode, string ToCurrencyCode);

    public async Task ProduceAsync(CurrencyExchangeRateDto dto)
    {
        var topics = _kafkaConfiguration.Producer.TopicConfigurations
            .ToDictionary(config => config.Contract, config => config.Name);

        var message = new CurrencyExchangeRateAdded(dto.FromCurrencyCode, dto.ToCurrencyCode);

        var serializedMessage = JsonSerializer.SerializeToUtf8Bytes(message);

        var messageType = typeof(CurrencyExchangeRateAdded).ToString().Split('+').Last();

        var headers = new Headers
        {
            new Header("MESSAGE_TYPE", Encoding.UTF8.GetBytes(messageType))
        };

        var kafkaMessage = new Message<string, byte[]>()
        {
            Value = serializedMessage,
            Timestamp = new Timestamp(DateTime.UtcNow),
            Headers = headers
        };

        await _producer.ProduceAsync(topics[messageType], kafkaMessage);
    }
}