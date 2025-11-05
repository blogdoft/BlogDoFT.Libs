using FluentValidation;

namespace WebApi.Api.Domain.Sales;

public class SaleValidator : AbstractValidator<SaleEntity>
{
    private const int MinInstallment = 1;
    private const int MaxInstallment = 10;
    public SaleValidator()
    {
        RuleFor(sale => sale.Id)
            .NotEmpty()
            .WithMessage("Sale must have a valid id")
            .NotNull()
            .WithMessage("Sale must have a id");

        RuleFor(sale => sale.Client)
            .NotNull()
            .WithMessage("A sale must have a client");

        RuleFor(sale => sale.Installments)
            .InclusiveBetween(from: MinInstallment, to: MaxInstallment)
            .WithMessage($"A sale installment must be between {MinInstallment} and {MaxInstallment}.");

        RuleFor(sale => sale.Items)
            .NotEmpty()
            .WithMessage("A sale must have at least one item");

        RuleForEach(sale => sale.Items)
            .SetValidator(new SaleItemValidator());
    }
}
