# Infrastructure.IntegrationTests

This project contains integration tests for the AuthService Infrastructure layer using real database instances via Testcontainers. These tests verify the actual synchronization flow between Domain Entities and ASP.NET Core Identity tables.

## Structure

```
Infrastructure.IntegrationTests/
├── IntegrationTestsBase.cs       # Base class with Testcontainers setup
└── SyncFlow/
    ├── UserRoleSyncTests.cs       # Tests for User/Role synchronization
    └── PermissionSyncTests.cs     # Tests for Permission synchronization
```

## What We Test

### User & Role Synchronization (`UserRoleSyncTests`)
- **User Creation**: Domain `AuthUser` creation triggers `IdentityUser` creation
- **Role Creation**: Domain `Role` creation triggers `IdentityRole` creation
- **Role Assignment**: Adding roles to users syncs to `UserRoles` table
- **Role Removal**: Removing roles from users removes from `UserRoles` table
- **Multiple Users**: Independent user synchronization

### Permission Synchronization (`PermissionSyncTests`)
- **User Permissions**: Adding/removing permissions syncs to `UserClaims` table
- **Role Permissions**: Adding/removing permissions syncs to `RoleClaims` table
- **Claim Format**: Correct permission claim value formatting (`allow:action:resource:id`)
- **Multiple Permissions**: Multiple permissions create multiple claims correctly

## Technology Stack

- **xUnit**: Test framework
- **FluentAssertions**: Fluent assertion library
- **Testcontainers**: Docker containers for PostgreSQL and RabbitMQ
- **Microsoft.AspNetCore.Mvc.Testing**: WebApplicationFactory for integration testing

## Prerequisites

### Docker
Testcontainers requires Docker to be installed and running:

```bash
# Check if Docker is running
docker ps

# If Docker is not running, start Docker Desktop or Docker daemon
```

### Ports
The following ports must be available:
- PostgreSQL: Auto-assigned by Testcontainers
- RabbitMQ: 5674 (AMQP), 15674 (Management UI)

## Running the Tests

### From Command Line

```bash
# Ensure Docker is running first!

# Run all integration tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run from AuthService directory
cd src/AuthService
dotnet test Tests/Infrastructure.IntegrationTests

# Run a specific test class
dotnet test --filter "FullyQualifiedName~UserRoleSyncTests"
```

### From Visual Studio
1. **Ensure Docker Desktop is running**
2. Open Test Explorer (Test > Test Explorer)
3. Build the solution
4. Click "Run All" or select specific tests

## Test Lifecycle

Each test class inherits from `IntegrationTestsBase` which:

1. **Setup** (`InitializeAsync`):
   - Starts PostgreSQL container
   - Starts RabbitMQ container
   - Creates WebApplicationFactory with test configuration
   - Creates and initializes database schema
   - Starts external message bus

2. **Test Execution**:
   - Uses real database for all operations
   - Tests actual EF Core queries and SaveChanges
   - Verifies real synchronization behavior

3. **Cleanup** (`DisposeAsync`):
   - Stops message bus
   - Disposes containers
   - Cleans up resources

## Test Patterns

### Integration Test Example
```csharp
[Fact]
public async Task CreateAuthUser_ShouldSyncToIdentityUser()
{
    // Arrange
    using var scope = CreateScope();
    var dbContext = GetWriteDbContext(scope);
    
    var role = Role.Create("User").Value;
    dbContext.DomainRoles.Add(role);
    await dbContext.SaveChangesAsync();
    
    var authUser = AuthUser.Create("test@example.com", "testuser", role).Value;

    // Act
    dbContext.DomainUsers.Add(authUser);
    await dbContext.SaveChangesAsync();

    // Assert - Check both domain and identity tables
    var savedAuthUser = await dbContext.DomainUsers
        .FirstOrDefaultAsync(u => u.Id == authUser.Id);
    savedAuthUser.Should().NotBeNull();
    
    var identityUser = await dbContext.Users
        .FirstOrDefaultAsync(u => u.Id == authUser.Id);
    identityUser.Should().NotBeNull();
    identityUser!.Email.Should().Be("test@example.com");
}
```

## Coverage

Currently testing:
- ✅ User creation and Identity sync (6 tests)
- ✅ Role creation and Identity sync (6 tests)
- ✅ User-Role assignment/removal (6 tests)
- ✅ User permission sync (7 tests)
- ✅ Role permission sync (7 tests)

Total: **~19 integration tests**

## Troubleshooting

### Docker not available
```
Error: Testcontainers.Exceptions.DockerApiException: Docker is not running
```
**Solution**: Start Docker Desktop or Docker daemon

### Port conflicts
```
Error: Port 5674 is already in use
```
**Solution**: Stop other services using the port or change the port in `IntegrationTestsBase`

### Tests are slow
Integration tests are slower than unit tests because they:
- Start actual Docker containers
- Create real database schemas
- Execute real database queries

This is expected and ensures real behavior is tested.

### Database cleanup issues
If tests fail with schema errors, the database might not have been cleaned up properly between runs. The `InitializeAsync` method calls `EnsureDeletedAsync()` to prevent this.

## Adding New Tests

When adding new integration tests:

1. Create a test class that inherits from `IntegrationTestsBase`
2. Use `CreateScope()` to get a fresh service scope
3. Use `GetWriteDbContext(scope)` to access the database
4. Test the full flow from domain operation to Identity table verification
5. Always clean up resources with `using` statements

## Performance Considerations

- **Parallel Execution**: Tests in different classes can run in parallel
- **Container Reuse**: Testcontainers may cache images for faster startup
- **Test Isolation**: Each test gets a fresh database schema

## CI/CD Integration

These tests work in CI/CD pipelines that support Docker:
- GitHub Actions (with Docker service)
- Azure DevOps (with Docker agent)
- GitLab CI (with Docker-in-Docker)

Ensure the CI environment has:
- Docker installed and running
- Sufficient permissions to create containers
- Network access for pulling container images
