# Auth Management Redesign - API Reference

## New Endpoints

### GET /users

**Description**: Fetches all users in the system for admin management

**Authentication**: Required (Admin role)

**Request**:
```http
GET /users HTTP/1.1
Host: localhost:5433
Authorization: Bearer {token}
```

**Response** (200 OK):
```json
[
  {
    "id": "01234567-89ab-cdef-0123-456789abcdef",
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "roles": [
      {
        "id": "01234567-89ab-cdef-0123-456789abcdef",
        "name": "Admin"
      },
      {
        "id": "01234567-89ab-cdef-0123-456789abcdef",
        "name": "Manager"
      }
    ],
    "permissions": [
      {
        "key": "allow_read_user_*",
        "effect": "Allow",
        "action": "Read",
        "resource": "User",
        "resourceId": "*"
      }
    ]
  },
  {
    "id": "01234567-89ab-cdef-0123-456789abcdef",
    "email": "jane.smith@example.com",
    "firstName": "Jane",
    "lastName": "Smith",
    "roles": [
      {
        "id": "01234567-89ab-cdef-0123-456789abcdef",
        "name": "Instructor"
      }
    ],
    "permissions": []
  }
]
```

**Error Responses**:
- `401 Unauthorized` - Missing or invalid authentication token
- `403 Forbidden` - User does not have admin privileges
- `500 Internal Server Error` - Server error

**Frontend Integration**:
```typescript
// Service
export async function fetchAllUsers(): Promise<UserDto[]> {
  const response = await axiosClient.get<UserDto[]>("admin/users");
  return response.data;
}

// Hook
export function useUsers() {
  return useQuery({
    queryKey: ['auth', 'users'],
    queryFn: () => fetchAllUsers(),
  });
}

// Usage
const { data: users, isLoading, error } = useUsers();
```

---

## Enhanced Endpoints

### GET /admin/roles

**Description**: Fetches all roles (now includes user count)

**Changes**:
- **Added field**: `userCount` - Number of users assigned to the role

**Before**:
```json
[
  {
    "id": "01234567-89ab-cdef-0123-456789abcdef",
    "name": "Admin",
    "permissionCount": 12
  }
]
```

**After**:
```json
[
  {
    "id": "01234567-89ab-cdef-0123-456789abcdef",
    "name": "Admin",
    "permissionCount": 12,
    "userCount": 5
  },
  {
    "id": "01234567-89ab-cdef-0123-456789abcdef",
    "name": "Manager",
    "permissionCount": 8,
    "userCount": 10
  }
]
```

**Impact**:
- ✅ **Backward compatible** - Existing clients can ignore new field
- ✅ **Performance optimized** - Single query calculates all counts
- ✅ **No breaking changes** - All existing fields remain unchanged

**Frontend Type Update**:
```typescript
export type RoleListItemDto = {
  id: string;
  name: string;
  permissionCount: number;
  userCount: number; // NEW FIELD
};
```

---

## Existing Endpoints (Used in New UI)

### GET /admin/users/:userId

**Description**: Fetch single user by ID

**Used By**: UserDrawer component (when opened)

**Response**:
```json
{
  "id": "01234567-89ab-cdef-0123-456789abcdef",
  "email": "john.doe@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "roles": [
    {
      "id": "01234567-89ab-cdef-0123-456789abcdef",
      "name": "Admin"
    }
  ],
  "permissions": [
    {
      "key": "allow_read_user_*",
      "effect": "Allow",
      "action": "Read",
      "resource": "User",
      "resourceId": "*"
    }
  ]
}
```

---

### POST /admin/users/:userId/roles

**Description**: Add role to user

**Used By**: UserDrawer > MultiSelect (on selection)

**Request**:
```json
{
  "roleName": "Manager"
}
```

**Response**: `204 No Content`

**Frontend Integration**:
```typescript
const { addRole } = useUserManagement(userId);
await addRole.mutateAsync({ roleName: "Manager" });
// UI updates automatically via React Query invalidation
```

---

### DELETE /admin/users/:userId/roles/:roleName

**Description**: Remove role from user

**Used By**: UserDrawer > MultiSelect (on deselection)

