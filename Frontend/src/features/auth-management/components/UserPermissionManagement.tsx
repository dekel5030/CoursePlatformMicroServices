import { useState } from 'react';
import { useAuthUser, useUserManagement } from '../hooks';
import { Button, Skeleton } from '@/components/ui';
import PermissionBadge from './PermissionBadge';
import AddPermissionModal from './AddPermissionModal';
import ConfirmationModal from './ConfirmationModal';
import type { AddPermissionRequest } from '../types';
import type { ApiErrorResponse } from '@/api/axiosClient';

interface UserPermissionManagementProps {
  userId: string;
}

export default function UserPermissionManagement({ userId }: UserPermissionManagementProps) {
  const { data: user, isLoading, error } = useAuthUser(userId);
  const { addPermission, removePermission } = useUserManagement(userId);

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
      <div className="space-y-4">
        <Skeleton className="h-24 w-full" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-destructive/15 text-destructive px-4 py-3 rounded-md">
        Failed to load user data: {error.message}
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-start justify-between">
        <div className="space-y-1">
          <h2 className="text-2xl font-bold">Fine-Grained Permissions</h2>
          <p className="text-sm text-muted-foreground">
            User-specific permissions override role permissions
          </p>
        </div>
        <Button onClick={() => setIsAddModalOpen(true)}>
          + Add Permission
        </Button>
      </div>

      {user && user.permissions.length === 0 ? (
        <div className="text-center py-12 text-muted-foreground">
          <p>No fine-grained permissions assigned to this user</p>
        </div>
      ) : (
        <div className="space-y-2">
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
        open={isAddModalOpen}
        onOpenChange={setIsAddModalOpen}
        onSubmit={handleAddPermission}
        isLoading={addPermission.isPending}
      />

      <ConfirmationModal
        open={!!confirmRemove}
        onOpenChange={() => {
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
