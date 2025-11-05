using BlogDoFT.Libs.ResultPattern;
using WebApi.Api.Domain.Sales;
using WebApi.Api.Domain.Sales.ValueObjects;

namespace WebApi.Api.Features.Sales.Infrastructure.Models;

public class SaleModel
{
    public int Id { get; set; }

    public Guid NavigationId { get; set; }

    public Guid ClientId { get; set; }
    public string ClientName { get; set; }

    public int Installments { get; set; }

    public string PaymentMethod { get; set; }

    public ICollection<SaleItemModel> Items { get; set; } = [];

    internal static SaleModel From(SaleEntity entity) => new()
    {
        NavigationId = entity.Id,
        ClientId = entity.Client!.Id,
        ClientName = entity.Client.Name,
        Installments = entity.Installments,
        PaymentMethod = entity.PaymentMethod.ToString(),
        Items = entity.Items.Select(item => SaleItemModel.From(item)).ToList(),
    };

    internal Result<SaleEntity> ToEntity() => new SaleFactory()
        .WithId(NavigationId)
        .WithClient(Client.Create(ClientId, ClientName))
        .WithInstallments(Installments)
        .WithPaymentMethod(PaymentMethod)
        .WithSaleItem(Items.Select(item => SaleItemEntity.Create(
            product: Product.Create(item.ProductId, item.ProductDescription ?? "undefine"),
            quantity: item.Quantity,
            unitValue: item.UnitValue
        )))
        .Build();
}
