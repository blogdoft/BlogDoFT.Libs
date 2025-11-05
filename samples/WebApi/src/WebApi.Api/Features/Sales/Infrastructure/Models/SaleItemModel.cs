using WebApi.Api.Domain.Sales;

namespace WebApi.Api.Features.Sales.Infrastructure.Models;

public class SaleItemModel
{
    public int Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductDescription { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitValue { get; set; }
    public decimal Discount { get; set; }
    public int SaleId { get; set; }
    public SaleModel Sale { get; set; }

    internal static SaleItemModel From(SaleItemEntity item) => new()
    {
        ProductId = item.Product.Id,
        ProductDescription = item.Product.Description,
        Quantity = item.Quantity,
        UnitValue = item.UnitValue,
        Discount = item.Discount,
    };
}
