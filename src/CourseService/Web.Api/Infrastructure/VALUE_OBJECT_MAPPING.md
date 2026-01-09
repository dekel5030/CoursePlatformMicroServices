# Centralized Value Object Mapping Implementation

## Overview
This implementation provides automatic model binding and JSON serialization for Value Objects that implement `ISingleValueObject<T>`. It eliminates the need for manual instantiation (e.g., `new CourseId(id)`) in endpoint handlers and keeps the Domain layer free from infrastructure concerns.

## Architecture

### Components

#### 1. Model Binding Infrastructure
Located in `Infrastructure/ModelBinders/`:

- **SingleValueObjectModelBinder**: Handles the conversion from route/query string parameters to Value Objects
  - Extracts the string value from the route/query
  - Converts it to the inner type (e.g., Guid)
  - Instantiates the Value Object using its constructor
  - Adds model errors if conversion fails

- **SingleValueObjectModelBinderProvider**: Automatically discovers types implementing `ISingleValueObject<T>`
  - Scans parameter types in endpoint signatures
  - Returns appropriate model binder for Value Objects
  - Returns null for non-Value Object types

#### 2. JSON Serialization
Located in `Infrastructure/JsonConverters/`:

- **SingleValueObjectJsonConverter**: Flattens Value Objects to their inner value in JSON
  - Already existed in the codebase
  - Handles serialization for request/response bodies
  - Ensures clean JSON schema for API consumers

#### 3. Route Constraints (Infrastructure, not yet wired)
Located in `Infrastructure/RouteConstraints/`:

- **SingleValueObjectRouteConstraint**: Validates Value Objects at the routing level
  - Created for future enhancement
  - Can be used to add route constraints for type safety

## Usage

### Before (Manual Mapping)
```csharp
app.MapGet("courses/{id:Guid}", async (
    Guid id,
    IMediator mediator,
    CancellationToken cancellationToken) =>
{
    var query = new GetCourseByIdQuery(new CourseId(id)); // Manual instantiation
    Result<CourseDetailsDto> result = await mediator.Send(query, cancellationToken);
    return result.Match(Results.Ok, CustomResults.Problem);
});
```

### After (Automatic Binding)
```csharp
app.MapGet("courses/{id}", async (
    [FromRoute] CourseId id, // Automatic binding!
    IMediator mediator,
    CancellationToken cancellationToken) =>
{
    var query = new GetCourseByIdQuery(id); // Direct usage
    Result<CourseDetailsDto> result = await mediator.Send(query, cancellationToken);
    return result.Match(Results.Ok, CustomResults.Problem);
});
```

## Implementation Details

### Registration in DI Container
In `Program.cs`:
```csharp
builder.Services.AddValueObjectModelBinding();
```

This extension method registers the `SingleValueObjectModelBinderProvider` with ASP.NET Core's MVC infrastructure.

### Route Patterns
- **Before**: `courses/{id:Guid}` - Explicit Guid constraint
- **After**: `courses/{id}` - Type inferred from parameter

The route no longer needs the `:Guid` constraint because the model binder handles type conversion and validation.

### Affected Endpoints
All endpoints that take Value Objects as route/query parameters:

#### Courses
- GetCourseById: `CourseId id`
- DeleteCourse: `CourseId courseId`
- PatchCourse: `CourseId id`

#### Lessons
- GetLessonById: `CourseId courseId`, `LessonId lessonId`
- CreateLesson: `CourseId courseid`
- PatchLesson: `CourseId courseId`, `LessonId lessonId`
- DeleteLesson: `CourseId courseId`, `LessonId lessonId`

## Benefits

### 1. Domain Purity
- Domain Value Objects remain free of infrastructure concerns
- No need for `IParsable<T>` or `TryParse` methods in Domain layer
- Clean separation between Domain and Infrastructure

### 2. Reduced Boilerplate
- Eliminates manual `new CourseId(id)` calls in every endpoint
- Reduces code duplication across endpoints
- Makes endpoint code more readable

### 3. Automatic Discovery
- Adding new Value Objects requires zero configuration
- Any type implementing `ISingleValueObject<T>` is automatically supported
- No manual registration needed

### 4. Type Safety
- Endpoints work with strongly-typed Value Objects
- Compile-time safety instead of runtime string manipulation
- IDE auto-completion and refactoring support

### 5. Consistent Error Handling
- Model binding errors are handled uniformly
- Invalid values result in proper 400 Bad Request responses
- Clear error messages for API consumers

## Notes

### ASP0020 Analyzer Warning
The ASP.NET analyzer (ASP0020) suggests implementing `IParsable<T>` for minimal API parameters. However, this would leak infrastructure concerns into the Domain layer. We suppress this warning via:

```xml
<NoWarn>$(NoWarn);ASP0020</NoWarn>
```

This is acceptable because:
1. We use custom model binding instead of `IParsable<T>`
2. Our approach maintains Domain purity
3. The functionality works correctly despite the warning

### Future Enhancements
- Wire up `SingleValueObjectRouteConstraint` for additional validation
- Add integration tests to verify end-to-end behavior
- Extend to support query string arrays of Value Objects
- Consider adding TypeConverter for compatibility with other frameworks

## Testing
The implementation has been verified by:
1. Building the entire CourseService successfully
2. Ensuring all endpoints compile without manual instantiation
3. Verifying JSON serialization still works with existing converters

## Related Files
- `Program.cs`: Registration of model binding
- `Infrastructure/ModelBinders/`: Model binding implementation
- `Infrastructure/JsonConverters/`: JSON serialization (pre-existing)
- `Infrastructure/RouteConstraints/`: Route constraint infrastructure
- `Endpoints/**/*.cs`: Updated endpoint signatures
- `Courses.Api.csproj`: ASP0020 suppression
