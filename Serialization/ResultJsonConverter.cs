using System.Text.Json;
using System.Text.Json.Serialization;
using Common;
using Common.Errors;

namespace Common.Serialization;
public class ResultJsonConverter<T> : JsonConverter<Result<T>>
{
    public override Result<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var jsonObject = jsonDoc.RootElement;

        if (!jsonObject.TryGetProperty("isSuccess", out var isSuccessProp))
            throw new JsonException("Missing 'isSuccess' in Result<T>");

        var isSuccess = isSuccessProp.GetBoolean();

        if (isSuccess)
        {
            if (!jsonObject.TryGetProperty("value", out var valueProp))
                throw new JsonException("Missing 'value' in successful Result<T>");

            var value = valueProp.Deserialize<T>(options);
            return Result<T>.Success(value);
        }
        else
        {
            if (!jsonObject.TryGetProperty("error", out var errorProp))
                throw new JsonException("Missing 'error' in failed Result<T>");

            var error = errorProp.Deserialize<Error>(options);
            return Result<T>.Failure(error!);
        }
    }


    public override void Write(Utf8JsonWriter writer, Result<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteBoolean("isSuccess", value.IsSuccess);

        if (value.IsSuccess)
        {
            writer.WritePropertyName("value");
            JsonSerializer.Serialize(writer, value.Value, options);
        }
        else
        {
            writer.WritePropertyName("error");
            JsonSerializer.Serialize(writer, value.Error, options);
        }

        writer.WriteEndObject();
    }
}
