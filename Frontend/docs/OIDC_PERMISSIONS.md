# OIDC Integration & Permissions System

This document describes the OIDC (OpenID Connect) integration and permissions system implemented in the frontend application.

## Overview

The frontend now uses a professional OIDC library (`react-oidc-context` based on `oidc-client-ts`) for authentication and session management, integrated with a robust permission-driven UI system that fetches user permissions from the backend AuthService `/me` endpoint.

## Architecture

### Authentication Flow

1. **OIDC Provider (AuthProvider)**: Wraps the application with authentication context
2. **AxiosInterceptorProvider**: Sets up axios interceptors for token handling and 401 responses
3. **PermissionsProvider**: Fetches and manages user permissions from `/me` endpoint

### OIDC Configuration

Located in: `src/providers/AuthProvider.tsx`

```typescript
{
  authority: import.meta.env.VITE_OIDC_AUTHORITY,
  client_id: "CoursePlatformWeb",
  redirect_uri: window.location.origin,
  post_logout_redirect_uri: window.location.origin,
  response_type: "code",
  scope: "openid profile email",
  automaticSilentRenew: true,
  loadUserInfo: true
}
```

**Environment Variables:**
- `VITE_OIDC_AUTHORITY`: Keycloak Issuer URL (e.g., `http://localhost:8080/realms/course-platform`)
- `VITE_OIDC_CLIENT_ID`: OAuth client ID (set to `CoursePlatformWeb`)

### Axios Integration

**Request Interceptor:**
- Automatically retrieves the `access_token` from OIDC session storage
- Adds it to the `Authorization` header for all API requests

**Response Interceptor:**
- Handles 401 Unauthorized responses
- Triggers OIDC logout and clears application state
- Transforms API errors into consistent format

### Permissions System

The permissions system provides:
- **Context**: `PermissionsContext` for accessing user data and permissions
- **Hooks**: Multiple hooks for checking permissions and roles
- **Components**: Guard components for conditional rendering

## Available Hooks

### useCurrentUser()
Returns the currently authenticated user's data or `null`.

```typescript
const user = useCurrentUser();
// user: { id, email, firstName, lastName, roles, permissions } | null
```

### usePermissions()
Returns the current user's permission list.

```typescript
const permissions = usePermissions();
// permissions: PermissionDto[]
```

### useHasPermission(action, resource, resourceId?)
Checks if the user has a specific permission.

```typescript
const canCreateCourse = useHasPermission("Create", "Course");
const canEditSpecific = useHasPermission("Update", "Course", courseId);
```

### useHasAnyPermission(checks)
Checks if the user has any of the specified permissions.

```typescript
const canManage = useHasAnyPermission([
  ["Create", "Course"],
  ["Update", "Course"],
  ["Delete", "Course"]
]);
```

### useHasAllPermissions(checks)
Checks if the user has all of the specified permissions.

```typescript
const hasFullAccess = useHasAllPermissions([
  ["Create", "Course"],
  ["Delete", "Course"]
]);
```

### useHasRole(roleName)
Checks if the user has a specific role.

```typescript
const isAdmin = useHasRole("Admin");
```

### useHasAnyRole(roleNames)
Checks if the user has any of the specified roles.

```typescript
const isStaff = useHasAnyRole(["Admin", "Instructor"]);
```

## Permission Guard Components

### PermissionGuard
Conditionally renders children based on a single permission.

```typescript
<PermissionGuard 
  action="Create" 
  resource="Course"
  fallback={<div>No permission</div>}
>
  <CreateCourseButton />
</PermissionGuard>
```

### AnyPermissionGuard
Conditionally renders children if user has any of the specified permissions.

```typescript
<AnyPermissionGuard 
  checks={[
    ["Create", "Course"],
    ["Update", "Course"]
  ]}
>
  <CourseManagementPanel />
</AnyPermissionGuard>
```

## Usage Examples

### Protecting UI Elements

```typescript
function CourseActions({ courseId }: { courseId: string }) {
  const canEdit = useHasPermission("Update", "Course", courseId);
  const canDelete = useHasPermission("Delete", "Course", courseId);

  return (
    <div>
      {canEdit && <EditButton />}
      {canDelete && <DeleteButton />}
    </div>
  );
}
```

### Role-Based Navigation

```typescript
function AdminMenu() {
  const isAdmin = useHasRole("Admin");
  
  if (!isAdmin) {
    return null;
  }

  return (
    <nav>
      <Link to="/admin/users">User Management</Link>
      <Link to="/admin/roles">Role Management</Link>
    </nav>
  );
}
```

### Displaying User Information

```typescript
function UserProfile() {
  const currentUser = useCurrentUser();
  
  if (!currentUser) {
    return <div>Please log in</div>;
  }

  return (
    <div>
      <h1>{currentUser.firstName} {currentUser.lastName}</h1>
      <p>{currentUser.email}</p>
      <div>
        Roles: {currentUser.roles.map(r => r.name).join(", ")}
      </div>
    </div>
  );
}
```

## Permission Model

Permissions follow this structure:

```typescript
{
  key: string;           // Unique identifier
  effect: "Allow" | "Deny";
  action: string;        // e.g., "Create", "Read", "Update", "Delete"
  resource: string;      // e.g., "Course", "User", "Enrollment"
  resourceId: string;    // Specific resource ID or "*" for all
}
```

## Authorization Flow

1. User authenticates via Keycloak (OIDC)
2. OIDC tokens are stored in session storage
3. `PermissionsProvider` fetches user data from `/me` endpoint
4. User permissions are cached via React Query
5. Hooks and guards check permissions against cached data
6. UI elements conditionally render based on permissions

## Token Refresh

- **Automatic Silent Refresh**: Enabled by default
- Tokens are automatically renewed in the background
- No user interaction required for token refresh

## Session Expiration Handling

When a 401 Unauthorized response is received:
1. Axios interceptor catches the error
2. Query cache is cleared
3. OIDC user session is removed
4. User is redirected to login (via OIDC)

## Testing

To test the permissions system:

1. Log in as different users with different roles
2. Verify that UI elements show/hide based on permissions
3. Check that API calls include proper Authorization headers
4. Test session expiration by invalidating tokens

## Configuration Checklist

- [x] OIDC Authority URL configured in `.env`
- [x] Client ID set to "CoursePlatformWeb"
- [x] Authorization Code with PKCE flow enabled
- [x] Automatic silent refresh configured
- [x] Axios interceptor integrated with OIDC
- [x] 401 handler triggers logout
- [x] Permissions provider fetches from `/me`
- [x] Permission checking hooks available
- [x] Guard components for conditional rendering

## Files Modified/Created

### Created
- `src/providers/AxiosInterceptorProvider.tsx`
- `src/providers/PermissionsProvider.tsx`
- `src/services/currentUser.service.ts`
- `src/hooks/usePermissions.ts`
- `src/components/common/PermissionGuard.tsx`
- `docs/OIDC_PERMISSIONS.md` (this file)

### Modified
- `src/providers/AuthProvider.tsx`
- `src/api/axiosClient.ts`
- `src/App.tsx`
- `src/hooks/index.ts`
- `src/components/common/ProfileMenu/UserNav.tsx`
- `.env`
