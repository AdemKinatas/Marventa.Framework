using Marventa.Framework.Core.Interfaces.Events;
using Marventa.Framework.Core.Events;

namespace Marventa.Framework.Domain.ECommerce.Shipping.Events;

public class ShipmentLostEvent : DomainEvent
{
    public Guid ShipmentId { get; }
    public string OrderId { get; }
    public string TrackingNumber { get; }

    public ShipmentLostEvent(Guid shipmentId, string orderId, string trackingNumber)
    {
        ShipmentId = shipmentId;
        OrderId = orderId;
        TrackingNumber = trackingNumber;
    }
}