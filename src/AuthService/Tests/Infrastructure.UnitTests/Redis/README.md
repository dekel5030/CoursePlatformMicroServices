# Redis Utilities Test Coverage

This document describes the test coverage for the AuthService Redis utilities located in `Infrastructure/Redis`.

## Test Coverage Summary

All functional classes in the Redis folder have comprehensive unit test coverage:

### ✅ RedisRolePermissionsCacheWriter (9 tests)
**Location:** `Infrastructure.UnitTests/Redis/RedisRolePermissionsCacheWriterTests.cs`

Covers:
- Writing role permissions to Redis cache
- Correct Redis key generation (lowercase role names)
- JSON serialization of `RolePermissionsCacheModel`
- Empty permissions collections
- Multiple permissions handling
- Preservation of permission order
- Various role names

**Edge Cases Tested:**
- Empty permissions collection
- Multiple permissions
- Role name case sensitivity (converted to lowercase in key)
- Permission order preservation

### ✅ RoleEventsCollector (11 tests)
**Location:** `Infrastructure.UnitTests/Redis/EventCollector/RoleEventsCollectorTests.cs`

Covers:
- Marking single and multiple roles for refresh
- Publishing events on flush
- Clearing collected roles after flush
- Duplicate role name deduplication
- Multiple mark and flush cycles
- Cancellation token handling

**Edge Cases Tested:**
- Duplicate role names (deduplicated via HashSet)
- Empty flush (no roles marked)
- Multiple flush cycles
- Role accumulation between flushes

### ✅ RoleCacheInvalidationHandler (19 tests)
**Location:** `Infrastructure.UnitTests/Redis/EventHandlers/RoleCacheInvalidationHandlerTests.cs`

Covers:
- Handling `RoleCreatedDomainEvent`
- Handling `RolePermissionAddedDomainEvent`
- Handling `RolePermissionRemovedDomainEvent`
- Marking roles for refresh via collector
- Multiple events for same role
- Different role names and permissions

**Edge Cases Tested:**
- Multiple events for the same role
- Different permissions on the same role
- Different role names
- Cancellation token handling

## Components Not Tested (with Rationale)

### ⚠️ HostApplicationExtensions
**Location:** `Infrastructure/Redis/Extensions/HostApplicationExtensions.cs`

**Rationale for No Direct Tests:**
This is a dependency injection configuration extension method. Its correctness is implicitly verified by:
1. The integration tests that use the configured services
2. The unit tests that verify the actual implementations work correctly
3. Compile-time type safety ensuring correct service registrations

Testing DI registration methods typically requires:
- Setting up a full `IHostApplicationBuilder`
- Resolving services and verifying their types
- This is more appropriate for integration tests rather than unit tests

**Verification Strategy:**
The extension registers the following services which are all tested:
- `RoleCacheInvalidationHandler` → Tested in unit tests
- `RedisRolePermissionsCacheWriter` → Tested in unit tests
- `RoleEventsCollector` → Tested in unit tests

Since all implementations are thoroughly tested, and the method only performs service registration, additional testing would provide minimal value.

### ℹ️ Interfaces
The following interfaces have no tests (by design):
- `IRolePermissionsCacheWriter` - Interface only, no implementation logic
- `IRoleEventsCollector` - Interface only, no implementation logic

## Test Statistics

- **Total Test Files:** 3
- **Total Tests:** 39
- **Test Pass Rate:** 100%
- **Code Coverage:** All functional classes covered

## Running the Tests

```bash
# Run all Redis tests
cd src/AuthService
dotnet test Tests/Infrastructure.UnitTests/Infrastructure.UnitTests.csproj --filter "FullyQualifiedName~Redis"

# Run all infrastructure unit tests
dotnet test Tests/Infrastructure.UnitTests/Infrastructure.UnitTests.csproj
```

## Dependencies

The tests use the following testing frameworks and libraries:
- **xUnit** - Test framework
- **Moq** - Mocking framework for `IConnectionMultiplexer`, `IDatabase`, `IEventPublisher`, `IRoleEventsCollector`
- **FluentAssertions** - Assertion library for readable test assertions

## Notes

1. All tests are independent and can run in any order
2. Tests use mocking to avoid external dependencies (no real Redis instance needed)
3. Tests follow the Arrange-Act-Assert pattern
4. Tests include both positive and negative test cases
5. Tests use descriptive names that clearly state what is being tested

## Uncovered Edge Cases

While comprehensive, the following theoretical edge cases are not explicitly tested:

1. **Extremely Large Permissions Collections** - The tests don't verify behavior with thousands of permissions
   - *Mitigation:* Current tests cover the happy path and standard use cases
   
2. **Redis Connection Failures** - Tests use mocks, so actual Redis connection failures aren't simulated
   - *Mitigation:* This would be better covered in integration tests with real Redis instances
   
3. **Concurrent Access** - Thread safety of `RoleEventsCollector` HashSet is not explicitly tested
   - *Mitigation:* The class is registered as Scoped, so typically used within a single thread
   
4. **Very Long Role Names** - No tests for extremely long role names (Redis key length limits)
   - *Mitigation:* Domain validation should prevent invalid role names at creation time

5. **JSON Serialization Edge Cases** - Special characters in permissions strings
   - *Mitigation:* JSON serialization handles this by default; permissions follow a structured format

These edge cases would be better addressed through:
- Integration tests with real Redis instances
- Load testing for performance verification
- Domain-level validation for business rules
