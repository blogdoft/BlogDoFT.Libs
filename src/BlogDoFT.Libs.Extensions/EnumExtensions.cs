namespace BlogDoFT.Libs.Extensions;

/// <summary>
/// Provides extension methods for converting between enum values, integers, and strings.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Converts an enum value to its underlying integer representation.
    /// </summary>
    /// <param name="value">The enum value to convert.</param>
    /// <returns>The integer value backing <paramref name="value"/>.</returns>
    public static int AsInteger(this Enum value) =>
        Convert.ToInt32(value);

    /// <summary>
    /// Parses a string into the specified enum type.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to parse into.</typeparam>
    /// <param name="enumValue">The string representation of the enum value.</param>
    /// <returns>The parsed <typeparamref name="TEnum"/> value.</returns>
    /// <exception cref="InvalidCastException">Thrown when <paramref name="enumValue"/> is not a valid value for <typeparamref name="TEnum"/>.</exception>
    public static TEnum ToEnum<TEnum>(this string enumValue)
        where TEnum : struct
    {
        var isValid = Enum.TryParse<TEnum>(enumValue, out var parsed);

        if (!isValid)
        {
            throw new InvalidCastException($"{enumValue} is not valid for {typeof(TEnum).FullName}");
        }

        return parsed;
    }
}
