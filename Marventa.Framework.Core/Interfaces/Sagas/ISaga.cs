namespace Marventa.Framework.Core.Interfaces.Sagas;

public interface ISaga
{
    Guid CorrelationId { get; set; }
    SagaStatus Status { get; set; }
    string CurrentStep { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? CompletedAt { get; set; }
    List<string> CompletedSteps { get; set; }
    string? ErrorMessage { get; set; }
}