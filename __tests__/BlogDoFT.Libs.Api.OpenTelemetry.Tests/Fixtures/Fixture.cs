namespace BlogDoFT.Libs.Api.OpenTelemetry.Tests.Fixtures;

public static class Fixture
{
    private static readonly Faker _faker = new("pt_BR");

    public static Faker Get() => _faker;

    public static Faker<TObject> Get<TObject>()
        where TObject : class => new("pt_BR");
}
