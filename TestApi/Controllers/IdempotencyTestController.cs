using Marventa.Framework.ApiResponse;
using Marventa.Framework.TestApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace Marventa.Framework.TestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdempotencyTestController : ControllerBase
{
    private readonly TestDbContext _context;
    private readonly ILogger<IdempotencyTestController> _logger;
    private static int _requestCounter = 0;

    public IdempotencyTestController(TestDbContext context, ILogger<IdempotencyTestController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Creates an order. Send with "Idempotency-Key" header to test idempotency
    /// Example: Idempotency-Key: order-12345
    /// </summary>
    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var requestNumber = Interlocked.Increment(ref _requestCounter);
        _logger.LogInformation("Processing create order request #{RequestNumber}", requestNumber);

        // Simulate order creation
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerName = request.CustomerName,
            TotalAmount = request.Items.Sum(i => i.Price * i.Quantity),
            OrderDate = DateTime.UtcNow,
            Items = request.Items.Select(i => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Order created: {OrderId} (Request #{RequestNumber})", order.Id, requestNumber);

        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                orderId = order.Id,
                customerName = order.CustomerName,
                totalAmount = order.TotalAmount,
                itemCount = order.Items.Count,
                requestNumber,
                message = "If you send the same Idempotency-Key header, you'll get this exact response cached"
            },
            "Order created successfully"
        );

        return Ok(response);
    }

    /// <summary>
    /// Updates a product price. Test idempotency with PUT
    /// Example: Idempotency-Key: update-price-12345
    /// </summary>
    [HttpPut("update-price/{productId}")]
    public async Task<IActionResult> UpdatePrice(Guid productId, [FromBody] UpdatePriceRequest request)
    {
        var requestNumber = Interlocked.Increment(ref _requestCounter);
        _logger.LogInformation("Processing price update request #{RequestNumber} for product {ProductId}", requestNumber, productId);

        var product = await _context.Products.FindAsync(productId);
        if (product == null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Product not found"));
        }

        var oldPrice = product.Price;
        product.Price = request.NewPrice;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Product price updated: {ProductId} from ${OldPrice} to ${NewPrice} (Request #{RequestNumber})",
            productId, oldPrice, request.NewPrice, requestNumber);

        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                productId,
                productName = product.Name,
                oldPrice,
                newPrice = request.NewPrice,
                requestNumber,
                message = "With the same Idempotency-Key, this response will be cached"
            },
            "Price updated successfully"
        );

        return Ok(response);
    }

    /// <summary>
    /// Deletes a product. Test idempotency with DELETE
    /// Example: Idempotency-Key: delete-product-12345
    /// </summary>
    [HttpDelete("delete-product/{productId}")]
    public async Task<IActionResult> DeleteProduct(Guid productId)
    {
        var requestNumber = Interlocked.Increment(ref _requestCounter);
        _logger.LogInformation("Processing delete request #{RequestNumber} for product {ProductId}", requestNumber, productId);

        var product = await _context.Products.FindAsync(productId);
        if (product == null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Product not found"));
        }

        var productName = product.Name;
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Product deleted: {ProductId} - {ProductName} (Request #{RequestNumber})",
            productId, productName, requestNumber);

        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                productId,
                productName,
                requestNumber,
                message = "With idempotency, subsequent requests will return this cached response even though the product is already deleted"
            },
            "Product deleted successfully"
        );

        return Ok(response);
    }

    /// <summary>
    /// Test endpoint that fails - to verify idempotency doesn't cache errors
    /// </summary>
    [HttpPost("fail")]
    public IActionResult FailingEndpoint()
    {
        _logger.LogWarning("Failing endpoint called - this will not be cached by idempotency middleware");
        throw new InvalidOperationException("This endpoint always fails - idempotency should NOT cache 5xx responses");
    }
}

public class CreateOrderRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public List<OrderItemRequest> Items { get; set; } = new();
}

public class OrderItemRequest
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class UpdatePriceRequest
{
    public decimal NewPrice { get; set; }
}
