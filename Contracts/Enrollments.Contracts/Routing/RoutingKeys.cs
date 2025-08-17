namespace Enrollments.Contracts.Routing;

public static class RoutingKeys
{
    public static string Created(int version)   => $"enrollments.created.v{version}";
    public static string Cancelled(int version) => $"enrollments.cancelled.v{version}";
}