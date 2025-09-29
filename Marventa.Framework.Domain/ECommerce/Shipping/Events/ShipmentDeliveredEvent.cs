using Marventa.Framework.Core.Interfaces.Events;
using Marventa.Framework.Core.Events;

namespace Marventa.Framework.Domain.ECommerce.Shipping.Events;

public class ShipmentDeliveredEvent : DomainEvent
{
    public Guid ShipmentId { get; }
    public string OrderId { get; }
    public string TrackingNumber { get; }
    public DateTime DeliveredAt { get; }

    public ShipmentDeliveredEvent(Guid shipmentId, string orderId, string trackingNumber, DateTime deliveredAt)
    {
        ShipmentId = shipmentId;
        OrderId = orderId;
        TrackingNumber = trackingNumber;
        DeliveredAt = deliveredAt;
    }
}