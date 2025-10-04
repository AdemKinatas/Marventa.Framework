using Marventa.Framework.ApiResponse;
using Marventa.Framework.Core.Domain.Specification;
using Marventa.Framework.TestApi.Data;
using Marventa.Framework.TestApi.Data.Specifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Marventa.Framework.TestApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpecificationTestController : ControllerBase
{
    private readonly TestDbContext _context;
    private readonly ILogger<SpecificationTestController> _logger;

    public SpecificationTestController(TestDbContext context, ILogger<SpecificationTestController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Seeds test products
    /// </summary>
    [HttpPost("seed")]
    public async Task<IActionResult> SeedProducts()
    {
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "Laptop", Price = 1200m, Description = "High-end laptop", IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-10) },
            new() { Id = Guid.NewGuid(), Name = "Mouse", Price = 25m, Description = "Wireless mouse", IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-5) },
            new() { Id = Guid.NewGuid(), Name = "Keyboard", Price = 80m, Description = "Mechanical keyboard", IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-3) },
            new() { Id = Guid.NewGuid(), Name = "Monitor", Price = 350m, Description = "4K monitor", IsActive = true, CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new() { Id = Guid.NewGuid(), Name = "Headphones", Price = 150m, Description = "Noise-canceling", IsActive = false, CreatedAt = DateTime.UtcNow.AddDays(-7) },
            new() { Id = Guid.NewGuid(), Name = "Webcam", Price = 75m, Description = "HD webcam", IsActive = true, CreatedAt = DateTime.UtcNow },
        };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        var response = ApiResponse<object>.SuccessResponse(
            new { productsCreated = products.Count },
            "Test products seeded successfully"
        );

        return Ok(response);
    }

    /// <summary>
    /// Gets active products using specification
    /// </summary>
    [HttpGet("active-products")]
    public async Task<IActionResult> GetActiveProducts()
    {
        var spec = new ActiveProductsSpecification();
        var query = SpecificationEvaluator.GetQuery(_context.Products.AsQueryable(), spec);
        var products = await query.ToListAsync();

        var response = ApiResponse<object>.SuccessResponse(
            products,
            $"Retrieved {products.Count} active products using ActiveProductsSpecification"
        );

        return Ok(response);
    }

    /// <summary>
    /// Gets products by price range using specification
    /// </summary>
    [HttpGet("by-price-range")]
    public async Task<IActionResult> GetProductsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {
        var spec = new ProductsByPriceRangeSpecification(minPrice, maxPrice);
        var query = SpecificationEvaluator.GetQuery(_context.Products.AsQueryable(), spec);
        var products = await query.ToListAsync();

        var response = ApiResponse<object>.SuccessResponse(
            products,
            $"Retrieved {products.Count} products between ${minPrice} and ${maxPrice} using ProductsByPriceRangeSpecification"
        );

        return Ok(response);
    }

    /// <summary>
    /// Gets paginated products using specification
    /// </summary>
    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginatedProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 3)
    {
        var spec = new ProductsWithPaginationSpecification(pageNumber, pageSize);
        var query = SpecificationEvaluator.GetQuery(_context.Products.AsQueryable(), spec);
        var products = await query.ToListAsync();

        var totalCount = await _context.Products.Where(p => p.IsActive).CountAsync();

        var response = ApiResponse<object>.SuccessResponse(
            new
            {
                products,
                pagination = new
                {
                    pageNumber,
                    pageSize,
                    totalCount,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                }
            },
            $"Retrieved page {pageNumber} using ProductsWithPaginationSpecification"
        );

        return Ok(response);
    }

    /// <summary>
    /// Gets recent products using specification
    /// </summary>
    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentProducts([FromQuery] int days = 7)
    {
        var spec = new RecentProductsSpecification(days);
        var query = SpecificationEvaluator.GetQuery(_context.Products.AsQueryable(), spec);
        var products = await query.ToListAsync();

        var response = ApiResponse<object>.SuccessResponse(
            products,
            $"Retrieved {products.Count} products created in the last {days} days using RecentProductsSpecification"
        );

        return Ok(response);
    }

    /// <summary>
    /// Demonstrates combining multiple specifications with AND logic
    /// </summary>
    [HttpGet("combined")]
    public async Task<IActionResult> GetCombinedSpecification([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice, [FromQuery] int days = 7)
    {
        // Manually combine specifications
        var priceSpec = new ProductsByPriceRangeSpecification(minPrice, maxPrice);
        var recentSpec = new RecentProductsSpecification(days);

        // Apply both criteria
        var query = _context.Products.AsQueryable();
        query = SpecificationEvaluator.GetQuery(query, priceSpec);

        // Add additional filtering
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        query = query.Where(p => p.CreatedAt >= cutoffDate);

        var products = await query.ToListAsync();

        var response = ApiResponse<object>.SuccessResponse(
            products,
            $"Retrieved {products.Count} products with combined specifications (price range ${minPrice}-${maxPrice} and created in last {days} days)"
        );

        return Ok(response);
    }
}
