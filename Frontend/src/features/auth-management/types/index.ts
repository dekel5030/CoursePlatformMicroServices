export type PermissionDto = {
  key: string;
  effect: string;
  action: string;
  resource: string;
  resourceId: string;
};

export type RoleListItemDto = {
  id: string;
  name: string;
  permissionCount: number;
  userCount: number;
};

export type RoleDetailDto = {
  id: string;
  name: string;
  permissions: PermissionDto[];
};

export type PaginatedResponse<T> = {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
};

export type UserDto = {
  id: string;
  email: string;
  firstName: string | null;
  lastName: string | null;
  roles: UserRoleDto[];
  permissions: PermissionDto[];
};

export type UserRoleDto = {
  id: string;
  name: string;
};

export type AddPermissionRequest = {
  effect: string;
  action: string;
  resource: string;
  resourceId: string;
};

export type AddRoleRequest = {
  roleName: string;
};
