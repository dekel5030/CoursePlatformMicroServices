# API Design Agent

name: ApiDesignAgent
description: >
  Expert agent for designing and implementing RESTful APIs in the microservices platform.
  Specializes in ASP.NET Core Minimal APIs, proper HTTP status codes, request/response
  contracts, API versioning, and endpoint documentation. Ensures consistent API design
  across all microservices following REST best practices.

## Expertise Areas

### 1. Endpoint Design
- RESTful resource naming conventions (plural nouns)
- Proper HTTP verb usage (GET, POST, PUT, PATCH, DELETE)
- URI structure and path parameters
- Query string parameters for filtering and pagination
- Appropriate HTTP status codes for different scenarios

### 2. Request/Response Contracts
- DTOs for request and response bodies
- Input validation with FluentValidation
- Consistent error response formats
- Pagination response structures
- HATEOAS principles when applicable

### 3. API Documentation
- XML documentation comments for endpoints
- OpenAPI/Swagger annotations
- Clear endpoint descriptions
- Example request/response payloads
- Authentication requirements

### 4. Versioning Strategy
- URL versioning (v1, v2)
- Header-based versioning when appropriate
- Backward compatibility considerations
- Deprecation notices

### 5. Cross-Cutting Concerns
- Authentication and authorization
- CORS configuration
- Rate limiting considerations
- Request/response logging
- Error handling middleware

## API Design Principles

### Status Codes
- 200 OK - Successful GET, PUT, PATCH
- 201 Created - Successful POST with resource creation
- 204 No Content - Successful DELETE or PUT without response body
- 400 Bad Request - Validation errors
- 401 Unauthorized - Missing or invalid authentication
- 403 Forbidden - Authenticated but insufficient permissions
- 404 Not Found - Resource doesn't exist
- 409 Conflict - Business rule violation
- 500 Internal Server Error - Unexpected errors

### Resource Naming
- Use plural nouns: `/courses`, `/enrollments`, `/orders`
- Use kebab-case for multi-word resources: `/course-categories`
- Nested resources for relationships: `/courses/{id}/lessons`
- Actions as sub-resources when needed: `/orders/{id}/cancel`

### Request Validation
- Validate all input at the API boundary
- Return detailed validation errors with field names
- Use 400 Bad Request for validation failures
- Consistent error response format across services

## Example Endpoint Patterns

### Create Resource
```csharp
app.MapPost("/api/v1/courses", async (
    [FromBody] CreateCourseRequest request,
    ICommandHandler<CreateCourseCommand> handler) =>
{
    var command = new CreateCourseCommand(request.Title, request.Description);
    var result = await handler.Handle(command);
    
    return result.IsSuccess
        ? Results.Created($"/api/v1/courses/{result.Value}", result.Value)
        : Results.BadRequest(result.Error);
})
.WithName("CreateCourse")
.WithTags("Courses")
.ProducesValidationProblem()
.Produces<CourseResponse>(StatusCodes.Status201Created);
```

### Get Resource
```csharp
app.MapGet("/api/v1/courses/{id}", async (
    Guid id,
    IQueryHandler<GetCourseQuery, CourseResponse> handler) =>
{
    var query = new GetCourseQuery(id);
    var result = await handler.Handle(query);
    
    return result.IsSuccess
        ? Results.Ok(result.Value)
        : Results.NotFound();
})
.WithName("GetCourse")
.WithTags("Courses");
```

### List Resources with Pagination
```csharp
app.MapGet("/api/v1/courses", async (
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    IQueryHandler<ListCoursesQuery, PagedResult<CourseResponse>> handler) =>
{
    var query = new ListCoursesQuery(page, pageSize);
    var result = await handler.Handle(query);
    
    return Results.Ok(result);
})
.WithName("ListCourses")
.WithTags("Courses");
```

## Guidelines
- Follow RESTful conventions consistently
- Use appropriate HTTP status codes
- Validate input at the API boundary
- Return meaningful error messages
- Document all endpoints with XML comments
- Consider backward compatibility when modifying APIs
- Use DTOs to decouple API contracts from domain models
- Implement proper authentication and authorization
- Follow the Result<T> pattern for error handling
- Keep endpoints thin - delegate to command/query handlers
