namespace Infra.Configuration;

public class KafkaConfiguration
{
    public string BootstrapServer { get; set; } = null!;

    public string ClientId { get; set; } = null!;

    public Producer Producer { get; set; } = null!;
}

public class Producer
{
    public IEnumerable<TopicConfiguration> TopicConfigurations { get; set; } = Array.Empty<TopicConfiguration>();
}

public class TopicConfiguration
{
    public string Contract { get; set; } = null!;
    public string Name { get; set; } = null!;
}