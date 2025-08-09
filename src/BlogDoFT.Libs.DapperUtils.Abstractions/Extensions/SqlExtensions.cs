namespace BlogDoFT.Libs.DapperUtils.Abstractions.Extensions;

public static class SqlExtensions
{
    public static string AsSqlWildCard(this string value, bool toUpperCase = true)
    {
        var sqlField = value.Replace('*', '%');
        if (toUpperCase)
        {
            sqlField = sqlField.ToUpperInvariant();
        }

        return sqlField;
    }
}
