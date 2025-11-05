namespace WebApi.Api.Domain.Sales.ValueObjects;

public record class Product
{
    private Product(Guid id, string description)
    {
        Id = id;
        Description = description;
    }

    public Guid Id { get; }

    public string Description { get; }

    public static Product Create(Guid id, string description)
        => new(id, description);
}
