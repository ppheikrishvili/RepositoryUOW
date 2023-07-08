namespace RepositoryUOWDomain.Shared.Extensions;

public static class StringExtensions
{
    public static bool IsNumeric(this string text) => double.TryParse(text, out _);

    public static decimal ToDecimal(this string text) =>
        decimal.TryParse(text, out decimal outDecimal) ? outDecimal : 0;

    public static decimal ToInt(this string text) => int.TryParse(text, out int outInt) ? outInt : 0;

    public static string StringTrimming(this string source, int size = -1)
    {
        size = (size == -1) ? 25 : size;
        if (source.Length > size && size > 3) return $"{source[..(size - 3)]}...";
        return source;
    }
}