using System.Text.Json;
using System.Text.Json.Serialization;

namespace VehicleRentalSystem.JsonConverters
{
    public static class UtcHelper
    {
        public static DateTime ToUtc(DateTime value) => value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)   // unspecified (e.g. from SQL Server) = UTC
        };

        public static DateTime? ToUtc(DateTime? value) => value.HasValue ? ToUtc(value.Value) : null;
    }

    public class UtcDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            UtcHelper.ToUtc(reader.GetDateTime());

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
            writer.WriteStringValue(UtcHelper.ToUtc(value));
    }

    public class UtcNullableDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            reader.TokenType == JsonTokenType.Null ? null : UtcHelper.ToUtc(reader.GetDateTime());

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue) writer.WriteStringValue(UtcHelper.ToUtc(value.Value));
            else writer.WriteNullValue();
        }
    }
}

 