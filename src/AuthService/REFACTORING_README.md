# AuthService - Clean Architecture Refactoring

This document describes the refactored AuthService following Clean Architecture, DDD, and Event-Driven Architecture principles.

## Architecture Overview

The AuthService has been restructured into four main layers:

### 1. Domain Layer (`Domain/`)
Contains the core business logic and domain model.

**Entities:**
- `AuthUser` - User authentication entity with roles and permissions
- `Role` - Role entity with permissions
- `Permission` - Permission entity
- Junction entities: `UserRole`, `UserPermission`, `RolePermission`

**Value Objects (Primitives):**
- `AuthUserId` - Strongly-typed ID for AuthUser
- `RoleId` - Strongly-typed ID for Role
- `PermissionId` - Strongly-typed ID for Permission

**Domain Events:**
- `UserRegisteredDomainEvent` - Raised when a new user registers
- `UserLoggedInDomainEvent` - Raised when a user logs in
- `RoleAssignedDomainEvent` - Raised when a role is assigned to a user

**Errors:**
- `AuthUserErrors` - Domain-specific errors for authentication
- `RoleErrors` - Domain-specific errors for roles

### 2. Application Layer (`Application/`)
Contains application logic, use cases, and abstractions.

**Commands & Handlers:**
- `RegisterUserCommand` / `RegisterUserCommandHandler` - User registration
- `LoginUserCommand` / `LoginUserCommandHandler` - User authentication

**Abstractions:**
- `ICommand` / `ICommandHandler` - Command pattern interfaces
- `IQuery` / `IQueryHandler` - Query pattern interfaces
- `IPasswordHasher` - Password hashing abstraction
- `ITokenService` - JWT token generation abstraction
- `IWriteDbContext` / `IReadDbContext` - Database context abstractions
- `IEventPublisher` - Event publishing abstraction

**DTOs:**
- `RegisterRequestDto` - Registration request
- `LoginRequestDto` - Login request
- `AuthResponseDto` - Authentication response with token

### 3. Infrastructure Layer (`Infrastructure/`)
Contains implementations of infrastructure concerns.

**Database:**
- `AuthDbContext` - EF Core DbContext with proper entity configurations
- Strongly-typed ID conversions
- Auto-included navigations for performance

**Security:**
- `BcryptPasswordHasher` - BCrypt password hashing implementation
- `TokenService` - JWT token generation implementation

**Domain Events:**
- `DomainEventsDispatcher` - Dispatches domain events to handlers

**Messaging:**
- `MassTransitEventPublisher` - Publishes integration events via MassTransit/RabbitMQ

### 4. Web.Api Layer (`Web.Api/`)
Contains HTTP endpoints and presentation logic.

**Endpoints:**
- `POST /auth/register` - User registration endpoint
- `POST /auth/login` - User login endpoint

**Infrastructure:**
- `CustomResults` - Consistent error response formatting
- `ResultExtensions` - Result pattern helpers
- Minimal API endpoint pattern with `IEndpoint` interface

## Configuration

Update `appsettings.json` with the following:

```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Database=authdb;Username=postgres;Password=postgres",
    "RabbitMq": "amqp://guest:guest@localhost:5672"
  },
  "Jwt": {
    "Key": "your-secret-key-min-32-characters-long",
    "Issuer": "AuthService",
    "Audience": "CoursePlatform",
    "ExpirationMinutes": "60"
  }
}
```

## Key Patterns Used

1. **Clean Architecture** - Dependency inversion, domain at the center
2. **Domain-Driven Design** - Rich domain model, value objects, domain events
3. **CQRS** - Separation of commands and queries
4. **Result Pattern** - Functional error handling with `Result<T>`
5. **Repository Pattern** - DbContext abstractions for data access
6. **Event-Driven** - Domain events for cross-aggregate communication
7. **Minimal APIs** - Modern ASP.NET Core endpoint pattern

## Migration Notes

### What's New
- Clean separation of concerns across layers
- Strongly-typed IDs for entities
- Domain events for business logic triggers
- Result pattern for error handling
- MassTransit integration for event publishing
- Minimal API endpoints

### Pending Work
1. **Event-Driven User Creation** - Currently registration creates a placeholder userId (0). This needs to be refactored to:
   - Publish UserCreationRequested event
   - Handle UserCreated integration event from UserService
   - Complete AuthUser creation asynchronously

2. **Domain Event Publishing** - Domain events are raised but need to be dispatched:
   - Call DomainEventsDispatcher after SaveChanges
   - Create domain event handlers
   - Map domain events to integration events

3. **Testing** - Add unit tests for:
   - CommandHandlers
   - QueryHandlers
   - Domain entities
   - Domain event handlers

4. **Cleanup** - Remove old code:
   - Controllers folder
   - Services folder
   - Data folder
   - Models folder
   - Old Dtos folder

## Running the Service

1. **Database Setup:**
   ```bash
   cd Infrastructure
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

2. **Run the Service:**
   ```bash
   cd Web.Api
   dotnet run
   ```

3. **Test Endpoints:**
   ```bash
   # Register
   curl -X POST http://localhost:5000/auth/register \
     -H "Content-Type: application/json" \
     -d '{"email":"user@example.com","password":"Password123","fullname":"John Doe"}'
   
   # Login
   curl -X POST http://localhost:5000/auth/login \
     -H "Content-Type: application/json" \
     -d '{"email":"user@example.com","password":"Password123"}'
   ```

## Future Enhancements

1. Implement OAuth2/OpenID Connect
2. Add refresh token support
3. Implement 2FA
4. Add password reset functionality
5. Implement account confirmation via email
6. Add rate limiting
7. Implement CORS policies
8. Add comprehensive logging with Serilog
9. Implement health checks
10. Add API versioning
