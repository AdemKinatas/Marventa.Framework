namespace Marventa.Framework.Core.Interfaces.Sagas;

public interface ISagaStep<TSaga> where TSaga : class, ISaga
{
    string StepName { get; }
    Task<SagaStepResult> ExecuteAsync(TSaga saga, object @event, CancellationToken cancellationToken = default);
    Task<SagaStepResult> CompensateAsync(TSaga saga, string reason, CancellationToken cancellationToken = default);
}