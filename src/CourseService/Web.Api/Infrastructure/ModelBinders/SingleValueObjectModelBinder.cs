using Kernel;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;

namespace Courses.Api.Infrastructure.ModelBinders;

/// <summary>
/// Model binder that automatically converts route and query parameters to ISingleValueObject types.
/// </summary>
public class SingleValueObjectModelBinder : IModelBinder
{
    private readonly Type _valueType;
    private readonly ConstructorInfo _constructor;

    public SingleValueObjectModelBinder(Type modelType, Type valueType)
    {
        _valueType = valueType;
        _constructor = modelType.GetConstructor(new[] { valueType })
            ?? throw new InvalidOperationException(
                $"Type {modelType.Name} must have a constructor that accepts {valueType.Name}");
    }

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var rawValue = valueProviderResult.FirstValue;
        if (string.IsNullOrEmpty(rawValue))
        {
            return Task.CompletedTask;
        }

        try
        {
            // Convert the string value to the inner value type
            object? innerValue = Convert.ChangeType(rawValue, _valueType);

            if (innerValue is null)
            {
                bindingContext.ModelState.TryAddModelError(
                    modelName,
                    $"Cannot convert '{rawValue}' to {_valueType.Name}.");
                return Task.CompletedTask;
            }

            // Create an instance of the value object using the constructor
            var valueObject = _constructor.Invoke(new[] { innerValue });

            bindingContext.Result = ModelBindingResult.Success(valueObject);
        }
        catch (Exception ex)
        {
            bindingContext.ModelState.TryAddModelError(
                modelName,
                $"Cannot convert '{rawValue}' to {bindingContext.ModelType.Name}. {ex.Message}");
        }

        return Task.CompletedTask;
    }
}
