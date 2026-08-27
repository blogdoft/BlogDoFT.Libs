using System.Globalization;
using System.Text;

namespace BlogDoFT.Libs.DapperUtils.Abstractions.Extensions;

/// <summary>
/// Provides extension methods to help build SQL-friendly string values.
/// </summary>
public static class SqlExtensions
{
    /// <summary>
    /// Converts the wildcard character '*' into the SQL wildcard '%'.
    /// </summary>
    /// <param name="value">The string to be converted.</param>
    /// <param name="toUpperCase">When <see langword="true"/>, the returned value is converted to upper case. Defaults to <see langword="true"/>.</param>
    /// <returns>The value with '*' replaced by '%', optionally upper-cased.</returns>
    public static string AsSqlWildCard(this string value, bool toUpperCase = true)
    {
        var sqlField = value.Replace('*', '%');
        if (toUpperCase)
        {
            sqlField = sqlField.ToUpperInvariant();
        }

        return sqlField;
    }

    /// <summary>
    /// Normalizes a string for case- and accent-insensitive search by removing diacritics
    /// (including cedillas) and converting the result to upper case.
    /// </summary>
    /// <param name="value">The string to be normalized. Can be <see langword="null"/>.</param>
    /// <returns>The normalized, upper-case, accent-free string, or <see cref="string.Empty"/> when <paramref name="value"/> is <see langword="null"/> or whitespace.</returns>
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
