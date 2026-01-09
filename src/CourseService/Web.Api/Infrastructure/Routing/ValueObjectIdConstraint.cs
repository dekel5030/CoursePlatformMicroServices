namespace Courses.Api.Infrastructure.Routing;

internal class ValueObjectIdConstraint<T> : IRouteConstraint
    where T : IParsable<T>
{
    public bool Match(
        HttpContext? httpContext,
        IRouter? route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        //if (values.TryGetValue(routeKey, out var value) && value is string s)
        //{
        //    return T.TryParse(s, null, out _);
        //}

        //return false;

        if (!values.TryGetValue(routeKey, out var value) || value is null)
            return false;

        // Inbound: string from URL
        if (value is string s)
            return T.TryParse(s, null, out _);

        // Outbound: Value Object
        if (value is T)
            return true;

        // Fallback: try ToString
        return T.TryParse(value.ToString(), null, out _);
    }
}
