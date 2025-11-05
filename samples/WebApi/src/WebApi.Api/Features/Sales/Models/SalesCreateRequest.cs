namespace WebApi.Api.Features.Sales.Models;

public class SalesCreateRequest
{
    public required ClientLookup Client { get; set; }

    public required PaymentMethod PaymentMethod { get; set; }

    public required int Installments { get; set; } = 1;

    public required IEnumerable<SaleItem> Items { get; set; } = [];
}
