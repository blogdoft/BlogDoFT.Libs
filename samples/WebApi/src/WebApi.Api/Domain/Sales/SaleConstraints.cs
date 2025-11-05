namespace WebApi.Api.Domain.Sales;

public static class SaleConstraints
{
    public const int MaxItemPerSale = 20;
    public const int MaxDiscountLayer = 15;
    public const int SecondDiscountLayer = 10;
    public const int FirstDiscountLayer = 5;

    public const decimal FirstDiscountPercent = 1.25M;
    public const decimal SecondDiscountPercent = 1.5M;
    public const decimal MaxDiscountPercent = 2M;
}
