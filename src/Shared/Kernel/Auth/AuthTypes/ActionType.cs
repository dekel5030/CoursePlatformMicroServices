namespace Kernel.Auth.AuthTypes;

public enum ActionType
{
    /// <summary>Read a resource.</summary>
    Read = 0,
    /// <summary>Create a resource.</summary>
    Create = 1,
    /// <summary>Update a resource.</summary>
    Update = 2,
    /// <summary>Delete a resource.</summary>
    Delete = 3,
    /// <summary>Wildcard for all actions.</summary>
    Wildcard = 99
}