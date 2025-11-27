export const API_ADMIN_URL = "/admin";

export interface Permission {
  id: number;
  name: string;
}

export interface Role {
  id: number;
  name: string;
  permissions: Permission[];
}

export interface User {
  id: string;
  email: string;
  isActive: boolean;
  roles: string[];
  directPermissions: string[];
}

export async function getAllPermissions(): Promise<Permission[]> {
  const response = await fetch(`${API_ADMIN_URL}/permissions`, {
    method: "GET",
    credentials: "include",
  });

  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.title || "Failed to fetch permissions");
  }

  return response.json();
}

export async function createPermission(name: string): Promise<Permission> {
  const response = await fetch(`${API_ADMIN_URL}/permissions`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify({ name }),
  });

  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.title || "Failed to create permission");
  }

  return response.json();
}

export async function deletePermission(id: number): Promise<void> {
  const response = await fetch(`${API_ADMIN_URL}/permissions/${id}`, {
    method: "DELETE",
    credentials: "include",
  });

  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.title || "Failed to delete permission");
  }
}

export async function getAllRoles(): Promise<Role[]> {
  const response = await fetch(`${API_ADMIN_URL}/roles`, {
    method: "GET",
    credentials: "include",
  });

  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.title || "Failed to fetch roles");
  }

  return response.json();
}

export async function createRole(name: string): Promise<Role> {
  const response = await fetch(`${API_ADMIN_URL}/roles`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify({ name }),
  });

  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.title || "Failed to create role");
  }

  return response.json();
}

export async function deleteRole(id: number): Promise<void> {
  const response = await fetch(`${API_ADMIN_URL}/roles/${id}`, {
    method: "DELETE",
    credentials: "include",
  });

  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.title || "Failed to delete role");
  }
}

export async function assignPermissionToRole(
  roleId: number,
  permissionId: number
): Promise<void> {
  const response = await fetch(
    `${API_ADMIN_URL}/roles/${roleId}/permissions/${permissionId}`,
    {
      method: "POST",
      credentials: "include",
    }
  );

  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.title || "Failed to assign permission to role");
  }
}

export async function removePermissionFromRole(
  roleId: number,
  permissionId: number
): Promise<void> {
  const response = await fetch(
    `${API_ADMIN_URL}/roles/${roleId}/permissions/${permissionId}`,
    {
      method: "DELETE",
      credentials: "include",
    }
  );

  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.title || "Failed to remove permission from role");
  }
}

export async function getAllUsers(): Promise<User[]> {
  const response = await fetch(`${API_ADMIN_URL}/users`, {
    method: "GET",
    credentials: "include",
  });

  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.title || "Failed to fetch users");
  }

  return response.json();
}

export async function assignPermissionToUser(
  userId: string,
  permissionId: number
): Promise<void> {
  const response = await fetch(
    `${API_ADMIN_URL}/users/${userId}/permissions/${permissionId}`,
    {
      method: "POST",
      credentials: "include",
    }
  );

  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.title || "Failed to assign permission to user");
  }
}

export async function removePermissionFromUser(
  userId: string,
  permissionId: number
): Promise<void> {
  const response = await fetch(
    `${API_ADMIN_URL}/users/${userId}/permissions/${permissionId}`,
    {
      method: "DELETE",
      credentials: "include",
    }
  );

  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.title || "Failed to remove permission from user");
  }
}

export async function assignRoleToUser(
  userId: string,
  roleId: number
): Promise<void> {
  const response = await fetch(
    `${API_ADMIN_URL}/users/${userId}/roles/${roleId}`,
    {
      method: "POST",
      credentials: "include",
    }
  );

  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.title || "Failed to assign role to user");
  }
}

export async function removeRoleFromUser(
  userId: string,
  roleId: number
): Promise<void> {
  const response = await fetch(
    `${API_ADMIN_URL}/users/${userId}/roles/${roleId}`,
    {
      method: "DELETE",
      credentials: "include",
    }
  );

  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.title || "Failed to remove role from user");
  }
}
