using Marventa.Framework.Core.Entities;
using Marventa.Framework.Core.Interfaces.Events;
using Marventa.Framework.Core.Events;
using Marventa.Framework.Domain.Common;

namespace Marventa.Framework.Domain.ECommerce.Fraud;

public class FraudCheck : BaseAggregateRoot
{
    public string UserId { get; private set; } = string.Empty;
    public string OrderId { get; private set; } = string.Empty;
    public string IpAddress { get; private set; } = string.Empty;
    public string UserAgent { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public FraudStatus Status { get; private set; }
    public FraudRiskLevel RiskLevel { get; private set; }
    public int RiskScore { get; private set; }
    public List<FraudRule> TriggeredRules { get; private set; } = new();
    public string? ReviewNotes { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string? ReviewedBy { get; private set; }

    private FraudCheck() { }

    public static FraudCheck Create(string userId, string orderId, string ipAddress, string userAgent, decimal amount, string currency)
    {
        var fraudCheck = new FraudCheck
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OrderId = orderId,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Amount = amount,
            Currency = currency,
            Status = FraudStatus.Pending,
            RiskLevel = FraudRiskLevel.Low,
            RiskScore = 0,
            CreatedDate = DateTime.UtcNow
        };

        fraudCheck.AddDomainEvent(new FraudCheckCreatedDomainEvent(fraudCheck.Id.ToString(), userId, orderId, amount));
        return fraudCheck;
    }

    public void AddRule(FraudRule rule)
    {
        TriggeredRules.Add(rule);
        RiskScore += rule.Points;
        UpdateRiskLevel();
        UpdatedDate = DateTime.UtcNow;

        AddDomainEvent(new FraudRuleTriggeredDomainEvent(Id.ToString(), rule.RuleType, rule.Points, RiskScore));
    }

    public void Approve(string reviewedBy, string? notes = null)
    {
        Status = FraudStatus.Approved;
        ReviewedBy = reviewedBy;
        ReviewNotes = notes;
        ReviewedAt = DateTime.UtcNow;
        UpdatedDate = DateTime.UtcNow;

        AddDomainEvent(new FraudCheckApprovedDomainEvent(Id.ToString(), OrderId, reviewedBy));
    }

    public void Reject(string reviewedBy, string reason)
    {
        Status = FraudStatus.Rejected;
        ReviewedBy = reviewedBy;
        ReviewNotes = reason;
        ReviewedAt = DateTime.UtcNow;
        UpdatedDate = DateTime.UtcNow;

        AddDomainEvent(new FraudCheckRejectedDomainEvent(Id.ToString(), OrderId, reviewedBy, reason));
    }

    public void MarkAsBlocked(string reason)
    {
        Status = FraudStatus.Blocked;
        ReviewNotes = reason;
        ReviewedAt = DateTime.UtcNow;
        UpdatedDate = DateTime.UtcNow;

        AddDomainEvent(new FraudCheckBlockedDomainEvent(Id.ToString(), OrderId, reason));
    }

    private void UpdateRiskLevel()
    {
        RiskLevel = RiskScore switch
        {
            >= 80 => FraudRiskLevel.Critical,
            >= 60 => FraudRiskLevel.High,
            >= 40 => FraudRiskLevel.Medium,
            >= 20 => FraudRiskLevel.Low,
            _ => FraudRiskLevel.Minimal
        };

        if (RiskScore >= 80)
        {
            Status = FraudStatus.Blocked;
            AddDomainEvent(new HighRiskFraudDetectedDomainEvent(Id.ToString(), OrderId, UserId, RiskScore));
        }
    }
}

public class FraudRule
{
    public string RuleType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Points { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public enum FraudStatus
{
    Pending,
    Approved,
    Rejected,
    Blocked,
    UnderReview
}

public enum FraudRiskLevel
{
    Minimal,
    Low,
    Medium,
    High,
    Critical
}