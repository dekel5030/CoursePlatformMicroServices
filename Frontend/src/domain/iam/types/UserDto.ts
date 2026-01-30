import type { UserRoleDto } from "./UserRoleDto";
import type { PermissionDto } from "./PermissionDto";

export type UserDto = {
  id: string;
  email: string;
  firstName: string | null;
  lastName: string | null;
  roles: UserRoleDto[];
  permissions: PermissionDto[];
};
