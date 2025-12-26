import type { PermissionDto } from '../types';

export interface GroupedPermissions {
  [category: string]: PermissionDto[];
}

export const PERMISSION_CATEGORIES: Record<string, string[]> = {
  'Course Management': ['Course', 'Courses'],
  'User Management': ['User', 'Users'],
  'Enrollment Management': ['Enrollment', 'Enrollments'],
  'Order Management': ['Order', 'Orders'],
  'Analytics': ['Analytics', 'Reports'],
  'System': ['System', 'Admin', 'Settings'],
};

export function categorizePermission(permission: PermissionDto): string {
  for (const [category, resources] of Object.entries(PERMISSION_CATEGORIES)) {
    if (resources.some((r) => permission.resource.toLowerCase().includes(r.toLowerCase()))) {
      return category;
    }
  }
  return 'Other';
}

export function groupPermissionsByCategory(permissions: PermissionDto[]): GroupedPermissions {
  const grouped: GroupedPermissions = {};
  permissions.forEach((permission) => {
    const category = categorizePermission(permission);
    if (!grouped[category]) {
      grouped[category] = [];
    }
    grouped[category].push(permission);
  });
  return grouped;
}
