using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kernel;

namespace Courses.Api.Infrastructure.JsonConverters;

public class SingleValueObjectJsonConverterFactory : JsonConverterFactory
{
    private static readonly ConcurrentDictionary<Type, Type?> _valueTypeCache = new();

    public override bool CanConvert(Type typeToConvert)
    {
        return GetInnerValueType(typeToConvert) != null;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var valueType = GetInnerValueType(typeToConvert)!;

        var ctor = typeToConvert.GetConstructor(new[] { valueType });
        if (ctor == null)
        {
            throw new InvalidOperationException(
                $"Type {typeToConvert.Name} must have a constructor that accepts {valueType.Name}");
        }

        var converterType = typeof(SingleValueObjectJsonConverter<,>).MakeGenericType(typeToConvert, valueType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private static Type? GetInnerValueType(Type type)
    {
        return _valueTypeCache.GetOrAdd(type, t =>
        {
            var svoInterface = t.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISingleValueObject<>));

            return svoInterface?.GetGenericArguments()[0];
        });
    }
}