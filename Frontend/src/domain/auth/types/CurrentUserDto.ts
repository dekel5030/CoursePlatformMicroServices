import type { RoleDto } from "./RoleDto";
import type { PermissionDto } from "./PermissionDto";

export interface CurrentUserDto {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: RoleDto[];
  permissions: PermissionDto[];
}
