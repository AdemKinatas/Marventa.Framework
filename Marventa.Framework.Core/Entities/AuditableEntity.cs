using System;

namespace Marventa.Framework.Core.Entities;

public abstract class AuditableEntity : BaseEntity
{
    public string Version { get; set; } = string.Empty;
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}