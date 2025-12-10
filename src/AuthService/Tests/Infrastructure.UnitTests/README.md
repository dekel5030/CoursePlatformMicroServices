# Infrastructure.UnitTests

This project contains unit tests for the AuthService Infrastructure layer, focusing on testing mapping logic and isolated components.

## Structure

The test project mirrors the folder structure of the Infrastructure project:

```
Infrastructure.UnitTests/
└── Identity/
    └── Mappers/
        ├── ApplicationIdentityUserTests.cs
        └── ApplicationIdentityRoleTests.cs
```

## What We Test

### Mapping Logic
- **ApplicationIdentityUser**: Verifies correct mapping from domain `AuthUser` to ASP.NET Core Identity `IdentityUser<Guid>`
  - ID preservation
  - Email mapping
  - Username mapping
  - Handling empty usernames

- **ApplicationIdentityRole**: Verifies correct mapping from domain `Role` to ASP.NET Core Identity `IdentityRole<Guid>`
  - ID preservation  
  - Name mapping
  - Normalized name generation

## Running the Tests

### From Command Line

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run tests from the AuthService directory
cd src/AuthService
dotnet test Tests/Infrastructure.UnitTests
```

### From Visual Studio
1. Open Test Explorer (Test > Test Explorer)
2. Build the solution
3. Click "Run All" to execute all tests

## Dependencies

- **xUnit**: Test framework
- **FluentAssertions**: Fluent assertion library for readable tests
- **Moq**: Mocking library for dependencies
- **Microsoft.NET.Test.Sdk**: Test SDK for running tests

## Test Patterns

### Arrange-Act-Assert (AAA)
All tests follow the AAA pattern:
```csharp
[Fact]
public void Constructor_WithAuthUser_ShouldMapIdCorrectly()
{
    // Arrange
    var role = Role.Create("User").Value;
    var authUser = AuthUser.Create("test@example.com", "testuser", role).Value;

    // Act
    var identityUser = new ApplicationIdentityUser(authUser);

    // Assert
    identityUser.Id.Should().Be(authUser.Id);
}
```

### Theory Tests
For testing multiple inputs:
```csharp
[Theory]
[InlineData("user1@example.com", "user1")]
[InlineData("admin@example.com", "administrator")]
public void Constructor_WithDifferentAuthUsers_ShouldMapAllFieldsCorrectly(
    string email, string userName)
{
    // Test implementation
}
```

## Coverage

Currently testing:
- ✅ ApplicationIdentityUser mapping (9 tests)
- ✅ ApplicationIdentityRole mapping (11 tests)

Total: **20 tests**

## Adding New Tests

When adding new infrastructure components that have isolated, testable logic:

1. Create a test class in the corresponding folder structure
2. Follow existing naming conventions (`{ClassName}Tests.cs`)
3. Use AAA pattern for test organization
4. Add XML documentation to explain test purpose
5. Use FluentAssertions for readable assertions

## Notes

- Unit tests focus on mapping logic and data transformations
- Sync event handlers are tested via integration tests (see Infrastructure.IntegrationTests)
- DbContext mocking is avoided in favor of testing actual behavior in integration tests
