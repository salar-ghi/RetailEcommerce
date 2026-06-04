using System.Text.Json.Serialization;

namespace Application.Common.Json;

/// <summary>
/// Serializes enum values as their numeric values for API clients that store enum choices as ids.
/// String enum values are still accepted on input so existing clients remain compatible.
/// </summary>
/// <typeparam name="TEnum">The enum type handled by this converter.</typeparam>
public sealed class NumericEnumJsonConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number when reader.TryGetInt32(out var numericValue) =>
                (TEnum)Enum.ToObject(typeof(TEnum), numericValue),
            JsonTokenType.String when Enum.TryParse<TEnum>(reader.GetString(), ignoreCase: true, out var enumValue) =>
                enumValue,
            _ => throw new JsonException($"Unable to convert JSON value to {typeof(TEnum).Name}.")
        };
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(Convert.ToInt32(value));
    }
}
