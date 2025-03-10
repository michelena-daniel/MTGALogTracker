using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LogWorker.Helpers
{
    public class CustomDateConverter : JsonConverter<DateTime>
    {
        private const string DateFormat = "dd/MM/yyyy HH:mm:ss";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (DateTime.TryParseExact(reader.GetString(), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date;
            }

            throw new JsonException($"Invalid date format. Expected format: {DateFormat}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
        }
    }
}
