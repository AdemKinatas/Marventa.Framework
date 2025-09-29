using Marventa.Framework.Core.Interfaces.Events;
using Marventa.Framework.Core.Events;

namespace Marventa.Framework.Domain.ECommerce.Payment.Events;

public class PaymentRefundedEvent : DomainEvent
{
    public Guid PaymentId { get; }
    public string OrderId { get; }
    public decimal RefundAmount { get; }

    public PaymentRefundedEvent(Guid paymentId, string orderId, decimal refundAmount)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        RefundAmount = refundAmount;
    }
}