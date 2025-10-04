using Marventa.Framework.Core.Domain.Specification;

namespace Marventa.Framework.TestApi.Data.Specifications;

public class ActiveProductsSpecification : BaseSpecification<Product>
{
    public ActiveProductsSpecification() : base(p => p.IsActive)
    {
        ApplyOrderBy(p => p.Name);
    }
}

public class ProductsByPriceRangeSpecification : BaseSpecification<Product>
{
    public ProductsByPriceRangeSpecification(decimal minPrice, decimal maxPrice)
        : base(p => p.Price >= minPrice && p.Price <= maxPrice)
    {
        ApplyOrderByDescending(p => p.Price);
    }
}

public class ProductsWithPaginationSpecification : BaseSpecification<Product>
{
    public ProductsWithPaginationSpecification(int pageNumber, int pageSize)
        : base(p => p.IsActive)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        ApplyOrderBy(p => p.CreatedAt);
    }
}

public class RecentProductsSpecification : BaseSpecification<Product>
{
    public RecentProductsSpecification(int days = 7)
        : base(p => p.CreatedAt >= DateTime.UtcNow.AddDays(-days) && p.IsActive)
    {
        ApplyOrderByDescending(p => p.CreatedAt);
    }
}
