import { useNavigate } from 'react-router-dom';
import { Users, Shield } from 'lucide-react';
import { useRoles } from '../hooks';
import { Badge, Card, CardHeader, CardTitle, CardContent, CardFooter, Skeleton } from '@/components/ui';

export default function RoleList() {
  const navigate = useNavigate();
  const { data: roles, isLoading, error } = useRoles();

  if (isLoading) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {[1, 2, 3].map((i) => (
            <Skeleton key={i} className="h-40" />
          ))}
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="bg-destructive/15 text-destructive px-4 py-3 rounded-md">
          Failed to load roles: {error.message}
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-6">
      <div className="space-y-2">
        <h2 className="text-2xl font-bold">Roles</h2>
        <p className="text-muted-foreground">Manage security roles and their permissions</p>
      </div>

      {roles && roles.length === 0 ? (
        <div className="text-center py-12 text-muted-foreground">
          <p>No roles found</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {roles?.map((role) => (
            <Card
              key={role.id}
              className="cursor-pointer hover:shadow-lg transition-shadow"
              onClick={() => navigate(`/admin/roles/${encodeURIComponent(role.name)}`)}
            >
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Shield className="h-5 w-5 text-primary" />
                  {role.name}
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-1">
                    <div className="text-2xl font-bold">{role.userCount}</div>
                    <div className="text-sm text-muted-foreground flex items-center gap-1">
                      <Users className="h-3 w-3" />
                      {role.userCount === 1 ? 'User' : 'Users'}
                    </div>
                  </div>
                  <div className="space-y-1">
                    <div className="text-2xl font-bold">{role.permissionCount}</div>
                    <div className="text-sm text-muted-foreground">
                      {role.permissionCount === 1 ? 'Permission' : 'Permissions'}
                    </div>
                  </div>
                </div>
              </CardContent>
              <CardFooter>
                <Badge variant="secondary" className="text-xs">
                  {role.permissionCount} permissions
                </Badge>
              </CardFooter>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}
