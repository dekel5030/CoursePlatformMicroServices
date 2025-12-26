import { useState, useEffect } from 'react';
import { Loader2 } from 'lucide-react';
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
} from '@/components/ui';
import { Badge } from '@/components/ui';
import { Combobox } from '@/components/ui';
import { useUserManagement, useRoles } from '../../hooks';
import type { UserDto } from '../../types';
import type { ApiErrorResponse } from '@/api/axiosClient';

interface UserDrawerProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  user: UserDto | null;
}

export default function UserDrawer({ open, onOpenChange, user }: UserDrawerProps) {
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
      // Execute all role changes in parallel for better performance
      await Promise.all([
        ...rolesToAdd.map(roleName => addRole.mutateAsync({ roleName })),
        ...rolesToRemove.map(roleName => removeRole.mutateAsync(roleName))
      ]);

      setSelectedRoles(newRoles);
    } catch (err) {
      const apiError = err as ApiErrorResponse;
      setError(apiError.message || 'Failed to update roles');
    }
  };

  return (
    <Sheet open={open} onOpenChange={onOpenChange}>
      <SheetContent className="w-full sm:max-w-md overflow-y-auto">
        <SheetHeader>
          <SheetTitle>Edit User</SheetTitle>
        </SheetHeader>

        <div className="space-y-6 mt-6">
          <div className="space-y-3">
            <h3 className="text-sm font-semibold text-foreground">User Information</h3>
            <div className="space-y-2">
              <div className="flex flex-col">
                <span className="text-xs text-muted-foreground">Name</span>
                <span className="text-sm font-medium">
                  {user.firstName} {user.lastName}
                </span>
              </div>
              <div className="flex flex-col">
                <span className="text-xs text-muted-foreground">Email</span>
                <span className="text-sm font-medium">{user.email}</span>
              </div>
            </div>
          </div>

          <div className="space-y-3">
            <h3 className="text-sm font-semibold text-foreground">Assigned Roles</h3>
            {error && (
              <div className="bg-destructive/15 text-destructive px-3 py-2 rounded-md text-sm">
                {error}
              </div>
            )}
            
            {rolesLoading ? (
              <div className="flex items-center gap-2 text-muted-foreground">
                <Loader2 className="animate-spin" size={20} />
                <span className="text-sm">Loading roles...</span>
              </div>
            ) : (
              <Combobox
                options={roleOptions}
                value={selectedRoles}
                onChange={handleRoleChange}
                placeholder="Select roles..."
              />
            )}

            {(addRole.isPending || removeRole.isPending) && (
              <div className="flex items-center gap-2 text-muted-foreground text-sm">
                <Loader2 className="animate-spin" size={16} />
                <span>Saving changes...</span>
              </div>
            )}
          </div>

          <div className="space-y-3">
            <h3 className="text-sm font-semibold text-foreground">Permissions</h3>
            {user.permissions.length === 0 ? (
              <p className="text-sm text-muted-foreground">No direct permissions assigned</p>
            ) : (
              <div className="space-y-2">
                {user.permissions.map((permission) => (
                  <div key={permission.key} className="flex items-center gap-2 p-2 rounded-md border border-border bg-muted/50">
                    <Badge variant={permission.effect === 'Allow' ? 'default' : 'destructive'}>
                      {permission.effect}
                    </Badge>
                    <span className="text-sm">
                      {permission.action} on {permission.resource}
                      {permission.resourceId && ` (${permission.resourceId})`}
                    </span>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </SheetContent>
    </Sheet>
  );
}
