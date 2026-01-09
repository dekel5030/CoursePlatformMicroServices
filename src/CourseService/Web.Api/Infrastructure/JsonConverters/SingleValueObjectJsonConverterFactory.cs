using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kernel;

namespace Courses.Api.Infrastructure.JsonConverters;

public sealed class SingleValueObjectJsonConverterFactory : JsonConverterFactory
{
    private static readonly ConcurrentDictionary<Type, Type?> _typeCache = new();

    public override bool CanConvert(Type typeToConvert)
    {
        return GetValueType(typeToConvert) != null;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var valueType = GetValueType(typeToConvert)
            ?? throw new InvalidOperationException($"Type {typeToConvert.Name} is not a valid ISingleValueObject.");

        var converterType = typeof(SingleValueObjectJsonConverter<,>)
            .MakeGenericType(typeToConvert, valueType);

        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private static Type? GetValueType(Type type)
    {
        return _typeCache.GetOrAdd(type, t =>
        {
            var interfaces = t.GetInterfaces();

            var svoInterface = interfaces.FirstOrDefault(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISingleValueObject<,>));

            if (svoInterface is null) return null;

            var genericArgs = svoInterface.GetGenericArguments();
            return genericArgs[0] == t ? genericArgs[1] : null;
        });
    }
}
