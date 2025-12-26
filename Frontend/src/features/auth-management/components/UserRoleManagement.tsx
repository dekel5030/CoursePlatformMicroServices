import { useState } from "react";
import { useAuthUser, useUserManagement } from "../hooks";
import styles from "./UserRoleManagement.module.css";
import ConfirmationModal from "./ConfirmationModal";
import AddRoleModal from "./AddRoleModal";
import type { ApiErrorResponse } from "@/api/axiosClient";

interface UserRoleManagementProps {
  userId: string;
}

export default function UserRoleManagement({
  userId,
}: UserRoleManagementProps) {
  const { data: user, isLoading, error } = useAuthUser(userId);
  const { addRole, removeRole } = useUserManagement(userId);

  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const [confirmRemove, setConfirmRemove] = useState<string | null>(null);
  const [removeError, setRemoveError] = useState<ApiErrorResponse | null>(null);

  const handleRemoveRole = async () => {
    if (!confirmRemove) return;

    setRemoveError(null);
    try {
      await removeRole.mutateAsync(confirmRemove);
      setConfirmRemove(null);
      setRemoveError(null);
    } catch (err: unknown) {
      setRemoveError(err as ApiErrorResponse);
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
        <h2 className={styles.sectionTitle}>Assigned Roles</h2>
        <button
          onClick={() => setIsAddModalOpen(true)}
          className={styles.addButton}
        >
          + Add Role
        </button>
      </div>

      {user && user.roles.length === 0 ? (
        <div className={styles.empty}>
          <p>No roles assigned to this user</p>
        </div>
      ) : (
        <div className={styles.roles}>
          {user?.roles.map((role) => (
            <div key={role.id} className={styles.roleCard}>
              <span className={styles.roleName}>{role.name}</span>
              <button
                className={styles.removeButton}
                onClick={() => setConfirmRemove(role.name)}
                title="Remove role"
              >
                ×
              </button>
            </div>
          ))}
        </div>
      )}

      <AddRoleModal
        isOpen={isAddModalOpen}
        onClose={() => setIsAddModalOpen(false)}
        onSubmit={(roleName) => addRole.mutateAsync({ roleName })}
        isLoading={addRole.isPending}
      />

      <ConfirmationModal
        isOpen={!!confirmRemove}
        onClose={() => {
          setConfirmRemove(null);
          setRemoveError(null);
        }}
        onConfirm={handleRemoveRole}
        title="Remove Role"
        message={`Are you sure you want to remove the role "${confirmRemove}"? This action cannot be undone.`}
        confirmText="Remove"
        isLoading={removeRole.isPending}
        error={removeError?.message}
      />
    </div>
  );
}
