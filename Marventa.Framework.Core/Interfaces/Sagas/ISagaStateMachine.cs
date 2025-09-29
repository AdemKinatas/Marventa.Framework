namespace Marventa.Framework.Core.Interfaces.Sagas;

public interface ISagaStateMachine<TInstance> : ISaga
    where TInstance : class, ISaga
{
}