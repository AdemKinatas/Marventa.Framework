using Marventa.Framework.Core.Events;

namespace Marventa.Framework.Domain.ECommerce.Shipping.Events;

public class ShipmentShippedEvent : DomainEvent
{
    public Guid ShipmentId { get; }
    public string OrderId { get; }
    public string TrackingNumber { get; }
    public DateTime ShippedAt { get; }

    public ShipmentShippedEvent(Guid shipmentId, string orderId, string trackingNumber, DateTime shippedAt)
    {
        ShipmentId = shipmentId;
        OrderId = orderId;
        TrackingNumber = trackingNumber;
        ShippedAt = shippedAt;
    }
}