import { useState } from 'react';
import { useAuthUser, useUserManagement } from '../hooks';
import styles from './UserPermissionManagement.module.css';
import PermissionBadge from './PermissionBadge';
import AddPermissionModal from './AddPermissionModal';
import ConfirmationModal from './ConfirmationModal';
import type { AddPermissionRequest } from '../types';

interface UserPermissionManagementProps {
  userId: string;
}

export default function UserPermissionManagement({ userId }: UserPermissionManagementProps) {
  const { data: user, isLoading, error } = useAuthUser(userId);
  const { addPermission, removePermission } = useUserManagement(userId);

  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const [confirmRemove, setConfirmRemove] = useState<string | null>(null);

  const handleAddPermission = async (permission: AddPermissionRequest) => {
    try {
      await addPermission.mutateAsync(permission);
      setIsAddModalOpen(false);
    } catch (err) {
      console.error('Failed to add permission', err);
    }
  };

  const handleRemovePermission = async () => {
    if (!confirmRemove) return;

    try {
      await removePermission.mutateAsync(confirmRemove);
      setConfirmRemove(null);
    } catch (err) {
      console.error('Failed to remove permission', err);
    }
  };

  if (isLoading) {
    return (
      <div className={styles.section}>
        <div className={styles.skeleton}></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className={styles.section}>
        <div className={styles.error}>
          <p>Failed to load user data: {error.message}</p>
        </div>
      </div>
    );
  }

  return (
    <div className={styles.section}>
      <div className={styles.sectionHeader}>
        <div>
          <h2 className={styles.sectionTitle}>Fine-Grained Permissions</h2>
          <p className={styles.sectionSubtitle}>User-specific permissions override role permissions</p>
        </div>
        <button onClick={() => setIsAddModalOpen(true)} className={styles.addButton}>
          + Add Permission
        </button>
      </div>

      {user && user.permissions.length === 0 ? (
        <div className={styles.empty}>
          <p>No fine-grained permissions assigned to this user</p>
        </div>
      ) : (
        <div className={styles.permissions}>
          {user?.permissions.map((permission) => (
            <PermissionBadge
              key={permission.key}
              permission={permission}
              showRemove
              onRemove={() => setConfirmRemove(permission.key)}
            />
          ))}
        </div>
      )}

      <AddPermissionModal
        isOpen={isAddModalOpen}
        onClose={() => setIsAddModalOpen(false)}
        onSubmit={handleAddPermission}
        isLoading={addPermission.isPending}
      />

      <ConfirmationModal
        isOpen={!!confirmRemove}
        onClose={() => setConfirmRemove(null)}
        onConfirm={handleRemovePermission}
        title="Remove Permission"
        message="Are you sure you want to remove this permission? This action cannot be undone."
        confirmText="Remove"
        isLoading={removePermission.isPending}
      />
    </div>
  );
}
