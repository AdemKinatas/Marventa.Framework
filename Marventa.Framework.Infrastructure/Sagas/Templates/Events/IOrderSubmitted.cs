namespace Marventa.Framework.Infrastructure.Sagas.Templates.Events;

public interface IOrderSubmitted
{
    Guid OrderId { get; }
    string CustomerId { get; }
    decimal TotalAmount { get; }
    List<OrderItem> Items { get; }
}