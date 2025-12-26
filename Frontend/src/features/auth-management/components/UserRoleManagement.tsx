import { useState } from "react";
import { useAuthUser, useUserManagement } from "../hooks";
import { Button, Badge, Skeleton } from "@/components/ui";
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
          <h2 className="text-2xl font-bold">Role Assignments</h2>
          <p className="text-sm text-muted-foreground">
            Roles grant collections of permissions to users
          </p>
        </div>
        <Button onClick={() => setIsAddModalOpen(true)}>
          + Assign Role
        </Button>
      </div>

      {user && user.roles.length === 0 ? (
        <div className="text-center py-12 text-muted-foreground">
          <p>No roles assigned to this user</p>
        </div>
      ) : (
        <div className="flex flex-wrap gap-2">
          {user?.roles.map((role) => (
            <Badge
              key={role.id}
              variant="secondary"
              className="text-sm py-2 px-3 gap-2"
            >
              <span>{role.name}</span>
              <button
                onClick={() => setConfirmRemove(role.name)}
                className="ml-1 hover:text-destructive transition-colors"
                title="Remove role"
              >
                ×
              </button>
            </Badge>
          ))}
        </div>
      )}

      <AddRoleModal
        open={isAddModalOpen}
        onOpenChange={setIsAddModalOpen}
        onSubmit={async (roleName) => {
          await addRole.mutateAsync({ roleName });
          setIsAddModalOpen(false);
        }}
        isLoading={addRole.isPending}
      />

      <ConfirmationModal
        open={!!confirmRemove}
        onOpenChange={() => {
          setConfirmRemove(null);
          setRemoveError(null);
        }}
        onConfirm={handleRemoveRole}
        title="Remove Role"
        message="Are you sure you want to remove this role from the user? This action cannot be undone."
        confirmText="Remove"
        isLoading={removeRole.isPending}
        error={removeError?.message}
      />
    </div>
  );
}
