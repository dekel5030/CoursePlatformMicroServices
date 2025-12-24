export interface PermissionDto {
  key: string;
  effect: string;
  action: string;
  resource: string;
  resourceId: string;
}

export interface RoleDto {
  id: string;
  name: string;
}

export interface UserDto {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: RoleDto[];
  permissions: PermissionDto[];
}

export interface RoleListDto {
  id: string;
  name: string;
  permissionCount: number;
}

export interface RoleDetailDto {
  id: string;
  name: string;
  permissions: PermissionDto[];
}

export interface CreateRoleRequest {
  name: string;
}

export interface CreateRoleResponse {
  id: string;
  name: string;
}

export interface UserAddRoleRequest {
  roleName: string;
}

export interface UserAddPermissionRequest {
  key: string;
  effect: string;
  action: string;
  resource: string;
  resourceId: string;
}

export interface RoleAddPermissionRequest {
  key: string;
  effect: string;
  action: string;
  resource: string;
  resourceId: string;
}
