using Yarp.ReverseProxy.Configuration;

namespace Gateway.Api;

internal static class GatewayConfiguration
{
    public static IReadOnlyList<RouteConfig> GetRoutes()
    {
        return new[]
        {
            // Courses Route
            CreateRoute("courses-route", "courseservice", "/api/courses/{**catch-all}", "/api"),
            
            // Lessons Route
            CreateRoute("lessons-route", "courseservice", "/api/lessons/{**catch-all}", "/api"),
            CreateRoute("modules-route", "courseservice", "/api/modules/{**catch-all}", "/api"),
            
            // Auth Routes (דוגמה לשימוש חוזר)
            CreateRoute("auth-route", "authservice", "/api/auth/{**catch-all}", "/api"),
            CreateRoute("users-route", "userservice", "/api/users/{**catch-all}", "/api"),
            CreateRoute("auth-management", "authservice", "/api/admin/{**catch-all}", "/api/admin"),
            
            
            // Storage Route (שונה מהאחרים, אז נגדיר ידנית)
            new RouteConfig
            {
                RouteId = "storage-public-route",
                ClusterId = "garage-website-cluster",
                Match = new RouteMatch { Path = "/public/{bucket}/{**remainder}" },
                Transforms = new[]
                {
                    new Dictionary<string, string> { { "RequestHeader", "Host" }, { "Set", "{bucket}" } },
                    new Dictionary<string, string> { { "PathSet", "/{remainder}" } }
                }
            }
        };
    }

    public static IReadOnlyList<ClusterConfig> GetClusters()
    {
        return new[]
        {
            CreateCluster("courseservice", "https://courseservice"),
            CreateCluster("authservice", "https://authservice"),
            CreateCluster("userservice", "https://userservice"),
            CreateCluster("garage-website-cluster", "http://placeholder") // שים לב לשינוי כאן אם צריך
        };
    }

    // --- Helper Methods (כדי לא לשכפל קוד) ---

    //private static RouteConfig CreateRoute(string routeId, string clusterId, string path, string prefixToRemove)
    //{
    //    return new RouteConfig
    //    {
    //        RouteId = routeId,
    //        ClusterId = clusterId,
    //        Match = new RouteMatch { Path = path },
    //        Transforms = new[]
    //        {
    //            // 1. מוריד את ה-Prefix מה-Path
    //            new Dictionary<string, string>
    //            {
    //                { "PathRemovePrefix", prefixToRemove }
    //            },
    //            // 2. מוסיף את ה-Header הקריטי שחסר לך
    //            new Dictionary<string, string>
    //            {
    //                { "RequestHeader", "X-Forwarded-Prefix" },
    //                { "Set", prefixToRemove }
    //            }
    //        }
    //    };
    //}

    private static RouteConfig CreateRoute(string routeId, string clusterId, string path, string prefixToRemove)
    {
        return new RouteConfig
        {
            RouteId = routeId,
            ClusterId = clusterId,
            Match = new RouteMatch { Path = path },
            Transforms = new[]
            {
            // 1. חובה! מחזיר את ה-X-Forwarded-* הרגילים ש-YARP מבטל כשמוסיפים טרנספורמים ידניים
            new Dictionary<string, string>
            {
                { "X-Forwarded", "Set" },
                { "HeaderPrefix", "X-Forwarded-" }
            },

            // 2. מסיר את /api מהנתיב הפיזי
            new Dictionary<string, string>
            {
                { "PathRemovePrefix", prefixToRemove }
            },

            // 3. מוסיף את ה-Prefix כ-Header ידני (כדי ש-CourseService ידע)
            new Dictionary<string, string>
            {
                { "RequestHeader", "X-Forwarded-Prefix" },
                { "Set", prefixToRemove }
            }
        }
        };
    }

    private static ClusterConfig CreateCluster(string clusterId, string address)
    {
        return new ClusterConfig
        {
            ClusterId = clusterId,
            Destinations = new Dictionary<string, DestinationConfig>
            {
                { "destination1", new DestinationConfig { Address = address } }
            }
        };
    }
}
