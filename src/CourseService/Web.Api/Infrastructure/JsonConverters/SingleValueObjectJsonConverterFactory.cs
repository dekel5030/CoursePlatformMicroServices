using System.Text.Json;
using System.Text.Json.Serialization;
using Kernel;

namespace Courses.Api.Infrastructure.JsonConverters;

public class SingleValueObjectJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISingleValueObject<>));
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var iface = typeToConvert.GetInterfaces()
            .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISingleValueObject<>));

        var valueType = iface.GetGenericArguments()[0];
        var converterType = typeof(SingleValueObjectJsonConverter<,>).MakeGenericType(typeToConvert, valueType);

        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}
