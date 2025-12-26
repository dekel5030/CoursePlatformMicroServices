import { useState, useMemo } from 'react';
import { Loader2 } from 'lucide-react';
import Switch from '@/components/ui/Switch/Switch';
import Modal from '@/components/ui/Modal/Modal';
import Badge from '@/components/ui/Badge/Badge';
import { useRoleManagement } from '../../hooks';
import type { PermissionDto, AddPermissionRequest } from '../../types';
import type { ApiErrorResponse } from '@/api/axiosClient';
import styles from './PermissionMatrix.module.css';

interface PermissionMatrixProps {
  isOpen: boolean;
  onClose: () => void;
  roleName: string;
  permissions: PermissionDto[];
}

interface GroupedPermissions {
  [category: string]: PermissionDto[];
}

const PERMISSION_CATEGORIES: Record<string, string[]> = {
  'Course Management': ['Course', 'Courses'],
  'User Management': ['User', 'Users'],
  'Enrollment Management': ['Enrollment', 'Enrollments'],
  'Order Management': ['Order', 'Orders'],
  'Analytics': ['Analytics', 'Reports'],
  'System': ['System', 'Admin', 'Settings'],
};

function categorizePermission(permission: PermissionDto): string {
  for (const [category, resources] of Object.entries(PERMISSION_CATEGORIES)) {
    if (resources.some((r) => permission.resource.toLowerCase().includes(r.toLowerCase()))) {
      return category;
    }
  }
  return 'Other';
}

export default function PermissionMatrix({
  isOpen,
  onClose,
  roleName,
  permissions,
}: PermissionMatrixProps) {
  const [error, setError] = useState<string | null>(null);
  const { addPermission, removePermission } = useRoleManagement(roleName);

  const groupedPermissions = useMemo(() => {
    const grouped: GroupedPermissions = {};
    permissions.forEach((permission) => {
      const category = categorizePermission(permission);
      if (!grouped[category]) {
        grouped[category] = [];
      }
      grouped[category].push(permission);
    });
    return grouped;
  }, [permissions]);

  const handleTogglePermission = async (permission: PermissionDto, isEnabled: boolean) => {
    setError(null);
    try {
      if (isEnabled) {
        const request: AddPermissionRequest = {
          effect: permission.effect,
          action: permission.action,
          resource: permission.resource,
          resourceId: permission.resourceId,
        };
        await addPermission.mutateAsync(request);
      } else {
        await removePermission.mutateAsync(permission.key);
      }
    } catch (err) {
      const apiError = err as ApiErrorResponse;
      setError(apiError.message || 'Failed to update permission');
    }
  };

  const isPermissionEnabled = (permission: PermissionDto) => {
    return permissions.some((p) => p.key === permission.key);
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title={`Permission Matrix - ${roleName}`}>
      <div className={styles.container}>
        {error && <div className={styles.error}>{error}</div>}

        {(addPermission.isPending || removePermission.isPending) && (
          <div className={styles.loading}>
            <Loader2 className={styles.spinner} size={16} />
            <span>Updating permissions...</span>
          </div>
        )}

        {Object.keys(groupedPermissions).length === 0 ? (
          <div className={styles.empty}>
            <p>No permissions assigned to this role</p>
          </div>
        ) : (
          <div className={styles.categories}>
            {Object.entries(groupedPermissions).map(([category, perms]) => (
              <div key={category} className={styles.category}>
                <div className={styles.categoryHeader}>
                  <h3 className={styles.categoryTitle}>{category}</h3>
                  <Badge variant="default">{perms.length}</Badge>
                </div>
                <div className={styles.permissionList}>
                  {perms.map((permission) => (
                    <div key={permission.key} className={styles.permissionItem}>
                      <div className={styles.permissionInfo}>
                        <span className={styles.permissionAction}>{permission.action}</span>
                        <span className={styles.permissionResource}>on {permission.resource}</span>
                        {permission.resourceId && (
                          <Badge variant="info">{permission.resourceId}</Badge>
                        )}
                        <Badge variant={permission.effect === 'Allow' ? 'success' : 'error'}>
                          {permission.effect}
                        </Badge>
                      </div>
                      <Switch
                        checked={isPermissionEnabled(permission)}
                        onCheckedChange={(checked) => handleTogglePermission(permission, checked)}
                        disabled={addPermission.isPending || removePermission.isPending}
                      />
                    </div>
                  ))}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </Modal>
  );
}
