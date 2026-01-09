using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kernel;

namespace Courses.Api.Infrastructure.JsonConverters;

public sealed class SingleValueObjectJsonConverter<TObject, TValue> : JsonConverter<TObject>
    where TObject : ISingleValueObject<TValue>
{
    public override TObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = JsonSerializer.Deserialize<TValue>(ref reader, options);
        if (value is null) return default;

        return (TObject)Activator.CreateInstance(typeof(TObject), value)!;
    }

    public override void Write(Utf8JsonWriter writer, TObject value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }
}