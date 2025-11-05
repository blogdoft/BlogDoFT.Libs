namespace WebApi.Api.Domain.Sales.ValueObjects;

public sealed record class Client
{
    private Client(Guid id, string Name)
    {
        Id = id;
        this.Name = Name;
    }

    public Guid Id { get; }
    public string Name { get; }

    public static Client Create(Guid id, string name) => new(id, name);
}
