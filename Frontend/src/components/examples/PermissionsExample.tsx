import { useCurrentUser, useHasPermission, useHasRole } from "@/hooks";
import { PermissionGuard } from "@/components/common/PermissionGuard";

/**
 * Example component demonstrating the usage of OIDC and permissions system
 * This component shows various ways to use the permission hooks and guards
 */
export function PermissionsExample() {
  const currentUser = useCurrentUser();
  const canCreateCourse = useHasPermission("Create", "Course");
  const canManageUsers = useHasPermission("Update", "User");
  const isAdmin = useHasRole("Admin");

  if (!currentUser) {
    return (
      <div className="p-4 border rounded">
        <h2 className="text-xl font-bold mb-2">Not Authenticated</h2>
        <p>Please log in to see this content.</p>
      </div>
    );
  }

  return (
    <div className="p-4 space-y-4">
      <div className="border rounded p-4">
        <h2 className="text-xl font-bold mb-2">Current User Information</h2>
        <dl className="space-y-2">
          <div>
            <dt className="font-semibold">Name:</dt>
            <dd>{currentUser.firstName} {currentUser.lastName}</dd>
          </div>
          <div>
            <dt className="font-semibold">Email:</dt>
            <dd>{currentUser.email}</dd>
          </div>
          <div>
            <dt className="font-semibold">Roles:</dt>
            <dd>{currentUser.roles.map(r => r.name).join(", ")}</dd>
          </div>
          <div>
            <dt className="font-semibold">Permissions Count:</dt>
            <dd>{currentUser.permissions.length}</dd>
          </div>
        </dl>
      </div>

      <div className="border rounded p-4">
        <h2 className="text-xl font-bold mb-2">Permission Checks (Using Hooks)</h2>
        <ul className="list-disc list-inside space-y-1">
          <li>Can Create Course: {canCreateCourse ? "✅ Yes" : "❌ No"}</li>
          <li>Can Manage Users: {canManageUsers ? "✅ Yes" : "❌ No"}</li>
          <li>Is Admin: {isAdmin ? "✅ Yes" : "❌ No"}</li>
        </ul>
      </div>

      <div className="border rounded p-4">
        <h2 className="text-xl font-bold mb-2">Permission Guards (Conditional Rendering)</h2>
        
        <PermissionGuard 
          action="Create" 
          resource="Course"
          fallback={<p className="text-red-600">❌ You cannot create courses</p>}
        >
          <p className="text-green-600">✅ You can create courses!</p>
        </PermissionGuard>

        <PermissionGuard 
          action="Delete" 
          resource="User"
          fallback={<p className="text-red-600 mt-2">❌ You cannot delete users</p>}
        >
          <p className="text-green-600 mt-2">✅ You can delete users!</p>
        </PermissionGuard>
      </div>

      <div className="border rounded p-4">
        <h2 className="text-xl font-bold mb-2">Role-Based Content</h2>
        {isAdmin ? (
          <div className="bg-blue-100 p-3 rounded">
            <p className="font-semibold">Admin Panel</p>
            <p className="text-sm">You have access to administrative features.</p>
          </div>
        ) : (
          <div className="bg-gray-100 p-3 rounded">
            <p className="text-sm">Standard user view</p>
          </div>
        )}
      </div>

      <div className="border rounded p-4">
        <h2 className="text-xl font-bold mb-2">All Permissions</h2>
        <div className="max-h-40 overflow-y-auto">
          <ul className="text-sm space-y-1">
            {currentUser.permissions.map((perm, idx) => (
              <li key={idx} className="font-mono text-xs">
                {perm.effect} {perm.action} on {perm.resource}
                {perm.resourceId !== "*" && ` (${perm.resourceId})`}
              </li>
            ))}
          </ul>
        </div>
      </div>
    </div>
  );
}
