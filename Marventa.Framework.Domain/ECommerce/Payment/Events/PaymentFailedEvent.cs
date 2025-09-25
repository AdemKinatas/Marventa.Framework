using Marventa.Framework.Core.Events;

namespace Marventa.Framework.Domain.ECommerce.Payment.Events;

public class PaymentFailedEvent : DomainEvent
{
    public Guid PaymentId { get; }
    public string OrderId { get; }
    public string Reason { get; }

    public PaymentFailedEvent(Guid paymentId, string orderId, string reason)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        Reason = reason;
    }
}