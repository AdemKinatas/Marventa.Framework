using MassTransit;
using Marventa.Framework.Core.Interfaces;

namespace Marventa.Framework.Infrastructure.Sagas.Templates;

// Order Saga State
public class OrderSagaState : SagaStateMachineInstance, MassTransit.ISaga
{
    public Guid CorrelationId { get; set; }
    public string? CurrentState { get; set; }

    // Business Properties
    public string? OrderId { get; set; }
    public string? CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }

    // Saga State Properties
    public bool PaymentProcessed { get; set; }
    public bool InventoryReserved { get; set; }
    public bool ShippingArranged { get; set; }
    public string? FailureReason { get; set; }
    public DateTime? CompletedAt { get; set; }
}

// Saga Events
public interface IOrderSubmitted
{
    Guid OrderId { get; }
    string CustomerId { get; }
    decimal TotalAmount { get; }
    List<OrderItem> Items { get; }
}

public interface IPaymentProcessed
{
    Guid OrderId { get; }
    string TransactionId { get; }
    decimal Amount { get; }
}

public interface IPaymentFailed
{
    Guid OrderId { get; }
    string Reason { get; }
}

public interface IInventoryReserved
{
    Guid OrderId { get; }
    List<string> ReservationIds { get; }
}

public interface IInventoryReservationFailed
{
    Guid OrderId { get; }
    string Reason { get; }
}

public interface IShippingArranged
{
    Guid OrderId { get; }
    string TrackingNumber { get; }
    DateTime EstimatedDelivery { get; }
}

public class OrderItem
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

// Order Saga State Machine
public class OrderSagaStateMachine : MassTransitStateMachine<OrderSagaState>
{
    public State Processing { get; private set; } = null!;
    public State PaymentPending { get; private set; } = null!;
    public State InventoryPending { get; private set; } = null!;
    public State ShippingPending { get; private set; } = null!;
    public State Completed { get; private set; } = null!;
    public State Failed { get; private set; } = null!;

    public Event<IOrderSubmitted> OrderSubmitted { get; private set; } = null!;
    public Event<IPaymentProcessed> PaymentProcessed { get; private set; } = null!;
    public Event<IPaymentFailed> PaymentFailed { get; private set; } = null!;
    public Event<IInventoryReserved> InventoryReserved { get; private set; } = null!;
    public Event<IInventoryReservationFailed> InventoryReservationFailed { get; private set; } = null!;
    public Event<IShippingArranged> ShippingArranged { get; private set; } = null!;

    public OrderSagaStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderSubmitted, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentProcessed, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentFailed, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => InventoryReserved, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => InventoryReservationFailed, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => ShippingArranged, x => x.CorrelateById(context => context.Message.OrderId));

        Initially(
            When(OrderSubmitted)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId.ToString();
                    context.Saga.CustomerId = context.Message.CustomerId;
                    context.Saga.TotalAmount = context.Message.TotalAmount;
                    context.Saga.OrderDate = DateTime.UtcNow;
                })
                .TransitionTo(Processing)
                .Publish(context => new ProcessPaymentCommand
                {
                    OrderId = context.Message.OrderId,
                    Amount = context.Message.TotalAmount,
                    CustomerId = context.Message.CustomerId
                }));

        During(Processing,
            When(PaymentProcessed)
                .Then(context => context.Saga.PaymentProcessed = true)
                .TransitionTo(PaymentPending)
                .Publish(context => new ReserveInventoryCommand
                {
                    OrderId = Guid.Parse(context.Saga.OrderId!)
                }),
            When(PaymentFailed)
                .Then(context => context.Saga.FailureReason = context.Message.Reason)
                .TransitionTo(Failed)
                .Publish(context => new OrderFailedEvent
                {
                    OrderId = Guid.Parse(context.Saga.OrderId!),
                    Reason = context.Message.Reason
                }));

        During(PaymentPending,
            When(InventoryReserved)
                .Then(context => context.Saga.InventoryReserved = true)
                .TransitionTo(InventoryPending)
                .Publish(context => new ArrangeShippingCommand
                {
                    OrderId = Guid.Parse(context.Saga.OrderId!)
                }),
            When(InventoryReservationFailed)
                .Then(context => context.Saga.FailureReason = context.Message.Reason)
                .TransitionTo(Failed)
                .Publish(context => new RefundPaymentCommand
                {
                    OrderId = Guid.Parse(context.Saga.OrderId!),
                    Amount = context.Saga.TotalAmount
                }));

        During(InventoryPending,
            When(ShippingArranged)
                .Then(context =>
                {
                    context.Saga.ShippingArranged = true;
                    context.Saga.CompletedAt = DateTime.UtcNow;
                })
                .TransitionTo(Completed)
                .Publish(context => new OrderCompletedEvent
                {
                    OrderId = Guid.Parse(context.Saga.OrderId!),
                    CustomerId = context.Saga.CustomerId!,
                    CompletedAt = context.Saga.CompletedAt!.Value
                })
                .Finalize());

        SetCompletedWhenFinalized();
    }
}

// Commands
public class ProcessPaymentCommand
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string CustomerId { get; set; } = string.Empty;
}

public class ReserveInventoryCommand
{
    public Guid OrderId { get; set; }
}

public class ArrangeShippingCommand
{
    public Guid OrderId { get; set; }
}

public class RefundPaymentCommand
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
}

// Events
public class OrderCompletedEvent
{
    public Guid OrderId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
}

public class OrderFailedEvent
{
    public Guid OrderId { get; set; }
    public string Reason { get; set; } = string.Empty;
}