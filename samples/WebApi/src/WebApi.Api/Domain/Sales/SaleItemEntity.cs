using BlogDoFT.Libs.ResultPattern;
using WebApi.Api.Domain.Sales.ValueObjects;

namespace WebApi.Api.Domain.Sales;

public class SaleItemEntity
{
    private const int MaxItemPerSale = 20;
    private SaleItemEntity(
        Product product,
        decimal quantity,
        decimal unitValue,
        decimal discount = 0)
    {
        Product = product;
        Quantity = quantity;
        UnitValue = unitValue;
        Discount = discount;
        Amount = quantity * unitValue * (1 + discount);
    }
    public Product Product { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal Discount { get; private set; }
    public decimal UnitValue { get; private set; }
    public decimal Amount { get; }

    public static Result<SaleItemEntity> Create(
        Product product,
        decimal quantity,
        decimal unitValue)
    {
        return DiscountConstraint
            .GetDiscountPercent(quantity)
            .Then<decimal, SaleItemEntity>((discount) => new SaleItemEntity(
                product,
                quantity,
                unitValue,
                discount));
    }
}

