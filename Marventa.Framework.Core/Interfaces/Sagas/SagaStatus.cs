namespace Marventa.Framework.Core.Interfaces.Sagas;

/// <summary>
/// Saga status enumeration
/// </summary>
public enum SagaStatus
{
    Started,
    InProgress,
    Completed,
    Compensating,
    Compensated,
    Failed
}