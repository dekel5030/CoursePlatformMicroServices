import { useState, useMemo } from 'react';
import { Loader2 } from 'lucide-react';
import Switch from '@/components/ui/Switch/Switch';
import Modal from '@/components/ui/Modal/Modal';
import Badge from '@/components/ui/Badge/Badge';
import AddPermissionModal from '../AddPermissionModal';
import { useRoleManagement } from '../../hooks';
import { groupPermissionsByCategory } from '../../utils/permissionUtils';
import type { PermissionDto, AddPermissionRequest } from '../../types';
import type { ApiErrorResponse } from '@/api/axiosClient';
import styles from './PermissionMatrix.module.css';

interface PermissionMatrixProps {
  isOpen: boolean;
  onClose: () => void;
  roleName: string;
  permissions: PermissionDto[];
}

export default function PermissionMatrix({
  isOpen,
  onClose,
  roleName,
  permissions,
}: PermissionMatrixProps) {
  const [error, setError] = useState<string | null>(null);
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const { addPermission, removePermission } = useRoleManagement(roleName);

  const groupedPermissions = useMemo(() => {
    return groupPermissionsByCategory(permissions);
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

  const handleAddPermission = async (permission: AddPermissionRequest) => {
    setError(null);
    try {
      await addPermission.mutateAsync(permission);
      setIsAddModalOpen(false);
    } catch (err) {
      const apiError = err as ApiErrorResponse;
      setError(apiError.message || 'Failed to add permission');
      throw err;
    }
  };

  const isPermissionEnabled = (permission: PermissionDto) => {
    return permissions.some((p) => p.key === permission.key);
  };

  return (
    <>
      <Modal isOpen={isOpen} onClose={onClose} title={`Permission Matrix - ${roleName}`}>
        <div className={styles.container}>
          <div className={styles.headerActions}>
            <button onClick={() => setIsAddModalOpen(true)} className={styles.addButton}>
              + Add Permission
            </button>
          </div>

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
              <button onClick={() => setIsAddModalOpen(true)} className={styles.addButtonEmpty}>
                Add your first permission
              </button>
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

      <AddPermissionModal
        isOpen={isAddModalOpen}
        onClose={() => setIsAddModalOpen(false)}
        onSubmit={handleAddPermission}
        isLoading={addPermission.isPending}
      />
    </>
  );
}
