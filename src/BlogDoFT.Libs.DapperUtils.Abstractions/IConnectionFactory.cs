using System.Data;

namespace BlogDoFT.Libs.DapperUtils.Abstractions;

public interface IConnectionFactory
{
    IDbConnection GetNewConnection();
}
