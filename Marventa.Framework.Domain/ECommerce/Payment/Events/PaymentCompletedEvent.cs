using Marventa.Framework.Core.Events;

namespace Marventa.Framework.Domain.ECommerce.Payment.Events;

public class PaymentCompletedEvent : DomainEvent
{
    public Guid PaymentId { get; }
    public string OrderId { get; }
    public string TransactionId { get; }

    public PaymentCompletedEvent(Guid paymentId, string orderId, string transactionId)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        TransactionId = transactionId;
    }
}