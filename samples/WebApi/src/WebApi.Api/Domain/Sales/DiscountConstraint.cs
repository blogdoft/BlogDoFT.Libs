using BlogDoFT.Libs.ResultPattern;

namespace WebApi.Api.Domain.Sales;

public class DiscountConstraint
{
    private const string Negative = "Quantity should be greater than zero";
    private const string TooMany = "Too many items";
    private static readonly (Predicate<decimal> Match, Func<Result<decimal>> Outcome)[] _rules =
    [
        (q => q <= 0, () => Failure.ValidationError with { Message = Negative }),
        (q => q > 20, () => Failure.ValidationError with { Message = TooMany }),
        (q => q > 15, () => 0.02m),
        (q => q > 10, () => 0.015m),
        (q => q > 5,  () => 0.01m),
        (q => true,   () => 0.00m),
    ];

    public static Result<decimal> GetDiscountPercent(decimal quantity) =>
        _rules.First(r => r.Match(quantity)).Outcome();
}
