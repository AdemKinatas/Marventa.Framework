using MediatR;

namespace Marventa.Framework.Application.ECommerce.Fraud.Commands;

public record ReviewFraudCheckCommand(
    string FraudCheckId,
    string ReviewAction, // "approve", "reject"
    string ReviewedBy,
    string? Notes = null
) : IRequest<ReviewFraudCheckResponse>;

public record ReviewFraudCheckResponse(
    string FraudCheckId,
    string Status,
    DateTime ReviewedAt
);