using BlogDoFT.Libs.DapperUtils.Abstractions;
using BlogDoFT.Libs.DapperUtils.Abstractions.TypeHandlers;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace BlogDoFT.Libs.DapperUtils.Postgres;

internal class NpgConnectionFactory(IConfiguration configuration)
    : IConnectionFactory
{
    private readonly IConfiguration _configuration = configuration;

#pragma warning disable S3928 // The parameter %s is not declared in the argument list.
    public IDbConnection GetNewConnection()
    {
        var builder = new NpgsqlConnectionStringBuilder(_configuration.GetConnectionString("Default")
            ?? throw new ArgumentNullException("ConnectionStrings:Default"))
        {
            SearchPath = "public",
        };

        var connection = new NpgsqlConnection(builder.ConnectionString);
        SqlMapper.AddTypeHandler(new SqlDateOnlyTypeHandler());
        SqlMapper.AddTypeHandler(new SqlTimeOnlyTypeHandler());

        return connection;
    }
#pragma warning restore S3928
}
