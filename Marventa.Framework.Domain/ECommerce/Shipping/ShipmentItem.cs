namespace Marventa.Framework.Domain.ECommerce.Shipping;

public class ShipmentItem
{
    public string ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public decimal Weight { get; private set; }

    public ShipmentItem(string productId, string productName, int quantity, decimal weight)
    {
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        Weight = weight;
    }
}