using WebApi.Api.Domain.Sales.ValueObjects;
using WebApi.Api.Features.Sales.Models;

namespace WebApi.Api.Domain.Sales;

public class SaleEntity
{
    public Guid Id { get; init; }

    public Client? Client { get; init; }

    public PaymentMethod PaymentMethod { get; init; }

    public int Installments { get; init; }

    public ICollection<SaleItemEntity> Items { get; } = [];

    public decimal Amount => Items.Sum(i => i.Amount);

    public void AddItem(SaleItemEntity item)
    {
        Items.Add(item);
    }
}
