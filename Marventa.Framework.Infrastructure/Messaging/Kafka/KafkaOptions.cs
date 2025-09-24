namespace Marventa.Framework.Infrastructure.Messaging.Kafka;

public class KafkaOptions
{
    public const string SectionName = "Kafka";

    public string BootstrapServers { get; set; } = "localhost:9092";
    public string TopicPrefix { get; set; } = "marventa-";
    public Dictionary<string, string> TopicMappings { get; set; } = new();

    // Producer settings
    public Confluent.Kafka.Acks? Acks { get; set; } = Confluent.Kafka.Acks.Leader;
    public bool EnableIdempotence { get; set; } = true;
    public int MaxInFlight { get; set; } = 5;
    public int MessageTimeoutMs { get; set; } = 300000; // 5 minutes
    public int RequestTimeoutMs { get; set; } = 30000;
    public int RetryBackoffMs { get; set; } = 100;

    // Consumer settings
    public string GroupId { get; set; } = "marventa-consumers";
    public string AutoOffsetReset { get; set; } = "earliest";
    public bool EnableAutoCommit { get; set; } = false;
    public int SessionTimeoutMs { get; set; } = 6000;
    public int HeartbeatIntervalMs { get; set; } = 3000;
    public int MaxPollIntervalMs { get; set; } = 300000;

    // Security settings
    public string? SecurityProtocol { get; set; }
    public string? SaslMechanism { get; set; }
    public string? SaslUsername { get; set; }
    public string? SaslPassword { get; set; }
    public string? SslCaLocation { get; set; }
    public string? SslCertificateLocation { get; set; }
    public string? SslKeyLocation { get; set; }

    // Topic settings
    public int NumPartitions { get; set; } = 1;
    public short ReplicationFactor { get; set; } = 1;
    public Dictionary<string, string> TopicConfigs { get; set; } = new();

    // Advanced settings
    public bool EnableLogging { get; set; } = true;
    public int StatisticsIntervalMs { get; set; } = 0;
    public Dictionary<string, string> AdditionalConfig { get; set; } = new();
}