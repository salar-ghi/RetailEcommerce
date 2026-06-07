using System.Globalization;
using System.Text.Json.Serialization;

namespace Application.Common.Json;

/// <summary>
/// Allows optional DateTime fields to be omitted, null, or posted as an empty string.
/// This keeps browser forms from failing model binding when an optional date input is blank.
/// </summary>
public sealed class NullableDateTimeJsonConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected a JSON string for an optional DateTime value.");

        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (DateTime.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind,
                out var dateTime))
        {
            return dateTime;
        }

        throw new JsonException($"Unable to convert '{value}' to an optional DateTime value.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value);
        else
            writer.WriteNullValue();
    }
}
