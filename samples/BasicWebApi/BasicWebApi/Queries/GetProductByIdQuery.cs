using BasicWebApi.DTOs;
using BasicWebApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BasicWebApi.Queries;

public class GetProductByIdQuery : IRequest<ApiResponse<ProductDto>>
{
    public Guid Id { get; set; }
}

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ApiResponse<ProductDto>>
{
    private readonly ApplicationDbContext _context;

    public GetProductByIdQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .Where(p => p.Id == request.Id)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                CreatedDate = p.CreatedDate
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (product == null)
            return ApiResponse<ProductDto>.FailureResult("Product not found");

        return ApiResponse<ProductDto>.SuccessResult(product);
    }
}