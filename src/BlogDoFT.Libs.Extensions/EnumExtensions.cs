namespace BlogDoFT.Libs.Extensions;

public static class EnumExtensions
{
    public static int AsInteger(this Enum value) =>
        Convert.ToInt32(value);

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
