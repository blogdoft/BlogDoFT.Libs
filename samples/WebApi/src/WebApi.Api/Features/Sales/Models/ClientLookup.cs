using WebApi.Api.Domain.Sales.ValueObjects;

namespace WebApi.Api.Features.Sales.Models;

public class ClientLookup
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }

    internal static ClientLookup From(Client? client) => new()
    {
        Id = client!.Id,
        Name = client!.Name,
    };
}
