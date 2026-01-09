using Kernel;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Courses.Api.Infrastructure.ModelBinders;

/// <summary>
/// Provider that automatically discovers and creates model binders for ISingleValueObject types.
/// </summary>
public class SingleValueObjectModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var modelType = context.Metadata.ModelType;

        // Check if the type implements ISingleValueObject<T>
        var svoInterface = modelType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISingleValueObject<>));

        if (svoInterface is null)
        {
            return null;
        }

        // Get the inner value type
        var valueType = svoInterface.GetGenericArguments()[0];

        // Create and return the model binder
        return new SingleValueObjectModelBinder(modelType, valueType);
    }
}
