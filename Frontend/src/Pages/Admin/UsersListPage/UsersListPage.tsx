import { useState } from "react";
import { Skeleton } from "@/components/ui";
import { UserTable, UserDrawer } from "@/features/auth-management";
import { useUsers } from "@/features/auth-management/hooks";
import type { UserDto } from "@/features/auth-management/types";

export default function UsersListPage() {
  const { data: users, isLoading, error } = useUsers();
  const [selectedUser, setSelectedUser] = useState<UserDto | null>(null);

  if (isLoading) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="space-y-4">
          <Skeleton className="h-12 w-64" />
          <Skeleton className="h-96 w-full" />
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="bg-destructive/15 text-destructive px-4 py-3 rounded-md">
          <h2 className="font-semibold mb-1">Failed to load users</h2>
          <p className="text-sm">{error.message}</p>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-6">
      <div className="space-y-2">
        <h1 className="text-3xl font-bold">User Management</h1>
        <p className="text-muted-foreground">Manage user roles and permissions</p>
      </div>

      {users && users.length === 0 ? (
        <div className="text-center py-12 text-muted-foreground">
          <p>No users found in the system</p>
        </div>
      ) : (
        <UserTable users={users || []} onUserSelect={setSelectedUser} />
      )}

      <UserDrawer
        open={!!selectedUser}
        onOpenChange={(open) => !open && setSelectedUser(null)}
        user={selectedUser}
      />
    </div>
  );
}
