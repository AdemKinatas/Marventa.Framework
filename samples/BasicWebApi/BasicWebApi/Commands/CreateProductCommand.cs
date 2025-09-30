using BasicWebApi.DTOs;
using BasicWebApi.Entities;
using BasicWebApi.Data;
using MediatR;

namespace BasicWebApi.Commands;

public class CreateProductCommand : IRequest<ApiResponse<ProductDto>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ApiResponse<ProductDto>>
{
    private readonly ApplicationDbContext _context;

    public CreateProductCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CreatedDate = product.CreatedDate
        };

        return ApiResponse<ProductDto>.SuccessResult(dto, "Product created successfully");
    }
}