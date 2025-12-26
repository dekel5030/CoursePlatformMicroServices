import { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useRole, useRoleManagement } from '../hooks';
import PermissionBadge from './PermissionBadge';
import AddPermissionModal from './AddPermissionModal';
import ConfirmationModal from './ConfirmationModal';
import styles from './RoleDetail.module.css';
import type { AddPermissionRequest } from '../types';
import type { ApiErrorResponse } from '@/api/axiosClient';

export default function RoleDetail() {
  const navigate = useNavigate();
  const { roleName } = useParams<{ roleName: string }>();
  const { data: role, isLoading, error } = useRole(roleName);
  const { addPermission, removePermission } = useRoleManagement(roleName!);

  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const [confirmRemove, setConfirmRemove] = useState<string | null>(null);
  const [removeError, setRemoveError] = useState<ApiErrorResponse | null>(null);

  const handleAddPermission = async (permission: AddPermissionRequest) => {
    await addPermission.mutateAsync(permission);
    setIsAddModalOpen(false);
  };

  const handleRemovePermission = async () => {
    if (!confirmRemove) return;

    setRemoveError(null);
    try {
      await removePermission.mutateAsync(confirmRemove);
      setConfirmRemove(null);
      setRemoveError(null);
    } catch (err: unknown) {
      setRemoveError(err as ApiErrorResponse);
    }
  };

  if (isLoading) {
    return (
      <div className={styles.container}>
        <div className={styles.skeleton}>
          <div className={styles.skeletonHeader}></div>
          <div className={styles.skeletonContent}></div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className={styles.container}>
        <div className={styles.error}>
          <p>Failed to load role: {error.message}</p>
          <button onClick={() => navigate('/admin/roles')} className={styles.backButton}>
            Back to Roles
          </button>
        </div>
      </div>
    );
  }

  if (!role) {
    return (
      <div className={styles.container}>
        <div className={styles.error}>
          <p>Role not found</p>
          <button onClick={() => navigate('/admin/roles')} className={styles.backButton}>
            Back to Roles
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <button onClick={() => navigate('/admin/roles')} className={styles.backButton}>
          ← Back
        </button>
        <h1 className={styles.title}>{role.name}</h1>
        <p className={styles.subtitle}>
          {role.permissions.length} {role.permissions.length === 1 ? 'permission' : 'permissions'}
        </p>
      </div>

      <div className={styles.section}>
        <div className={styles.sectionHeader}>
          <h2 className={styles.sectionTitle}>Permissions</h2>
          <button onClick={() => setIsAddModalOpen(true)} className={styles.addButton}>
            + Add Permission
          </button>
        </div>

        {role.permissions.length === 0 ? (
          <div className={styles.empty}>
            <p>No permissions assigned to this role</p>
          </div>
        ) : (
          <div className={styles.permissions}>
            {role.permissions.map((permission) => (
              <PermissionBadge
                key={permission.key}
                permission={permission}
                showRemove
                onRemove={() => setConfirmRemove(permission.key)}
              />
            ))}
          </div>
        )}
      </div>

      <AddPermissionModal
        isOpen={isAddModalOpen}
        onClose={() => setIsAddModalOpen(false)}
        onSubmit={handleAddPermission}
        isLoading={addPermission.isPending}
      />

      <ConfirmationModal
        isOpen={!!confirmRemove}
        onClose={() => {
          setConfirmRemove(null);
          setRemoveError(null);
        }}
        onConfirm={handleRemovePermission}
        title="Remove Permission"
        message="Are you sure you want to remove this permission? This action cannot be undone."
        confirmText="Remove"
        isLoading={removePermission.isPending}
        error={removeError?.message}
      />
    </div>
  );
}
