using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace Auth.Application.UnitTests;

/// <summary>
/// Helper methods for unit tests
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// Creates a mock DbSet from a list of entities using MockQueryable for EF Core async support
    /// </summary>
    public static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
    {
        return data.AsQueryable().BuildMockDbSet();
    }
}
