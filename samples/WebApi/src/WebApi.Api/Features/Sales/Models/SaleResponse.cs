

using BlogDoFT.Libs.ResultPattern;
using WebApi.Api.Domain.Sales;

namespace WebApi.Api.Features.Sales.Models;

public class SaleResponse
{
    public Guid Id { get; set; }
    public required ClientLookup Client { get; set; }

    public required PaymentMethod PaymentMethod { get; set; }

    public required int Installments { get; set; } = 1;

    public required IEnumerable<SaleItemResponse> Items { get; set; } = [];

    internal static Result<SaleResponse> From(SaleEntity entity)
    {
        return new SaleResponse()
        {
            Id = entity.Id,
            Client = ClientLookup.From(entity.Client),
            PaymentMethod = entity.PaymentMethod,
            Installments = entity.Installments,
            Items = entity.Items.Select(item => SaleItemResponse.From(item)),
        };
    }
}
