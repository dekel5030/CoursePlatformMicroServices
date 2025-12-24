namespace Kernel.Auth.AuthTypes;

/// <summary>
/// The effect type of a permission: either Allow or Deny.
/// </summary>
public enum EffectType
{
    /// <summary>Permission is allowed.</summary>
    Allow = 0,
    /// <summary>Permission is denied.</summary>
    Deny = 1
}