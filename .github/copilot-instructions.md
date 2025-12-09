# Copilot Instructions for Course Platform Microservices

This repository is a learning management system (LMS) built with a microservices architecture using .NET 9, React, RabbitMQ, and PostgreSQL. Follow these guidelines when working with this codebase.

## Architecture Overview

This project uses multiple architectural patterns across different services:

- **Clean Architecture + DDD**: UserService, AuthService, CourseService, OrderService
- **N-Layer (Traditional)**: EnrollmentService
- **Event-Driven Architecture**: All services communicate via RabbitMQ using MassTransit
- **CQRS**: Separate read/write database connections where applicable
- **Outbox Pattern**: Transactional event publishing for reliability

### Microservices Structure

Each microservice follows this layered structure (Clean Architecture):

```
ServiceName/
├── Domain/              # Entities, Value Objects, Domain Events, Aggregates
├── Application/         # Use Cases, Commands, Queries, DTOs
├── Infrastructure/      # EF Core, Messaging, External Services, Persistence
├── Web.Api/            # Controllers, Middleware, API Configuration
├── SharedKernel/       # Shared domain primitives
└── Tests/
    ├── Domain.UnitTests/
    ├── Application.UnitTests/
    ├── IntegrationTests/
    └── Architecture.Tests/
```

## Technology Stack

### Backend
- **.NET 9** with C#
- **ASP.NET Core** for Web APIs
- **Entity Framework Core** for data access
- **MassTransit** for messaging
- **PostgreSQL 16** (each service has its own database)
- **RabbitMQ 3.13** for message broker
- **.NET Aspire** for orchestration

### Frontend
- **React 19** with TypeScript
- **Vite 7** for build tooling
- **React Router 7** for routing

### Testing
- **xUnit** for unit and integration tests
- **FluentAssertions** for assertions
- **Moq** for mocking
- **Testcontainers** for integration testing with real databases
- **WebApplicationFactory** for API testing

## Code Conventions

### Naming Conventions (enforced by .editorconfig)

1. **Interfaces**: Must start with `I` (e.g., `IUserRepository`)
2. **Types**: PascalCase (classes, structs, enums, delegates)
3. **Methods, Properties, Events**: PascalCase
4. **Async Methods**: Must end with `Async` (e.g., `GetUserAsync`)
5. **Private Fields**: Underscore prefix + camelCase (e.g., `_userRepository`)
6. **Constants & Static Readonly**: PascalCase, NO underscore (e.g., `MaxRetryCount`)
7. **Parameters & Locals**: camelCase
8. **Namespaces**: File-scoped namespaces preferred

### Code Style

- Use `var` when type is apparent
- Open braces on new lines (Allman style)
- Sort using directives with `System` namespaces first
- Use file-scoped namespaces
- Prefer expression-bodied members for properties and accessors
- Use primary constructors when appropriate

## Domain-Driven Design Patterns

### Entities
- All entities inherit from `Entity` base class
- Use private setters for properties
- Factory methods for creation (e.g., `CreateUser()`)
- Methods return `Result<T>` for error handling

### Value Objects
- **Strongly-typed IDs**: Use ULID-based identifiers (e.g., `UserId`, `CourseId`)
  - Example: `Id = new UserId(Guid.CreateVersion7())`
- **Domain Primitives**: `Money`, `FullName`, `PhoneNumber`, etc.
- Immutable records preferred

### Domain Events
- Raise domain events using `entity.Raise(new DomainEvent())`
- Event names use past tense (e.g., `UserProfileCreatedDomainEvent`)
- Events are handled by domain event handlers in Infrastructure layer

### Result Pattern
- Use `Result<T>` for error handling instead of exceptions
- Return `Result.Success(value)` or `Result.Failure(error)`
- Errors are defined in dedicated `Errors` classes

## Integration Events

Services communicate asynchronously via RabbitMQ:

- **Integration Events**: Cross-service events (e.g., `UserUpserted`, `EnrollmentCreated`, `OrderCompleted`)
- **MassTransit**: Used for message bus abstraction
- **Outbox Pattern**: Ensures reliable event publishing with transactional guarantees
- Event contracts are defined in the `Contracts/` directory

