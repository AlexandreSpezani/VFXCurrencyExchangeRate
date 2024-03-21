using Confluent.Kafka;
using Core.Gateways;
using Core.Repositories;
using Core.Services;
using Infra.Configuration;
using Infra.Gateways;
using Infra.Repositories;
using Infra.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoConfiguration = configuration.GetSection("Mongo").Get<MongoConfiguration>()!;

        var foreignApiConfiguration =
            configuration.GetSection("ForeignExchangeApi").Get<ForeignExchangeApiConfiguration>()!;

        var kafkaConfiguration = configuration.GetSection("Kafka").Get<KafkaConfiguration>()!;

        return services
            .AddSingleton(foreignApiConfiguration)
            .AddSingleton(kafkaConfiguration)
            .AddMongo(mongoConfiguration)
            .AddKafka(kafkaConfiguration)
            .AddRepositories()
            .AddServices();
    }

    private static IServiceCollection AddMongo(this IServiceCollection services, MongoConfiguration mongoConfiguration)
    {
        return services
            .AddSingleton<IMongoClient>(_ => new MongoClient(mongoConfiguration.ConnectionString))
            .AddSingleton(p => p.GetRequiredService<IMongoClient>().GetDatabase(mongoConfiguration.Database));
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddSingleton<ICurrencyExchangeRepository, CurrencyExchangeRepository>();
    }

    private static IServiceCollection AddKafka(this IServiceCollection services, KafkaConfiguration kafkaConfiguration)
    {
        services.AddSingleton<IAdminClient>(serviceProvider => new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = kafkaConfiguration.BootstrapServer
        }).Build());

        services.AddSingleton<IProducer<string, byte[]>>(serviceProvider =>
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = kafkaConfiguration.BootstrapServer,
                ClientId = kafkaConfiguration.ClientId,
                Acks = Acks.All,
            };

            return new ProducerBuilder<string, byte[]>(producerConfig).Build();
        });

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IForeignExchangeGateway, ForeignExchangeGateway>()
            .AddSingleton<IMessageProducer, MessageProducer>();
    }
}