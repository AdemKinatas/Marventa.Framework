using Marventa.Framework.Core.Entities;
using Marventa.Framework.Core.Interfaces.Events;
using Marventa.Framework.Core.Events;
using Marventa.Framework.Domain.ValueObjects;
using Marventa.Framework.Domain.ECommerce.Payment.Events;

namespace Marventa.Framework.Domain.ECommerce.Payment;

public class Payment : BaseEntity
{
    public string OrderId { get; private set; } = string.Empty;
    public string CustomerId { get; private set; } = string.Empty;
    public Money Amount { get; private set; } = null!;
    public PaymentStatus Status { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string? TransactionId { get; private set; }
    public string? GatewayResponse { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? FailureReason { get; private set; }
    public int RetryCount { get; private set; }
    public string? TenantId { get; set; }

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected Payment() { }

    public Payment(string orderId, string customerId, Money amount, PaymentMethod method)
    {
        OrderId = orderId;
        CustomerId = customerId;
        Amount = amount;
        Method = method;
        Status = PaymentStatus.Pending;

        _domainEvents.Add(new PaymentInitiatedEvent(Id, orderId, amount.Amount, amount.Currency.Code));
    }

    public void MarkAsProcessing(string transactionId)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Cannot process payment in status {Status}");

        Status = PaymentStatus.Processing;
        TransactionId = transactionId;
        UpdatedDate = DateTime.UtcNow;
    }

    public void MarkAsSuccessful(string gatewayResponse)
    {
        if (Status != PaymentStatus.Processing)
            throw new InvalidOperationException($"Cannot complete payment in status {Status}");

        Status = PaymentStatus.Successful;
        GatewayResponse = gatewayResponse;
        ProcessedAt = DateTime.UtcNow;
        UpdatedDate = DateTime.UtcNow;

        _domainEvents.Add(new PaymentCompletedEvent(Id, OrderId, TransactionId!));
    }

    public void MarkAsFailed(string reason)
    {
        Status = PaymentStatus.Failed;
        FailureReason = reason;
        RetryCount++;
        UpdatedDate = DateTime.UtcNow;

        _domainEvents.Add(new PaymentFailedEvent(Id, OrderId, reason));
    }

    public void Refund(Money refundAmount)
    {
        if (Status != PaymentStatus.Successful)
            throw new InvalidOperationException("Can only refund successful payments");

        if (refundAmount > Amount)
            throw new InvalidOperationException("Refund amount cannot exceed payment amount");

        Status = PaymentStatus.Refunded;
        UpdatedDate = DateTime.UtcNow;

        _domainEvents.Add(new PaymentRefundedEvent(Id, OrderId, refundAmount.Amount));
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}