namespace Infra.Configuration;

public class MongoConfiguration
{
    public string ConnectionString { set; get; } = default!;
    public string Database { set; get; } = default!;
}