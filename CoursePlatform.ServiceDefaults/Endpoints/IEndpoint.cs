using Microsoft.AspNetCore.Routing;

namespace CoursePlatform.ServiceDefaults.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
