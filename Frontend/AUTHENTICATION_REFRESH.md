# Automatic Access Token Refresh Implementation

## Overview
This implementation provides a robust, reusable mechanism for handling access token expiration in the React/TypeScript frontend. It automatically intercepts 401 Unauthorized responses, refreshes the access token, and retries the original request - all without requiring explicit token expiry checks.

## Architecture

### Core Components

#### 1. **Authenticated Fetch Wrapper** (`src/utils/authenticatedFetch.ts`)
The heart of the implementation. This module provides:
- **401 Interception**: Automatically detects when an access token has expired
- **Token Refresh**: Calls the refresh token endpoint to get a new access token
- **Request Retry**: Retries the original request once with the new token
- **Single-Flight Refresh**: Ensures concurrent 401s don't trigger multiple refresh attempts
- **Failure Handling**: Automatically logs out the user if token refresh fails

#### 2. **AuthContext Integration** (`src/context/AuthContext.tsx`)
The AuthProvider initializes the authenticated fetch wrapper on mount with:
- Access token getter
- Token refresh function
- Logout function

#### 3. **Service Layer** (`src/services/*.ts`)
All API service files (UsersAPI, CoursesAPI) now use the authenticated fetch wrapper instead of the native `fetch` API.

## How It Works

### Flow Diagram
```
User Request → authFetch() → Add Bearer Token → API Call
                                                   ↓
                                              401 Response?
                                                   ↓
                                    Yes ←──────────┴────────→ No
                                    ↓                         ↓
                            Refresh Token                Return Response
                            (single-flight)
                                    ↓
                              Success?
                                    ↓
                     Yes ←──────────┴────────→ No
                     ↓                         ↓
            Retry with New Token          Logout User
                     ↓                    (Session Expired)
               Return Response
```

### Single-Flight Refresh Pattern
When multiple concurrent requests receive 401 responses:
1. First request triggers a token refresh
2. A promise for this refresh is stored globally
3. Subsequent 401s wait for the same promise
4. All requests retry with the new token once it's available

This prevents the "thundering herd" problem where multiple requests could trigger multiple refresh attempts.

## Usage

### In Service Files
```typescript
import { getAuthenticatedFetch } from "../utils/authenticatedFetch";

export async function fetchUserById(id: string): Promise<User> {
  const authFetch = getAuthenticatedFetch();
  const response = await authFetch(`/api/users/${id}`, {
    credentials: "include",
  });
  
  if (!response.ok) throw new Error("Failed to fetch user");
  return await response.json();
}
```

### Direct Usage in Components (if needed)
```typescript
import { useAuth } from "../context/AuthContext";
import { createAuthenticatedFetch } from "../utils/authenticatedFetch";

function MyComponent() {
  const { currentUser, refreshAccessToken, logout } = useAuth();
  
  const authFetch = createAuthenticatedFetch(
    () => currentUser?.accessToken,
    refreshAccessToken,
    logout
  );
  
  // Use authFetch like normal fetch...
}
```

## Features & Benefits

### ✅ Automatic Token Refresh
No need to manually check token expiry or handle refresh logic in components.

### ✅ Transparent to Components
Components continue to use service functions normally - the refresh logic is completely transparent.

### ✅ Prevents Race Conditions
The single-flight pattern ensures only one refresh happens at a time, even with concurrent requests.

### ✅ Graceful Failure Handling
If refresh fails (e.g., refresh token expired), the user is automatically logged out.

### ✅ Type-Safe
Full TypeScript support with proper typing for all functions.

### ✅ No Backend Changes Required
This is a purely frontend solution that works with the existing backend API.

## Technical Details

### Token Storage
- Access tokens are stored in AuthContext state and localStorage
- Refresh tokens are sent via HTTP-only cookies (handled by the backend)

### Authorization Header
The wrapper automatically adds the `Authorization: Bearer <token>` header to all requests.

### Error Handling
```typescript
try {
  const response = await authFetch('/api/endpoint', { ... });
  // Handle successful response
} catch (error) {
  // This could be:
  // 1. Network error
  // 2. Session expired (after failed refresh)
  // 3. API error (non-401)
}
```

### Concurrency Safety
The implementation uses a global promise to track ongoing refresh attempts:
```typescript
let refreshPromise: Promise<string> | null = null;
```

When a refresh is needed:
- If `refreshPromise` is null, start a new refresh
- If `refreshPromise` exists, wait for it instead of starting a new one
- Clear `refreshPromise` when done (success or failure)

## Testing Scenarios

### Scenario 1: Normal Request
1. User makes a request with a valid access token
2. Request succeeds with 200 OK
3. Response is returned to the caller

### Scenario 2: Expired Access Token
1. User makes a request with an expired access token
2. API returns 401 Unauthorized
3. Wrapper calls refresh token endpoint
4. New access token is received and stored
5. Original request is retried with new token
6. Response is returned to the caller

### Scenario 3: Expired Refresh Token
1. User makes a request with an expired access token
2. API returns 401 Unauthorized
3. Wrapper calls refresh token endpoint
4. Refresh fails (refresh token also expired)
5. User is automatically logged out
6. Error is thrown to the caller

### Scenario 4: Concurrent Requests with Expired Token
1. Multiple requests are made simultaneously
2. All receive 401 Unauthorized
3. First request triggers token refresh
4. Other requests wait for the same refresh promise
5. All requests retry with the new token
6. All responses are returned to their respective callers

## Maintenance Notes

### Adding New API Services
When creating new API service files:
1. Import `getAuthenticatedFetch` from `utils/authenticatedFetch`
2. Use `const authFetch = getAuthenticatedFetch()` at the start of each function
3. Replace `fetch` calls with `authFetch`

### Modifying AuthContext
The AuthContext initialization of the wrapper is crucial. If modifying AuthContext:
- Ensure `initializeAuthenticatedFetch` is called on mount
- Use `useCallback` for `logout` and `refreshAccessToken` to maintain stable references
- Include all dependencies in the `useEffect` dependency array

## Security Considerations

### Token Storage
- Access tokens are stored in localStorage (accessible by JavaScript)
- Refresh tokens should be HTTP-only cookies (not accessible by JavaScript)
- This is a security best practice to minimize the impact of XSS attacks

### Automatic Logout
- On refresh failure, the user is immediately logged out
- This prevents unauthorized access with expired credentials

### Single Refresh
- The single-flight pattern prevents refresh token abuse
- Multiple concurrent requests won't trigger multiple refresh attempts

## Compatibility
- Works with React 19.x
- TypeScript 5.9+
- All modern browsers with Fetch API support
- Compatible with existing AuthContext and service layer

## Future Enhancements (Optional)
- Add request queuing during token refresh
- Add exponential backoff for failed refresh attempts
- Add telemetry/logging for monitoring refresh events
- Add request deduplication for identical concurrent requests
