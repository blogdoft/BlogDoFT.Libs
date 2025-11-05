namespace WebApi.Api.Features.Sales.Models;

public class SaleItem
{
    public required ProductLookup Product { get; set; }

    public required decimal Quantity { get; set; }

    public required decimal UnitValue { get; set; }
}
