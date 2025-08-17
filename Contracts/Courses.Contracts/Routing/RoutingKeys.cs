namespace Courses.Contracts.Routing;

public static class RoutingKeys
{
    public static string Created(int version)   => $"courses.created.v{version}";
    public static string Removed(int version)    => $"courses.removed.v{version}";
    public static string Upserted(int version)   => $"courses.upserted.v{version}";
}