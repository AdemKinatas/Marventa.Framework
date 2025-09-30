using MediatR;

namespace Marventa.Framework.Core.Domain;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
