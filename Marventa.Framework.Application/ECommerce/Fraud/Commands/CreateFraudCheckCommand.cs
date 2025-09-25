using MediatR;

namespace Marventa.Framework.Application.ECommerce.Fraud.Commands;

public record CreateFraudCheckCommand(
    string UserId,
    string OrderId,
    string IpAddress,
    string UserAgent,
    decimal Amount,
    string Currency
) : IRequest<CreateFraudCheckResponse>;

public record CreateFraudCheckResponse(
    string FraudCheckId,
    string Status,
    string RiskLevel,
    int RiskScore
);