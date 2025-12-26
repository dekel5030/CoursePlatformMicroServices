# Testing OIDC Integration & Permissions System

This guide provides instructions for testing the OIDC integration and permissions system.

## Prerequisites

1. **Keycloak Server**: Ensure Keycloak is running at the configured authority URL
2. **Backend Services**: AuthService must be running and accessible
3. **Client Configuration**: Ensure `CoursePlatformWeb` client is configured in Keycloak

## Environment Configuration

Verify the `.env` file contains:

```env
VITE_OIDC_AUTHORITY=http://localhost:8080/realms/course-platform
VITE_OIDC_CLIENT_ID=CoursePlatformWeb
VITE_API_BASE_URL=/api
```

## Running the Application

```bash
cd Frontend
npm install
npm run dev
```

The application will start at `http://localhost:5173`

## Manual Testing Scenarios

### 1. Authentication Flow

**Test OIDC Login:**
1. Navigate to `http://localhost:5173`
2. Click "Log in" button in the navbar
3. Should redirect to Keycloak login page
4. Enter credentials and submit
5. Should redirect back to the application
6. User should now be authenticated

**Expected Results:**
- ✅ Redirect to Keycloak works
- ✅ After login, redirected back to app
- ✅ Navbar shows user profile menu
- ✅ No console errors

### 2. Token Handling

**Test Token in API Calls:**
1. Open browser DevTools → Network tab
2. While authenticated, navigate to any page that makes API calls
3. Inspect the request headers
4. Look for `Authorization: Bearer <token>`

**Expected Results:**
- ✅ All API requests include Authorization header
- ✅ Token is valid JWT format
- ✅ No 401 errors for authenticated requests

### 3. Permissions Loading

**Test Permissions Provider:**
1. Log in to the application
2. Navigate to `/examples/permissions`
3. View the permissions example page

**Expected Results:**
- ✅ User information is displayed
- ✅ Roles are listed
- ✅ Permissions count is shown
- ✅ Permission checks work correctly
- ✅ Guard components render based on permissions

### 4. Session Expiration (401 Handling)

**Test Automatic Logout on 401:**
1. Log in to the application
2. In browser DevTools console, run:
   ```javascript
   // Simulate token expiration by clearing storage
   sessionStorage.clear();
   ```
3. Make any API request (navigate to a page that loads data)
4. Should automatically trigger logout

**Expected Results:**
- ✅ 401 error triggers logout
- ✅ Query cache is cleared
- ✅ User is logged out
- ✅ Can log in again

### 5. Permission-Based UI

**Test Permission Guards:**
1. Log in with a user that has specific permissions
2. Navigate to `/examples/permissions`
3. Observe which sections are visible based on permissions

**Test Different Users:**
- Admin user: Should see all admin features
- Regular user: Should see limited features
- User without specific permission: Should see fallback content

**Expected Results:**
- ✅ UI elements show/hide based on permissions
- ✅ Guard components work correctly
- ✅ No errors in console

### 6. Token Refresh

**Test Silent Token Renewal:**
1. Log in to the application
2. Keep the browser tab open for several minutes
3. The token should automatically refresh in the background
4. No user interaction should be required

**Expected Results:**
- ✅ Token refreshes automatically
- ✅ No interruption to user experience
- ✅ API calls continue to work

## Testing with Different Users

Create test users in Keycloak with different roles/permissions:

### Test User 1: Admin
- **Username**: `admin@test.com`
- **Roles**: Admin
- **Permissions**: All permissions (Create, Read, Update, Delete on all resources)

### Test User 2: Instructor
- **Username**: `instructor@test.com`
- **Roles**: Instructor
- **Permissions**: Create, Read, Update on Course resources

### Test User 3: Student
- **Username**: `student@test.com`
- **Roles**: Student
- **Permissions**: Read on Course resources

## API Endpoint Testing

### Test `/me` Endpoint

Using browser console while authenticated:

```javascript
fetch('/api/me', {
  headers: {
    'Authorization': `Bearer ${sessionStorage.getItem('oidc.user:' + import.meta.env.VITE_OIDC_AUTHORITY + ':' + import.meta.env.VITE_OIDC_CLIENT_ID)}`
  }
})
  .then(r => r.json())
  .then(console.log);
```

**Expected Response:**
```json
{
  "id": "guid",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "roles": [
    { "id": "guid", "name": "Admin" }
  ],
  "permissions": [
    {
      "key": "permission-key",
      "effect": "Allow",
      "action": "Create",
      "resource": "Course",
      "resourceId": "*"
    }
  ]
}
```

## Common Issues and Solutions

### Issue: OIDC Configuration Error

**Symptoms:** "OIDC configuration error" in console

**Solutions:**
1. Check `.env` file has correct `VITE_OIDC_AUTHORITY` and `VITE_OIDC_CLIENT_ID`
2. Verify Keycloak is running
3. Confirm client exists in Keycloak realm

### Issue: Redirect Loop

**Symptoms:** Continuous redirects between app and Keycloak

**Solutions:**
1. Check `redirect_uri` matches the application origin
2. Verify client configuration in Keycloak allows the redirect URI
3. Clear browser storage and try again

### Issue: 401 Errors on API Calls

**Symptoms:** API calls return 401 even when logged in

**Solutions:**
1. Check if token is being added to request headers
2. Verify token is valid (not expired)
3. Check backend AuthService is configured correctly
4. Verify JWT signature validation settings

### Issue: Permissions Not Loading

**Symptoms:** Permissions example shows empty permissions

**Solutions:**
1. Check if `/me` endpoint is accessible
2. Verify user has roles/permissions assigned in backend
3. Check browser network tab for errors
4. Verify PermissionsProvider is in component tree

## Debug Mode

To enable verbose logging:

1. Open browser DevTools console
2. Run: `localStorage.setItem('debug', 'oidc:*')`
3. Refresh the page
4. OIDC events will be logged to console

To disable:
```javascript
localStorage.removeItem('debug');
```

## Automated Test Scenarios (Future)

Future integration tests should cover:

- [ ] OIDC authentication flow
- [ ] Token refresh mechanism
- [ ] 401 handling and logout
- [ ] Permissions loading from `/me`
- [ ] Permission hooks return correct values
- [ ] Guard components render conditionally
- [ ] Role-based access control

## Performance Considerations

- Permissions are cached via React Query (5-minute stale time)
- Token refresh happens automatically in background
- No need to refetch permissions on every navigation

## Security Checklist

- [x] Tokens stored in secure session storage
- [x] Authorization header added to all API requests
- [x] 401 responses trigger automatic logout
- [x] PKCE enabled for authorization code flow
- [x] Post-logout redirect configured
- [x] No tokens exposed in console logs (in production)
