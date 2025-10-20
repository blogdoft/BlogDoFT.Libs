using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BlogDoFT.Libs.DapperUtils.Postgres.Tests;

public class NpgConnectionFactoryTests
{
    [Fact]
    public void Should_CreateConnection_When_ConnectionStringExists()
    {
        // Given
        var config = Substitute.For<IConfiguration>();
        config.GetConnectionString("Default").Returns("Host=localhost;Username=postgres;Password=secret;Database=testdb");

        var factory = new NpgConnectionFactory(config);

        // When
        var connection = factory.GetNewConnection();

        // Then
        connection.ShouldNotBeNull();
        connection.ShouldBeOfType<NpgsqlConnection>();
        connection.ConnectionString.ShouldContain("Host=localhost");
    }

    [Fact]
    public void Should_ThrowArgumentNullException_When_ConnectionStringIsMissing()
    {
        // Given
        var config = Substitute.For<IConfiguration>();
        config.GetConnectionString("Default").Returns((string?)null);

        var factory = new NpgConnectionFactory(config);

        // When & Assert
        var ex = Should.Throw<ArgumentNullException>(() => factory.GetNewConnection());
        ex.ParamName.ShouldBe("ConnectionStrings:Default");
    }
}
