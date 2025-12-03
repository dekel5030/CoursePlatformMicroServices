namespace Kernel.Auth.AuthTypes;

public enum ActionType
{
    /// <summary>Create a resource.</summary>
    Create,
    /// <summary>Read a resource.</summary>
    Read,
    /// <summary>Update a resource.</summary>
    Update,
    /// <summary>Delete a resource.</summary>
    Delete,
    /// <summary>Wildcard for all actions.</summary>
    Wildcard
}