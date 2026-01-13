using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kernel;

namespace Courses.Api.Infrastructure.JsonConverters;

internal sealed class SingleValueObjectJsonConverterFactory : JsonConverterFactory
{
    private static readonly ConcurrentDictionary<Type, Type?> _valueTypeCache = new();

    public override bool CanConvert(Type typeToConvert)
    {
        return GetInnerValueType(typeToConvert) != null;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type valueType = GetInnerValueType(typeToConvert)!;

        _ = typeToConvert.GetConstructor(new[] { valueType }) ?? throw new InvalidOperationException(
                $"Type {typeToConvert.Name} must have a constructor that accepts {valueType.Name}");

        Type converterType = typeof(SingleValueObjectJsonConverter<,>).MakeGenericType(typeToConvert, valueType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private static Type? GetInnerValueType(Type type)
    {
        return _valueTypeCache.GetOrAdd(type, t =>
        {
            Type? svoInterface = t.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISingleValueObject<>));

            return svoInterface?.GetGenericArguments()[0];
        });
    }
}
