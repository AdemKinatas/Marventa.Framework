namespace Marventa.Framework.Core.Interfaces.Sagas;

public class SagaStepResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public object? NextEvent { get; set; }
    public TimeSpan? Delay { get; set; }
    public bool RequiresCompensation { get; set; }

    public static SagaStepResult Success(object? nextEvent = null, TimeSpan? delay = null)
        => new() { IsSuccess = true, NextEvent = nextEvent, Delay = delay };

    public static SagaStepResult Failed(string errorMessage, bool requiresCompensation = true)
        => new() { IsSuccess = false, ErrorMessage = errorMessage, RequiresCompensation = requiresCompensation };

    public static SagaStepResult Compensated()
        => new() { IsSuccess = true, RequiresCompensation = false };
}