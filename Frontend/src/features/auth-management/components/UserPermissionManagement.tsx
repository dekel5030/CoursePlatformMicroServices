import { useState } from 'react';
import { useAuthUser, useUserManagement } from '../hooks';
import { Button, Skeleton } from '@/components/ui';
import PermissionBadge from './PermissionBadge';
import AddPermissionModal from './AddPermissionModal';
import ConfirmationModal from './ConfirmationModal';
import type { AddPermissionRequest } from '../types';
import type { ApiErrorResponse } from '@/api/axiosClient';
import { useTranslation } from 'react-i18next';

interface UserPermissionManagementProps {
  userId: string;
}

export default function UserPermissionManagement({ userId }: UserPermissionManagementProps) {
  const { t } = useTranslation();
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
      <div className="space-y-6">
        <div className="flex items-start justify-between">
          <div className="space-y-2">
            <Skeleton className="h-8 w-48" />
            <Skeleton className="h-4 w-64" />
          </div>
          <Skeleton className="h-10 w-32" />
        </div>
        <div className="space-y-3">
          {[1, 2, 3].map((i) => (
            <Skeleton key={i} className="h-12 w-full rounded-lg" />
          ))}
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-destructive/15 text-destructive px-4 py-3 rounded-md">
        {t('authManagement.users.permissions.failedToLoad', { message: error.message })}
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-start justify-between">
        <div className="space-y-1">
          <h2 className="text-2xl font-bold">{t('authManagement.users.permissions.title')}</h2>
          <p className="text-sm text-muted-foreground">
            {t('authManagement.users.permissions.subtitle')}
          </p>
        </div>
        <Button onClick={() => setIsAddModalOpen(true)}>
          {t('authManagement.users.permissions.addPermission')}
        </Button>
      </div>

      {user && user.permissions.length === 0 ? (
        <div className="text-center py-12 text-muted-foreground">
          <p>{t('authManagement.users.permissions.noPermissions')}</p>
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
        title={t('authManagement.users.permissions.removeTitle')}
        message={t('authManagement.users.permissions.removeMessage')}
        confirmText={t('authManagement.users.permissions.remove')}
        isLoading={removePermission.isPending}
        error={removeError?.message}
      />
    </div>
  );
}