## Database Conventions

- Each service has its own PostgreSQL database (database-per-service pattern)
- Use Entity Framework Core for data access
- Migrations are managed per service
- Use optimistic concurrency with version-based conflict detection

## Testing Practices

### Unit Tests
- Located in `Domain.UnitTests` and `Application.UnitTests`
- Test domain logic and use cases
- Use Moq for mocking dependencies
- Use FluentAssertions for assertions

### Integration Tests
- Located in `IntegrationTests` directories
- Use Testcontainers for ephemeral databases
- Use WebApplicationFactory for API testing
- Test end-to-end scenarios with real infrastructure

### Architecture Tests
- Verify layering and dependency rules
- Ensure Clean Architecture boundaries are maintained

## Building and Running

### Backend Services

```bash
# Run with .NET Aspire (recommended)
dotnet run --project CoursePlatform.AppHost

# Run individual service
cd src/UserService/Web.Api
dotnet run

# Run all tests
dotnet test CoursePlatform.sln

# Apply migrations (per service)
cd src/UserService
dotnet ef database update --project Infrastructure --startup-project Web.Api
```

### Frontend

```bash
cd Frontend
npm install
npm run dev      # Development server
npm run build    # Production build
npm run lint     # ESLint
```

### Infrastructure

```bash
# Start databases and RabbitMQ
cd infra
docker-compose up -d
docker-compose -f docker-compose.rabbitmq.yml up -d
```

## Service Ports (Default)

- **UserService**: 5125
- **AuthService**: 5433
- **CourseService**: 5434
- **EnrollmentService**: 5435
- **OrderService**: 5436
- **Frontend**: 5173
- **RabbitMQ Management**: 15672
- **Aspire Dashboard**: 15000

## Common Patterns to Follow

### Controllers
- Use minimal APIs or controller-based endpoints
- Return appropriate HTTP status codes
- Use DTOs for request/response models

### Dependency Injection
- Register services in Infrastructure layer
- Use interface-based abstractions
- Follow constructor injection pattern

### Error Handling
- Use Result pattern in domain and application layers
- Map domain errors to HTTP status codes in API layer
- Use middleware for global exception handling

### Authentication & Authorization
- JWT-based authentication (AuthService)
- Permission-based authorization using custom claims
- Use `[Authorize]` attributes on endpoints

## Shared Libraries

- **Common**: Shared utilities, Result pattern, Auth helpers
- **Common.Web**: Web-specific extensions, Swagger configuration
- **Contracts**: Shared DTOs and integration event definitions
- **Shared/Kernel**: Domain primitives (Error, Money, Result)
- **Shared/Messaging**: MassTransit messaging configuration
- **CoursePlatform.ServiceDefaults**: .NET Aspire service configuration defaults

## Best Practices

1. **Separation of Concerns**: Keep domain logic in Domain layer, infrastructure concerns in Infrastructure layer
2. **Immutability**: Prefer immutable value objects and DTOs
3. **Explicit Dependencies**: Make dependencies clear through constructor injection
4. **Single Responsibility**: Each class should have one reason to change
5. **Domain-Centric Design**: Domain layer should have no external dependencies
6. **Event-Driven Communication**: Use integration events for cross-service communication
7. **Database Independence**: Each service owns its data; no shared databases
8. **Idempotency**: Ensure message handlers are idempotent
9. **Optimistic Concurrency**: Use version fields to detect conflicts
10. **Comprehensive Testing**: Write unit, integration, and architecture tests

## When Adding New Features

1. Start with domain layer (entities, value objects, events)
2. Add application layer (commands, queries, handlers)
3. Implement infrastructure (repositories, event handlers)
4. Create API endpoints
5. Add integration events if cross-service communication is needed
6. Write tests at all layers
7. Update Swagger documentation
8. Consider database migrations

## Additional Notes

- Follow existing patterns in the codebase for consistency
- EnrollmentService uses traditional N-Layer architecture (different from other services)
- All services support .NET Aspire orchestration
- Use strongly-typed IDs to prevent primitive obsession
- Prefer Records over Classes for DTOs and value objects
