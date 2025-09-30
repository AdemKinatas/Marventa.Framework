using BasicWebApi.Commands;
using BasicWebApi.DTOs;
using BasicWebApi.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BasicWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetAll()
    {
        _logger.LogInformation("Getting all products");
        var result = await _mediator.Send(new GetAllProductsQuery());
        return Ok(result);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(Guid id)
    {
        _logger.LogInformation("Getting product {ProductId}", id);
        var result = await _mediator.Send(new GetProductByIdQuery { Id = id });

        if (!result.Succeeded)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Create([FromBody] CreateProductCommand command)
    {
        _logger.LogInformation("Creating product {ProductName}", command.Name);
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }
}