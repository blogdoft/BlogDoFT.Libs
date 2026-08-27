using System.Text;

namespace BlogDoFT.Libs.DapperUtils.Postgres;

/// <summary>
/// Builds a SQL statement that counts the total rows produced by another query.
/// </summary>
public static class PageCountStmt
{
    private const string SqlCount = "select count(1) from ({0}) as counter_result";

    /// <summary>
    /// Wraps the given SQL query in a <c>SELECT COUNT(1)</c> statement, to obtain the total number of rows it produces.
    /// </summary>
    /// <param name="sql">The SQL query to be counted.</param>
    /// <returns>A new <see cref="StringBuilder"/> containing the count statement.</returns>
    public static StringBuilder BuildCountSql(StringBuilder sql) =>
        new StringBuilder()
            .AppendFormat(SqlCount, sql);
}