**Request**: No body

**Response**: `204 No Content`

**Frontend Integration**:
```typescript
const { removeRole } = useUserManagement(userId);
await removeRole.mutateAsync("Manager");
// UI updates automatically via React Query invalidation
```

---

### POST /admin/roles/:roleName/permissions

**Description**: Add permission to role

**Used By**: PermissionMatrix (on toggle enable)

**Request**:
```json
{
  "effect": "Allow",
  "action": "Read",
  "resource": "Course",
  "resourceId": "*"
}
```

**Response**: `204 No Content`

**Frontend Integration**:
```typescript
const { addPermission } = useRoleManagement(roleName);
await addPermission.mutateAsync({
  effect: "Allow",
  action: "Read",
  resource: "Course",
  resourceId: "*"
});
```

---

### DELETE /admin/roles/:roleName/permissions/:permissionKey

**Description**: Remove permission from role

**Used By**: PermissionMatrix (on toggle disable)

**Request**: No body

**Response**: `204 No Content`

**Frontend Integration**:
```typescript
const { removePermission } = useRoleManagement(roleName);
await removePermission.mutateAsync(permissionKey);
```

---

### GET /admin/roles/:roleName

**Description**: Get role details with all permissions

**Used By**: RoleDetail page, PermissionMatrix

**Response**:
```json
{
  "id": "01234567-89ab-cdef-0123-456789abcdef",
  "name": "Admin",
  "permissions": [
    {
      "key": "allow_read_user_*",
      "effect": "Allow",
      "action": "Read",
      "resource": "User",
      "resourceId": "*"
    },
    {
      "key": "allow_write_user_*",
      "effect": "Allow",
      "action": "Write",
      "resource": "User",
      "resourceId": "*"
    }
  ]
}
```

---

## API Flow Examples

### User Role Assignment Flow

1. **User opens UserDrawer**
   - `GET /admin/users/:userId` - Fetch current user data
   - `GET /admin/roles` - Fetch available roles

2. **User selects role in MultiSelect**
   - `POST /admin/users/:userId/roles` - Add role
   - React Query invalidates cache
   - `GET /admin/users/:userId` - Refetch user data (automatic)
   - UI updates with new role badge

3. **User removes role in MultiSelect**
   - `DELETE /admin/users/:userId/roles/:roleName` - Remove role
   - React Query invalidates cache
   - `GET /admin/users/:userId` - Refetch user data (automatic)
   - UI updates, role badge removed

### Permission Management Flow

1. **User opens PermissionMatrix**
   - `GET /admin/roles/:roleName` - Fetch role with permissions
   - Permissions grouped by category in UI

2. **User toggles permission on**
   - `POST /admin/roles/:roleName/permissions` - Add permission
   - Loading indicator shown
   - React Query invalidates cache
   - `GET /admin/roles/:roleName` - Refetch role (automatic)
   - Toggle reflects new state

3. **User toggles permission off**
   - `DELETE /admin/roles/:roleName/permissions/:key` - Remove permission
   - Loading indicator shown
   - React Query invalidates cache
   - `GET /admin/roles/:roleName` - Refetch role (automatic)
   - Toggle reflects new state

---

## Error Handling

### Standard Error Response
```json
{
  "message": "User-friendly error message",
  "errors": {
    "RoleName": ["Role not found"]
  }
}
```

### Frontend Error Display

**Component-level**:
```typescript
try {
  await addRole.mutateAsync({ roleName });
} catch (err) {
  const apiError = err as ApiErrorResponse;
  setError(apiError.message);
  // Displays inline error in UI
}
```

**React Query Integration**:
```typescript
const { error } = useUsers();
if (error) {
  return <ErrorDisplay message={error.message} />;
}
```

---

## Performance Optimization

### Caching Strategy

**React Query Cache Keys**:
```typescript
['auth', 'users']                    // All users list
['auth', 'users', userId]            // Single user detail
['auth', 'roles']                    // All roles list
['auth', 'roles', roleName]          // Single role detail
```

**Invalidation Rules**:
- Adding/removing role → Invalidate `['auth', 'users', userId]`
- Adding/removing permission → Invalidate `['auth', 'roles', roleName]`
- User list changes automatically reflect in table

