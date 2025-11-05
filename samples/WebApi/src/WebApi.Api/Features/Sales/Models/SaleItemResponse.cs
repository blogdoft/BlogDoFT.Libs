using WebApi.Api.Domain.Sales;

namespace WebApi.Api.Features.Sales.Models;

public class SaleItemResponse
{
    public required ProductLookup Product { get; set; }

    public required decimal Quantity { get; set; }

    public required decimal UnitValue { get; set; }

    public required decimal Discount { get; set; }

    internal static SaleItemResponse From(SaleItemEntity item) => new()
    {
        Product = ProductLookup.From(item.Product),
        Quantity = item.Quantity,
        UnitValue = item.UnitValue,
        Discount = item.Discount,
    };
}
