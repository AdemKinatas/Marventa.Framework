namespace Marventa.Framework.Domain.ECommerce.Shipping;

public enum ShippingStatus
{
    Pending,
    Shipped,
    InTransit,
    OutForDelivery,
    Delivered,
    Returned,
    Lost,
    Cancelled
}