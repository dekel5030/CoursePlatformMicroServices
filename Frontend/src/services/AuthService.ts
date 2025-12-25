import { BaseService } from './BaseService';
import type {
  UserDto,
  RoleListDto,
  RoleDetailDto,
  CreateRoleRequest,
  CreateRoleResponse,
  UserAddRoleRequest,
  UserAddPermissionRequest,
  RoleAddPermissionRequest,
} from '@/types';

class AuthServiceClass extends BaseService {
  async getUserById(userId: string): Promise<UserDto> {
    return await this.get<UserDto>(`/admin/users/${userId}`);
  }

  async getAllRoles(): Promise<RoleListDto[]> {
    return await this.get<RoleListDto[]>('/admin/roles');
  }

  async getRoleByName(roleName: string): Promise<RoleDetailDto> {
    return await this.get<RoleDetailDto>(`/admin/roles/${roleName}`);
  }

  async createRole(request: CreateRoleRequest): Promise<CreateRoleResponse> {
    return await this.post<CreateRoleResponse>('/admin/roles', request);
  }

  async addUserRole(userId: string, request: UserAddRoleRequest): Promise<void> {
    await this.post<void>(`/admin/users/${userId}/roles`, request);
  }

  async removeUserRole(userId: string, roleName: string): Promise<void> {
    await this.delete<void>(`/admin/users/${userId}/roles/${roleName}`);
  }

  async addUserPermission(userId: string, request: UserAddPermissionRequest): Promise<void> {
    await this.post<void>(`/admin/users/${userId}/permissions`, request);
  }

  async removeUserPermission(userId: string, permissionKey: string): Promise<void> {
    await this.delete<void>(`/admin/users/${userId}/permissions/${permissionKey}`);
  }

  async addRolePermission(roleName: string, request: RoleAddPermissionRequest): Promise<void> {
    await this.post<void>(`/admin/roles/${roleName}/permissions`, request);
  }

  async removeRolePermission(roleName: string, permissionKey: string): Promise<void> {
    await this.delete<void>(`/admin/roles/${roleName}/permissions/${permissionKey}`);
  }
}

export const AuthService = new AuthServiceClass();
