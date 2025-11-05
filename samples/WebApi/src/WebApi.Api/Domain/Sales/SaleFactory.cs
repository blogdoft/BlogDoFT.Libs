using BlogDoFT.Libs.Extensions;
using BlogDoFT.Libs.ResultPattern;
using WebApi.Api.Domain.Sales.ValueObjects;
using WebApi.Api.Features.Sales.Models;

namespace WebApi.Api.Domain.Sales;

public class SaleFactory
{
    private Client? _client;
    private int _installments = 1;
    private PaymentMethod _paymentMethod = PaymentMethod.Cash;
    private IEnumerable<Result<SaleItemEntity>> _items = [];
    private Guid? _id;

    public Result<SaleEntity> Build()
    {
        var sale = new SaleEntity()
        {
            Id = _id ?? Guid.NewGuid(),
            Client = _client,
            PaymentMethod = _paymentMethod,
            Installments = _installments,
        };

        foreach (var item in _items)
        {
            if (item.IsFailure)
            {
                return item.Failure;
            }

            sale.AddItem(item.Value);
        }

        var validation = new SaleValidator().Validate(sale);

        if (!validation.IsValid)
        {
            return Failure.ValidationError with
            {
                Message = $"Some errors has found:" + Environment.NewLine +
                    string.Join(
                        separator: Environment.NewLine,
                        values: validation.Errors.Select(e => e.ErrorMessage))
            };
        }

        return sale;
    }

    internal SaleFactory WithId(Guid id)
    {
        _id = id;
        return this;
    }

    internal SaleFactory WithClient(Client client)
    {
        _client = client;
        return this;
    }

    internal SaleFactory WithInstallments(int installments)
    {
        _installments = installments;
        return this;
    }

    internal SaleFactory WithPaymentMethod(PaymentMethod paymentMethod)
    {
        _paymentMethod = paymentMethod;
        return this;
    }

    internal SaleFactory WithPaymentMethod(string paymentMethod)
    {
        _paymentMethod = paymentMethod.ToEnum<PaymentMethod>();
        return this;
    }

    internal SaleFactory WithSaleItem(IEnumerable<Result<SaleItemEntity>> items)
    {
        _items = items;
        return this;
    }
}
