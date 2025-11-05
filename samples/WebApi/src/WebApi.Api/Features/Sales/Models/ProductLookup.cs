using WebApi.Api.Domain.Sales.ValueObjects;

namespace WebApi.Api.Features.Sales.Models;

public class ProductLookup
{
    public required Guid Id { get; set; }
    public required string Description { get; set; }

    internal static ProductLookup From(Product product) => new()
    {
        Id = product.Id,
        Description = product.Description,
    };
}
