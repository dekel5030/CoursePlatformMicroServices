/**
 * Mirror of backend Kernel.Auth.AuthTypes for type-safe authorization.
 * These enums match the C# backend definitions exactly.
 */

/**
 * Actions that can be performed on resources.
 * Mirrors: Kernel.Auth.AuthTypes.ActionType
 */
export const ActionType = {
  Read: "read",
  Create: "create",
  Update: "update",
  Delete: "delete",
  Wildcard: "*",
} as const;

export type ActionType = (typeof ActionType)[keyof typeof ActionType];

/**
 * Resources that can be accessed.
 * Mirrors: Kernel.Auth.AuthTypes.ResourceType
 */
export const ResourceType = {
  Course: "course",
  Lesson: "lesson",
  User: "user",
  Enrollment: "enrollment",
  Wildcard: "*",
} as const;

export type ResourceType = (typeof ResourceType)[keyof typeof ResourceType];

/**
 * Resource ID wrapper for specific resource instances.
 * Mirrors: Kernel.Auth.AuthTypes.ResourceId
 */
export interface ResourceId {
  readonly value: string;
  readonly isWildcard: boolean;
}

/**
 * ResourceId factory and utilities
 */
export const ResourceId = {
  Wildcard: { value: "*", isWildcard: true } as ResourceId,
  
  create(value: string): ResourceId {
    if (value === "*") {
      return ResourceId.Wildcard;
    }
    return { value, isWildcard: false };
  },
} as const;

/**
 * Effect type for permissions (Allow or Deny).
 * Mirrors: Kernel.Auth.AuthTypes.EffectType
 */
export const EffectType = {
  Allow: "allow",
  Deny: "deny",
} as const;

export type EffectType = (typeof EffectType)[keyof typeof EffectType];

/**
 * Type-safe permission structure.
 */
export interface Permission {
  action: ActionType;
  resource: ResourceType;
  resourceId: ResourceId;
}