### Request Batching

**Current**: Individual requests for each mutation
**Future Enhancement**: Batch multiple role changes into single request

### Debouncing

**Search**: 150ms debounce on user table search
**Filter**: Immediate (client-side filtering via TanStack Table)

---

## Security Considerations

### Authentication
- All endpoints require valid JWT token
- Token must include admin permissions

### Authorization
- Role-based access control (RBAC)
- User can only manage resources within their scope
- Permission checks on every request

### Validation
- Role names validated against existing roles
- Permission structure validated
- User ID validated before operations

### Audit Trail
- All mutations logged (server-side)
- Who changed what, when
- Can be extended to show in UI (future)

---

## Migration Guide

### For Frontend Developers

**Old Pattern**:
```typescript
// Manual role input
<input 
  type="text" 
  value={roleName}
  onChange={e => setRoleName(e.target.value)}
/>
<button onClick={handleAddRole}>Add Role</button>
```

**New Pattern**:
```typescript
// MultiSelect with autocomplete
<MultiSelect
  options={roleOptions}
  selected={selectedRoles}
  onChange={handleRoleChange} // Automatic save
/>
```

### For Backend Developers

**New Query Handler**:
```csharp
// Register via DI scanning (automatic)
internal class GetAllUsersQueryHandler 
  : IQueryHandler<GetAllUsersQuery, IReadOnlyList<UserDto>>
{
  // Implementation
}
```

**Updated DTO**:
```csharp
// Add userCount field
public record RoleDto(
  Guid Id, 
  string Name, 
  int PermissionCount,
  int UserCount  // NEW
);
```

---

## Testing

### Backend Tests

**Unit Tests**:
```csharp
[Fact]
public async Task GetAllUsers_ReturnsAllUsers()
{
  // Arrange
  var handler = new GetAllUsersQueryHandler(dbContext);
  
  // Act
  var result = await handler.Handle(
    new GetAllUsersQuery(), 
    CancellationToken.None
  );
  
  // Assert
  result.IsSuccess.Should().BeTrue();
  result.Value.Should().NotBeEmpty();
}
```

**Integration Tests**:
```csharp
[Fact]
public async Task GetAllRoles_IncludesUserCount()
{
  // Arrange
  await SeedRolesAndUsers();
  
  // Act
  var response = await Client.GetAsync("/admin/roles");
  var roles = await response.Content.ReadFromJsonAsync<List<RoleDto>>();
  
  // Assert
  roles.Should().AllSatisfy(r => r.UserCount >= 0);
}
```

### Frontend Tests

**Component Tests**:
```typescript
test('UserTable displays all users', () => {
  const users = [/* mock data */];
  render(<UserTable users={users} onUserSelect={jest.fn()} />);
  
  expect(screen.getByText('John Doe')).toBeInTheDocument();
  expect(screen.getByText('Admin')).toBeInTheDocument();
});
```

**Integration Tests**:
```typescript
test('Adding role updates UI', async () => {
  const { result } = renderHook(() => useUserManagement('user-id'));
  
  await act(async () => {
    await result.current.addRole.mutateAsync({ roleName: 'Admin' });
  });
  
  expect(mockApiCall).toHaveBeenCalledWith(
    expect.objectContaining({
      url: '/admin/users/user-id/roles',
      method: 'POST'
    })
  );
});
```

---

## Monitoring & Observability

### Metrics to Track
- Average response time for `/users` endpoint
- Number of role assignments per day
- Number of permission changes per day
- Error rate for user management operations

### Logging
```csharp
_logger.LogInformation(
  "User {UserId} added role {RoleName} to user {TargetUserId}",
  currentUserId, roleName, targetUserId
);
```

### Frontend Analytics
```typescript
// Track user interactions
analytics.track('role_assigned', {
  userId,
  roleName,
  assignedBy: currentUser.id
});
```

---

## Conclusion

The API changes are minimal, backward-compatible, and focused on enabling the new UI functionality. The addition of the `/users` endpoint and the `userCount` field in role responses provide the necessary data for the enhanced user experience without disrupting existing integrations.
