namespace Marventa.Framework.Domain.ECommerce.Payment;

public enum PaymentStatus
{
    Pending,
    Processing,
    Successful,
    Failed,
    Refunded,
    PartiallyRefunded,
    Cancelled
}