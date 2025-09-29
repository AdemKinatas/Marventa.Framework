namespace Marventa.Framework.Core.Interfaces.Projections;

public interface IProjection
{
    string Id { get; set; }
    DateTime LastUpdated { get; set; }
    string Version { get; set; }
}