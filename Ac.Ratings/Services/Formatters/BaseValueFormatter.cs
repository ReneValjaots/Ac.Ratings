using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ac.Ratings.Services.Formatters {
    public abstract class BaseValueFormatter : JsonConverter<string?> {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            var value = reader.GetString();
            return TransformValue(value);
        }

        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options) {
            writer.WriteStringValue(value);
        }

        public abstract string? TransformValue(string? value);
    }
}
