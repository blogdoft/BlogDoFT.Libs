using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BlogDoFT.Libs.DapperUtils.Postgres.Tests;

public class NpgConnectionFactoryTests
{
    [Fact]
    public void Should_CreateConnection_When_ConnectionStringExists()
    {
        // Arrange
        var config = Substitute.For<IConfiguration>();
        config.GetConnectionString("Default").Returns("Host=localhost;Username=postgres;Password=secret;Database=testdb");

        var factory = new NpgConnectionFactory(config);

        // Act
        var connection = factory.GetNewConnection();

        // Assert
        connection.ShouldNotBeNull();
        connection.ShouldBeOfType<NpgsqlConnection>();
        connection.ConnectionString.ShouldContain("Host=localhost");
    }

    [Fact]
    public void Should_ThrowArgumentNullException_When_ConnectionStringIsMissing()
    {
        // Arrange
        var config = Substitute.For<IConfiguration>();
        config.GetConnectionString("Default").Returns((string?)null);

        var factory = new NpgConnectionFactory(config);

        // Act & Assert
        var ex = Should.Throw<ArgumentNullException>(() => factory.GetNewConnection());
        ex.ParamName.ShouldBe("ConnectionStrings:Default");
    }
}
