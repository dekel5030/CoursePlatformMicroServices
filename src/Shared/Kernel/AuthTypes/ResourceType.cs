namespace Kernel.AuthTypes;

/// <summary>
/// Resources that can be accessed.
/// </summary>
public enum ResourceType
{
    Course,
    Lesson,
    User,
    Enrollment,
    /// <summary>Wildcard for all resources.</summary>
    Wildcard
}