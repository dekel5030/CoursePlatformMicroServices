namespace Courses.Application.Abstractions.Data.ReadModels;

/// <summary>
/// Core Read Model for User projection (Instructor context).
/// Synchronized from UserService via UserCreated/UserUpdated events.
/// Enables de-normalized instructor info in course queries without WriteDbContext.
/// </summary>
public sealed class InstructorReadModel
{
    /// <summary>
    /// Primary Key - UserId (Point Lookup)
    /// </summary>
    public Guid Id { get; set; }

    // Identity
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Computed
    public string FullName { get; set; } = string.Empty;

    // Media
    public string? AvatarUrl { get; set; }

    // Timestamps
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}
