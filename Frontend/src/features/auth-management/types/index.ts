export interface PermissionDto {
  key: string;
  effect: string;
  action: string;
  resource: string;
  resourceId: string;
}

export interface RoleListItemDto {
  id: string;
  name: string;
  permissionCount: number;
}

export interface RoleDetailDto {
  id: string;
  name: string;
  permissions: PermissionDto[];
}

export interface UserDto {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: UserRoleDto[];
  permissions: PermissionDto[];
}

export interface UserRoleDto {
  id: string;
  name: string;
}

export interface AddPermissionRequest {
  effect: string;
  action: string;
  resource: string;
  resourceId: string;
}

export interface AddRoleRequest {
  roleName: string;
}
