import type { RoleDto } from "./RoleDto";
import type { PermissionDto } from "./PermissionDto";

export interface UserModel {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  roles: RoleDto[];
  permissions: PermissionDto[];
  avatarUrl?: string;
}
