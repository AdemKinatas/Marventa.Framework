using Marventa.Framework.Core.Interfaces.Events;
using Marventa.Framework.Core.Events;

namespace Marventa.Framework.Domain.ECommerce.Shipping.Events;

public class ShipmentInTransitEvent : DomainEvent
{
    public Guid ShipmentId { get; }
    public string TrackingNumber { get; }
    public string CurrentLocation { get; }

    public ShipmentInTransitEvent(Guid shipmentId, string trackingNumber, string currentLocation)
    {
        ShipmentId = shipmentId;
        TrackingNumber = trackingNumber;
        CurrentLocation = currentLocation;
    }
}