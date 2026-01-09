import type { RoleDto } from "./RoleDto";
import type { PermissionDto } from "./PermissionDto";

/**
 * UI Model: Stable interface for UI components
 * Decoupled from backend API schema
 */
export interface UserModel {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  roles: RoleDto[];
  permissions: PermissionDto[];
}
