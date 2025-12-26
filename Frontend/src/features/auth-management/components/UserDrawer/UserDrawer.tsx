import { useState, useEffect } from 'react';
import { Loader2 } from 'lucide-react';
import Drawer from '@/components/ui/Drawer/Drawer';
import Badge from '@/components/ui/Badge/Badge';
import MultiSelect from '@/components/ui/MultiSelect/MultiSelect';
import { useUserManagement, useRoles } from '../../hooks';
import type { UserDto } from '../../types';
import type { ApiErrorResponse } from '@/api/axiosClient';
import styles from './UserDrawer.module.css';

interface UserDrawerProps {
  isOpen: boolean;
  onClose: () => void;
  user: UserDto | null;
}

export default function UserDrawer({ isOpen, onClose, user }: UserDrawerProps) {
  const [selectedRoles, setSelectedRoles] = useState<string[]>([]);
  const [error, setError] = useState<string | null>(null);
  const { data: allRoles, isLoading: rolesLoading } = useRoles();
  const { addRole, removeRole } = useUserManagement(user?.id.toString() || '');

  useEffect(() => {
    if (user) {
      setSelectedRoles(user.roles.map((r) => r.name));
    }
  }, [user]);

  if (!user) return null;

  const roleOptions = allRoles?.map((role) => ({
    value: role.name,
    label: role.name,
  })) || [];

  const handleRoleChange = async (newRoles: string[]) => {
    setError(null);

    const rolesToAdd = newRoles.filter((role) => !selectedRoles.includes(role));
    const rolesToRemove = selectedRoles.filter((role) => !newRoles.includes(role));

    try {
      // Add roles
      for (const roleName of rolesToAdd) {
        await addRole.mutateAsync({ roleName });
      }

      // Remove roles
      for (const roleName of rolesToRemove) {
        await removeRole.mutateAsync(roleName);
      }

      setSelectedRoles(newRoles);
    } catch (err) {
      const apiError = err as ApiErrorResponse;
      setError(apiError.message || 'Failed to update roles');
    }
  };

  return (
    <Drawer isOpen={isOpen} onClose={onClose} title="Edit User">
      <div className={styles.content}>
        <div className={styles.section}>
          <h3 className={styles.sectionTitle}>User Information</h3>
          <div className={styles.infoGrid}>
            <div className={styles.infoItem}>
              <span className={styles.infoLabel}>Name</span>
              <span className={styles.infoValue}>
                {user.firstName} {user.lastName}
              </span>
            </div>
            <div className={styles.infoItem}>
              <span className={styles.infoLabel}>Email</span>
              <span className={styles.infoValue}>{user.email}</span>
            </div>
          </div>
        </div>

        <div className={styles.section}>
          <h3 className={styles.sectionTitle}>Assigned Roles</h3>
          {error && <div className={styles.error}>{error}</div>}
          
          {rolesLoading ? (
            <div className={styles.loading}>
              <Loader2 className={styles.spinner} size={20} />
              <span>Loading roles...</span>
            </div>
          ) : (
            <MultiSelect
              options={roleOptions}
              selected={selectedRoles}
              onChange={handleRoleChange}
              placeholder="Select roles..."
              disabled={addRole.isPending || removeRole.isPending}
            />
          )}

          {(addRole.isPending || removeRole.isPending) && (
            <div className={styles.savingIndicator}>
              <Loader2 className={styles.spinner} size={16} />
              <span>Saving changes...</span>
            </div>
          )}
        </div>

        <div className={styles.section}>
          <h3 className={styles.sectionTitle}>Permissions</h3>
          {user.permissions.length === 0 ? (
            <p className={styles.noPermissions}>No direct permissions assigned</p>
          ) : (
            <div className={styles.permissions}>
              {user.permissions.map((permission) => (
                <div key={permission.key} className={styles.permission}>
                  <Badge variant={permission.effect === 'Allow' ? 'success' : 'error'}>
                    {permission.effect}
                  </Badge>
                  <span className={styles.permissionText}>
                    {permission.action} on {permission.resource}
                    {permission.resourceId && ` (${permission.resourceId})`}
                  </span>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </Drawer>
  );
}
