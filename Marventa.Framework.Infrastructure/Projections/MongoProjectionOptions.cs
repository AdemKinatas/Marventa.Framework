namespace Marventa.Framework.Infrastructure.Projections;

public class MongoProjectionOptions
{
    public const string SectionName = "MongoProjections";

    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "projections";
    public Dictionary<string, string> CollectionMappings { get; set; } = new();
    public bool CreateIndexes { get; set; } = true;

    public string GetCollectionName<T>()
    {
        var typeName = typeof(T).Name;
        return CollectionMappings.TryGetValue(typeName, out var collectionName)
            ? collectionName
            : $"{typeName.ToLowerInvariant()}s";
    }
}