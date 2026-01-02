import { useState, useMemo } from "react";
import { Loader2 } from "lucide-react";
import { Switch, Badge, Button } from "@/components";
import AddPermissionModal from "./AddPermissionModal";
import { useUserManagement } from "../hooks";
import { groupPermissionsByCategory } from "../utils/permissionUtils";
import type { AddPermissionRequest } from "../types/AddPermissionRequest";
import type { PermissionDto } from "../types/PermissionDto";
import type { ApiErrorResponse } from "@/axios/axiosClient";

interface UserPermissionMatrixProps {
  userId: string;
  permissions: PermissionDto[];
}

export default function UserPermissionMatrix({
  userId,
  permissions,
}: UserPermissionMatrixProps) {
  const [error, setError] = useState<string | null>(null);
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);
  const { addPermission, removePermission } = useUserManagement(userId);

  const groupedPermissions = useMemo(() => {
    return groupPermissionsByCategory(permissions);
  }, [permissions]);

  const handleTogglePermission = async (
    permission: PermissionDto,
    isEnabled: boolean
  ) => {
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
      setError(apiError.message || "Failed to update permission");
    }
  };

  const handleAddPermission = async (permission: AddPermissionRequest) => {
    setError(null);
    try {
      await addPermission.mutateAsync(permission);
      setIsAddModalOpen(false);
    } catch (err) {
      const apiError = err as ApiErrorResponse;
      setError(apiError.message || "Failed to add permission");
      throw err;
    }
  };

  const isPermissionEnabled = (permission: PermissionDto) => {
    return permissions.some((p) => p.key === permission.key);
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h3 className="text-lg font-semibold">Permission Matrix</h3>
        <Button onClick={() => setIsAddModalOpen(true)} size="sm">
          + Add Permission
        </Button>
      </div>

      {error && (
        <div className="bg-destructive/15 text-destructive px-4 py-2 rounded-md text-sm">
          {error}
        </div>
      )}

      {(addPermission.isPending || removePermission.isPending) && (
        <div className="flex items-center gap-2 text-muted-foreground text-sm">
          <Loader2 className="animate-spin h-4 w-4" />
          <span>Updating permissions...</span>
        </div>
      )}

      {Object.keys(groupedPermissions).length === 0 ? (
        <div className="text-center py-8 space-y-4">
          <p className="text-muted-foreground">
            No permissions assigned to this user
          </p>
          <Button onClick={() => setIsAddModalOpen(true)} size="sm">
            Add your first permission
          </Button>
        </div>
      ) : (
        <div className="space-y-4">
          {Object.entries(groupedPermissions).map(([category, perms]) => (
            <div
              key={category}
              className="border border-border rounded-lg p-4 space-y-3"
            >
              <div className="flex items-center justify-between">
                <h4 className="text-sm font-semibold">{category}</h4>
                <Badge variant="secondary">{perms.length}</Badge>
              </div>
              <div className="space-y-2">
                {perms.map((permission) => (
                  <div
                    key={permission.key}
                    className="flex items-center justify-between p-3 rounded-md border border-border bg-muted/30 hover:bg-muted/50 transition-colors"
                  >
                    <div className="flex items-center gap-2 flex-wrap">
                      <span className="font-medium text-sm">
                        {permission.action}
                      </span>
                      <span className="text-sm text-muted-foreground">
                        on {permission.resource}
                      </span>
                      {permission.resourceId && (
                        <Badge variant="secondary" className="text-xs">
                          {permission.resourceId}
                        </Badge>
                      )}
                      <Badge
                        variant={
                          permission.effect === "Allow"
                            ? "default"
                            : "destructive"
                        }
                      >
                        {permission.effect}
                      </Badge>
                    </div>
                    <Switch
                      checked={isPermissionEnabled(permission)}
                      onCheckedChange={(checked) =>
                        handleTogglePermission(permission, checked)
                      }
                      disabled={
                        addPermission.isPending || removePermission.isPending
                      }
                    />
                  </div>
                ))}
              </div>
            </div>
          ))}
        </div>
      )}

      <AddPermissionModal
        open={isAddModalOpen}
        onOpenChange={setIsAddModalOpen}
        onSubmit={handleAddPermission}
        isLoading={addPermission.isPending}
      />
    </div>
  );
}
