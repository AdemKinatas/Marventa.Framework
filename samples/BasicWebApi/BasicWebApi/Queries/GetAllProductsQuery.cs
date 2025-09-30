using BasicWebApi.DTOs;
using BasicWebApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BasicWebApi.Queries;

public class GetAllProductsQuery : IRequest<ApiResponse<List<ProductDto>>>
{
}

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, ApiResponse<List<ProductDto>>>
{
    private readonly ApplicationDbContext _context;

    public GetAllProductsQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _context.Products
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                CreatedDate = p.CreatedDate
            })
            .ToListAsync(cancellationToken);

        return ApiResponse<List<ProductDto>>.SuccessResult(products);
    }
}