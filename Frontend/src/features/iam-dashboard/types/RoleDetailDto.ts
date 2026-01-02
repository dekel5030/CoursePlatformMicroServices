import type { PermissionDto } from "./PermissionDto";

export type RoleDetailDto = {
  id: string;
  name: string;
  permissions: PermissionDto[];
};
