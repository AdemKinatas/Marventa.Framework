using Microsoft.Extensions.Configuration;

namespace Marventa.Framework.Web.Options;

public class MarventaMessagingOptions
{
    private readonly IConfiguration _configuration;

    internal bool OutboxPatternEnabled { get; private set; }
    internal bool RabbitMqEnabled { get; private set; }
    internal bool KafkaEnabled { get; private set; }
    internal bool SagasEnabled { get; private set; }

    public MarventaMessagingOptions(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public MarventaMessagingOptions EnableOutboxPattern()
    {
        OutboxPatternEnabled = true;
        return this;
    }

    public MarventaMessagingOptions UseRabbitMq()
    {
        RabbitMqEnabled = true;
        return this;
    }

    public MarventaMessagingOptions UseKafka()
    {
        KafkaEnabled = true;
        return this;
    }

    public MarventaMessagingOptions EnableSagas()
    {
        SagasEnabled = true;
        return this;
    }

    public MarventaMessagingOptions UseAll()
    {
        return EnableOutboxPattern()
            .UseRabbitMq()
            .EnableSagas();
    }

    internal IConfiguration Configuration => _configuration;
}