using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace RepositoryUOW.Data.Converter;

public sealed class StringToUpperConverter : ValueConverter<string, string>
{
    public static readonly StringToUpperConverter Instance = new();

    private StringToUpperConverter() : base(s => s, 
#pragma warning disable CS8603
        s => s == null ? null : s.ToUpperInvariant())
#pragma warning restore CS8603
    {
    }
}