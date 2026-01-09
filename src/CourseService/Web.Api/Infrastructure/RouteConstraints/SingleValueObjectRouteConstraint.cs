using Kernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Reflection;

namespace Courses.Api.Infrastructure.RouteConstraints;

/// <summary>
/// Route constraint that validates ISingleValueObject types in route parameters.
/// </summary>
public class SingleValueObjectRouteConstraint : IRouteConstraint
{
    private readonly Type _valueType;
    private readonly ConstructorInfo _constructor;

    public SingleValueObjectRouteConstraint(Type modelType, Type valueType)
    {
        _valueType = valueType;
        _constructor = modelType.GetConstructor(new[] { valueType })
            ?? throw new InvalidOperationException(
                $"Type {modelType.Name} must have a constructor that accepts {valueType.Name}");
    }

    public bool Match(
        HttpContext? httpContext,
        IRouter? route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        if (!values.TryGetValue(routeKey, out var value) || value is null)
        {
            return false;
        }

        var valueString = value.ToString();
        if (string.IsNullOrEmpty(valueString))
        {
            return false;
        }

        try
        {
            // Try to convert the string to the inner value type
            object? innerValue = Convert.ChangeType(valueString, _valueType);

            if (innerValue is null)
            {
                return false;
            }

            // Try to create the value object - if this succeeds, the constraint is satisfied
            _ = _constructor.Invoke(new[] { innerValue });

            return true;
        }
        catch
        {
            return false;
        }
    }
}
