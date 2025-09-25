namespace Marventa.Framework.Core.Interfaces;

public interface ITenant
{
    string Id { get; }
    string Name { get; }
    string? ConnectionString { get; }
    Dictionary<string, object> Properties { get; }
}