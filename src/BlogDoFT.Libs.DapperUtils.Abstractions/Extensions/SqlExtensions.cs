using System.Globalization;
using System.Text;

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

    public static string ToSearchable(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        // Substituir rapidamente os cedilhas antes da normalização
        value = value.Replace('ç', 'c').Replace('Ç', 'C');

        // Normalizar para FormD (decomposição de acentos)
        var normalized = value.Normalize(NormalizationForm.FormD).AsSpan();

        // Alocar buffer no heap apenas se necessário
        var sb = new StringBuilder(normalized.Length);
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        // Retornar tudo em maiúsculas (sem necessidade de nova normalização na maioria dos casos)
        return sb.ToString().ToUpperInvariant();
    }
}
