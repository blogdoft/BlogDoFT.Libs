using BlogDoFT.Libs.DapperUtils.Abstractions;
using System.Data;

namespace BlogDoFT.Libs.DapperUtils.Postgres.Tests;

public class PostgresDatabaseFacadeTests
{
    [Fact]
    public void Should_NotOpenConnection_When_Constructed()
    {
        // Given
        var connection = Substitute.For<IDbConnection>();
        var connectionFactory = Substitute.For<IConnectionFactory>();
        connectionFactory.GetNewConnection().Returns(connection);

        // When
        using var facade = new PostgresDatabaseFacade(connectionFactory);

        // Then
        connection.DidNotReceive().Open();
    }

    [Fact]
    public void Should_OpenConnection_When_GetDbConnectionIsCalledAndConnectionIsClosed()
    {
        // Given
        var connection = Substitute.For<IDbConnection>();
        connection.State.Returns(ConnectionState.Closed);
        var connectionFactory = Substitute.For<IConnectionFactory>();
        connectionFactory.GetNewConnection().Returns(connection);
        using var facade = new PostgresDatabaseFacade(connectionFactory);

        // When
        var result = facade.GetDbConnection();

        // Then
        result.ShouldBeSameAs(connection);
        connection.Received(1).Open();
    }

    [Fact]
    public void Should_NotOpenConnection_When_GetDbConnectionIsCalledAndConnectionIsAlreadyOpen()
    {
        // Given
        var connection = Substitute.For<IDbConnection>();
        connection.State.Returns(ConnectionState.Open);
        var connectionFactory = Substitute.For<IConnectionFactory>();
        connectionFactory.GetNewConnection().Returns(connection);
        using var facade = new PostgresDatabaseFacade(connectionFactory);

        // When
        facade.GetDbConnection();

        // Then
        connection.DidNotReceive().Open();
    }

    [Fact]
    public void Should_DisposeConnection_When_FacadeIsDisposed()
    {
        // Given
        var connection = Substitute.For<IDbConnection>();
        var connectionFactory = Substitute.For<IConnectionFactory>();
        connectionFactory.GetNewConnection().Returns(connection);
        var facade = new PostgresDatabaseFacade(connectionFactory);

        // When
        facade.Dispose();

        // Then
        connection.Received(1).Dispose();
    }
}
