namespace Marventa.Framework.Infrastructure.Options;

public class ElasticsearchOptions : IValidateOptions<ElasticsearchOptions>
{
    public string ConnectionString { get; set; } = "http://localhost:9200";
    public string IndexPrefix { get; set; } = "marventa-";
    public string? Username { get; set; }
    public string? Password { get; set; }
    public int NumberOfShards { get; set; } = 1;
    public int NumberOfReplicas { get; set; } = 1;
    public bool UseSsl { get; set; } = false;
    public int TimeoutSeconds { get; set; } = 60;
    public int MaxRetries { get; set; } = 3;

    public ValidateOptionsResult Validate(string? name, ElasticsearchOptions options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
            errors.Add("ConnectionString is required.");

        if (!Uri.TryCreate(options.ConnectionString, UriKind.Absolute, out _))
            errors.Add("ConnectionString must be a valid URI.");

        if (string.IsNullOrWhiteSpace(options.IndexPrefix))
            errors.Add("IndexPrefix is required.");

        if (options.NumberOfShards < 1)
            errors.Add("NumberOfShards must be at least 1.");

        if (options.NumberOfReplicas < 0)
            errors.Add("NumberOfReplicas cannot be negative.");

        if (options.TimeoutSeconds <= 0)
            errors.Add("TimeoutSeconds must be greater than zero.");

        if (options.MaxRetries < 0)
            errors.Add("MaxRetries cannot be negative.");

        if (!string.IsNullOrWhiteSpace(options.Username) && string.IsNullOrWhiteSpace(options.Password))
            errors.Add("Password required when Username provided.");

        return errors.Count > 0 ? ValidateOptionsResult.Fail(errors) : ValidateOptionsResult.Success;
    }
}