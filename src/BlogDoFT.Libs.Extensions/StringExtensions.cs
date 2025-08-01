namespace BlogDoFT.Libs.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Replaces all occurrences of `oldString` by `newString` at value.
    /// </summary>
    /// <param name="value">The string to be analyzed.</param>
    /// <param name="oldString">Token to be replaced.</param>
    /// <param name="newString">Token to be introduced.</param>
    /// <returns>The same string with tokens replaced.</returns>
    public static string ReplaceAll(this string value, string oldString, string newString)
    {
        if (oldString.Equals(newString))
        {
            return value;
        }

        if (!value.Contains(oldString))
        {
            return value;
        }

        return value
            .Replace(oldString, newString)
            .ReplaceAll(oldString, newString);
    }
}
