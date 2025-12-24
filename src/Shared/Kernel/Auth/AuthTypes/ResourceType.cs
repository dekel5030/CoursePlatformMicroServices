namespace Kernel.Auth.AuthTypes;

/// <summary>
/// Resources that can be accessed.
/// </summary>
public enum ResourceType
{
    Course = 0,
    Lesson = 1,
    User = 2,
    Enrollment = 3,
    /// <summary>Wildcard for all resources.</summary>
    Wildcard = 999
}