namespace Marventa.Framework.Configuration;

/// <summary>
/// Contains constant values for configuration section names and keys used throughout the framework.
/// </summary>
public static class ConfigurationKeys
{
    #region Section Names

    public const string Jwt = "Jwt";
    public const string Swagger = "Swagger";
    public const string ApiVersioning = "ApiVersioning";
    public const string Cors = "Cors";
    public const string Caching = "Caching";
    public const string MemoryCache = "MemoryCache";
    public const string OutputCache = "OutputCache";
    public const string Redis = "Redis";
    public const string MultiTenancy = "MultiTenancy";
    public const string RateLimiting = "RateLimiting";
    public const string RabbitMQ = "RabbitMQ";
    public const string Kafka = "Kafka";
    public const string MassTransit = "MassTransit";
    public const string Elasticsearch = "Elasticsearch";
    public const string MongoDB = "MongoDB";
    public const string AzureStorage = "Azure:Storage";
    public const string AWS = "AWS";
    public const string LocalStorage = "LocalStorage";
    public const string HealthChecks = "HealthChecks";
    public const string Serilog = "Serilog";
    public const string OpenTelemetry = "OpenTelemetry";
    public const string ExceptionHandling = "ExceptionHandling";
    public const string ApplicationName = "ApplicationName";

    #endregion

    #region Connection Strings

    public const string DefaultConnection = "DefaultConnection";

    #endregion

    #region Common Keys

    public const string Enabled = "Enabled";
    public const string ConnectionString = "ConnectionString";
    public const string Host = "Host";
    public const string Username = "Username";
    public const string Password = "Password";

    #endregion

    #region CORS Keys

    public const string CorsAllowedOrigins = "Cors:AllowedOrigins";

    #endregion

    #region JWT Keys

    public const string JwtIssuer = "Jwt:Issuer";
    public const string JwtAudience = "Jwt:Audience";
    public const string JwtSecret = "Jwt:Secret";

    #endregion

    #region RabbitMQ Keys

    public const string RabbitMQHost = "RabbitMQ:Host";
    public const string RabbitMQVirtualHost = "RabbitMQ:VirtualHost";
    public const string RabbitMQUsername = "RabbitMQ:Username";
    public const string RabbitMQPassword = "RabbitMQ:Password";

    #endregion

    #region Kafka Keys

    public const string KafkaBootstrapServers = "Kafka:BootstrapServers";
    public const string KafkaGroupId = "Kafka:GroupId";

    #endregion

    #region Elasticsearch Keys

    public const string ElasticsearchUri = "Elasticsearch:Uri";

    #endregion

    #region MongoDB Keys

    public const string MongoDBConnectionString = "MongoDB:ConnectionString";
    public const string MongoDBDatabaseName = "MongoDB:DatabaseName";

    #endregion

    #region Azure Storage Keys

    public const string AzureStorageConnectionString = "Azure:Storage:ConnectionString";
    public const string AzureStorageContainerName = "Azure:Storage:ContainerName";

    #endregion

    #region AWS Keys

    public const string AWSAccessKey = "AWS:AccessKey";
    public const string AWSSecretKey = "AWS:SecretKey";
    public const string AWSRegion = "AWS:Region";
    public const string AWSBucketName = "AWS:BucketName";

    #endregion

    #region Local Storage Keys

    public const string LocalStorageBasePath = "LocalStorage:BasePath";
    public const string LocalStorageBaseUrl = "LocalStorage:BaseUrl";

    #endregion

    #region Redis Keys

    public const string RedisConnectionString = "Redis:ConnectionString";

    #endregion

    #region OpenTelemetry Keys

    public const string OpenTelemetryServiceName = "OpenTelemetry:ServiceName";
    public const string OpenTelemetryOtlpEndpoint = "OpenTelemetry:OtlpEndpoint";

    #endregion

    #region Default Values

    public static class Defaults
    {
        public const string RabbitMQHost = "localhost";
        public const string RabbitMQUsername = "guest";
        public const string RabbitMQPassword = "guest";
        public const string RabbitMQVirtualHost = "/";

        public const string KafkaBootstrapServers = "localhost:9092";
        public const string KafkaGroupId = "default-group";

        public const string ElasticsearchUri = "http://localhost:9200";
        public const string ElasticsearchDefaultIndex = "default-index";

        public const string MongoDBConnectionString = "mongodb://localhost:27017";
        public const string MongoDBDatabaseName = "default";

        public const string AWSRegion = "us-east-1";
        public const string AWSBucketName = "default";
        public const string AzureStorageContainerName = "default";

        public const string ApplicationName = "Marventa";
    }

    #endregion
}
