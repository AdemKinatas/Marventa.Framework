using Marventa.Framework.Core.Interfaces.Events;
using Marventa.Framework.Core.Events;

namespace Marventa.Framework.Domain.ECommerce.Payment.Events;

public class PaymentInitiatedEvent : DomainEvent
{
    public Guid PaymentId { get; }
    public string OrderId { get; }
    public decimal Amount { get; }
    public string Currency { get; }

    public PaymentInitiatedEvent(Guid paymentId, string orderId, decimal amount, string currency)
    {
        PaymentId = paymentId;
        OrderId = orderId;
        Amount = amount;
        Currency = currency;
    }
}