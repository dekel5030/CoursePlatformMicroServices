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
        if (values.TryGetValue(routeKey, out var value) && value is string s)
        {
            return T.TryParse(s, null, out _);
        }

        return false;
    }
}
