using Marventa.Framework.Core.Entities;
using Marventa.Framework.Core.Interfaces.Events;
using Marventa.Framework.Core.Events;
using Marventa.Framework.Domain.ValueObjects;
using Marventa.Framework.Domain.ECommerce.Shipping.Events;

namespace Marventa.Framework.Domain.ECommerce.Shipping;

public class Shipment : BaseEntity
{
    public string OrderId { get; private set; } = string.Empty;
    public string TrackingNumber { get; private set; } = string.Empty;
    public ShippingStatus Status { get; private set; }
    public ShippingCarrier Carrier { get; private set; }
    public Address FromAddress { get; private set; } = null!;
    public Address ToAddress { get; private set; } = null!;
    public Money ShippingCost { get; private set; } = null!;
    public decimal Weight { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime EstimatedDelivery { get; private set; }
    public string? SignedBy { get; private set; }
    public string? Notes { get; private set; }
    public string? TenantId { get; set; }

    private readonly List<ShipmentItem> _items = new();
    public IReadOnlyCollection<ShipmentItem> Items => _items.AsReadOnly();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected Shipment() { }

    public Shipment(
        string orderId,
        ShippingCarrier carrier,
        Address fromAddress,
        Address toAddress,
        Money shippingCost,
        DateTime estimatedDelivery)
    {
        OrderId = orderId;
        Carrier = carrier;
        FromAddress = fromAddress;
        ToAddress = toAddress;
        ShippingCost = shippingCost;
        EstimatedDelivery = estimatedDelivery;
        Status = ShippingStatus.Pending;
        TrackingNumber = GenerateTrackingNumber();

        _domainEvents.Add(new ShipmentCreatedEvent(Id, OrderId, TrackingNumber));
    }

    public void AddItem(string productId, string productName, int quantity, decimal weight)
    {
        _items.Add(new ShipmentItem(productId, productName, quantity, weight));
        Weight += weight * quantity;
    }

    public void MarkAsShipped()
    {
        if (Status != ShippingStatus.Pending)
            throw new InvalidOperationException($"Cannot ship from status {Status}");

        Status = ShippingStatus.Shipped;
        ShippedAt = DateTime.UtcNow;

        _domainEvents.Add(new ShipmentShippedEvent(Id, OrderId, TrackingNumber, ShippedAt.Value));
    }

    public void MarkAsInTransit(string currentLocation)
    {
        if (Status != ShippingStatus.Shipped && Status != ShippingStatus.InTransit)
            throw new InvalidOperationException($"Cannot mark in transit from status {Status}");

        Status = ShippingStatus.InTransit;
        Notes = $"In transit at: {currentLocation}";

        _domainEvents.Add(new ShipmentInTransitEvent(Id, TrackingNumber, currentLocation));
    }

    public void MarkAsDelivered(string? signedBy = null)
    {
        if (Status != ShippingStatus.InTransit && Status != ShippingStatus.OutForDelivery)
            throw new InvalidOperationException($"Cannot deliver from status {Status}");

        Status = ShippingStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        SignedBy = signedBy ?? "Customer";

        _domainEvents.Add(new ShipmentDeliveredEvent(Id, OrderId, TrackingNumber, DeliveredAt.Value));
    }

    public void MarkAsLost()
    {
        Status = ShippingStatus.Lost;
        _domainEvents.Add(new ShipmentLostEvent(Id, OrderId, TrackingNumber));
    }

    private static string GenerateTrackingNumber()
    {
        return $"TRK{DateTime.UtcNow:yyyyMMdd}{Guid.NewGuid().ToString()[..8].ToUpper()}";
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}