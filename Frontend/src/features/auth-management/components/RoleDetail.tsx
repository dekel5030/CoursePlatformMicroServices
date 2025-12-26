import { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Settings, ArrowLeft } from 'lucide-react';
import { Button, Skeleton } from '@/components/ui';
import { useRole } from '../hooks';
import PermissionBadge from './PermissionBadge';
import PermissionMatrix from './PermissionMatrix/PermissionMatrix';

export default function RoleDetail() {
  const navigate = useNavigate();
  const { roleName } = useParams<{ roleName: string }>();
  const { data: role, isLoading, error } = useRole(roleName);
  const [isMatrixOpen, setIsMatrixOpen] = useState(false);

  if (isLoading) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="space-y-4">
          <Skeleton className="h-10 w-48" />
          <Skeleton className="h-64 w-full" />
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="text-center space-y-4">
          <div className="bg-destructive/15 text-destructive px-4 py-3 rounded-md">
            Failed to load role: {error.message}
          </div>
          <Button onClick={() => navigate('/admin/roles')} variant="outline">
            <ArrowLeft className="mr-2 h-4 w-4" />
            Back to Roles
          </Button>
        </div>
      </div>
    );
  }

  if (!role) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="text-center space-y-4">
          <p className="text-muted-foreground">Role not found</p>
          <Button onClick={() => navigate('/admin/roles')} variant="outline">
            <ArrowLeft className="mr-2 h-4 w-4" />
            Back to Roles
          </Button>
        </div>
      </div>
    );
  }

  const topPermissions = role.permissions.slice(0, 3);

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-6">
      <div className="space-y-2">
        <Button
          onClick={() => navigate('/admin/roles')}
          variant="ghost"
          className="mb-4"
        >
          <ArrowLeft className="mr-2 h-4 w-4" />
          Back
        </Button>
        <h1 className="text-3xl font-bold">{role.name}</h1>
        <p className="text-muted-foreground">
          {role.permissions.length} {role.permissions.length === 1 ? 'permission' : 'permissions'}
        </p>
      </div>

      <div className="border border-border rounded-lg p-6 space-y-4">
        <div className="flex items-center justify-between">
          <h2 className="text-xl font-semibold">Top Permissions</h2>
          <Button onClick={() => setIsMatrixOpen(true)}>
            <Settings className="mr-2 h-4 w-4" />
            Manage All Permissions
          </Button>
        </div>

        {role.permissions.length === 0 ? (
          <div className="text-center py-8 text-muted-foreground">
            <p>No permissions assigned to this role</p>
          </div>
        ) : (
          <div className="space-y-3">
            {topPermissions.map((permission) => (
              <PermissionBadge
                key={permission.key}
                permission={permission}
                showRemove={false}
              />
            ))}
            {role.permissions.length > 3 && (
              <Button
                onClick={() => setIsMatrixOpen(true)}
                variant="link"
                className="w-full"
              >
                View all {role.permissions.length} permissions →
              </Button>
            )}
          </div>
        )}
      </div>

      <PermissionMatrix
        open={isMatrixOpen}
        onOpenChange={setIsMatrixOpen}
        roleName={roleName!}
        permissions={role.permissions}
      />
    </div>
  );
}
