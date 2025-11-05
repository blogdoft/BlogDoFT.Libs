using FluentValidation;

namespace WebApi.Api.Domain.Sales;

public class SaleItemValidator : AbstractValidator<SaleItemEntity>
{
    public SaleItemValidator()
    {
        RuleFor(item => item.Product)
            .NotNull()
            .WithMessage("A sale item must have a product");

        RuleFor(item => item.Quantity)
            .GreaterThan(0)
            .WithMessage("A sale item quantity must be greater than zero.");

        RuleFor(item => item.Discount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("A sale item discount must be greater than or equal to zero");
    }
}
