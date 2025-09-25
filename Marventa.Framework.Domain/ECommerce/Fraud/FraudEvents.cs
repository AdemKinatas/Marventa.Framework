using Marventa.Framework.Core.Events;

namespace Marventa.Framework.Domain.ECommerce.Fraud;

public record FraudCheckCreatedDomainEvent(string FraudCheckId, string UserId, string OrderId, decimal Amount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType { get; } = nameof(FraudCheckCreatedDomainEvent);
}

public record FraudRuleTriggeredDomainEvent(string FraudCheckId, string RuleType, int Points, int TotalScore) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType { get; } = nameof(FraudRuleTriggeredDomainEvent);
}

public record FraudCheckApprovedDomainEvent(string FraudCheckId, string OrderId, string ReviewedBy) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType { get; } = nameof(FraudCheckApprovedDomainEvent);
}

public record FraudCheckRejectedDomainEvent(string FraudCheckId, string OrderId, string ReviewedBy, string Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType { get; } = nameof(FraudCheckRejectedDomainEvent);
}

public record FraudCheckBlockedDomainEvent(string FraudCheckId, string OrderId, string Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType { get; } = nameof(FraudCheckBlockedDomainEvent);
}

public record HighRiskFraudDetectedDomainEvent(string FraudCheckId, string OrderId, string UserId, int RiskScore) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType { get; } = nameof(HighRiskFraudDetectedDomainEvent);
}

public record SuspiciousActivityDetectedIntegrationEvent(string UserId, string ActivityType, string IpAddress, Dictionary<string, object> Metadata) : IIntegrationEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public string CorrelationId { get; } = Guid.NewGuid().ToString();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType { get; } = nameof(SuspiciousActivityDetectedIntegrationEvent);
}