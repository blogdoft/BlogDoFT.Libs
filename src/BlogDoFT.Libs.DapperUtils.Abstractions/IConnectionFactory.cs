using System.Data;

namespace BlogDoFT.Libs.DapperUtils.Abstractions;

/// <summary>
/// Creates database connections.
/// </summary>
public interface IConnectionFactory
{
    /// <summary>
    /// Creates a new database connection.
    /// </summary>
    /// <returns>A new <see cref="IDbConnection"/> instance.</returns>
    IDbConnection GetNewConnection();
}
