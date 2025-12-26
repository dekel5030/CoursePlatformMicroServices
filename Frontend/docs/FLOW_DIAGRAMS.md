# OIDC & Permissions System - Flow Diagrams

This document provides visual representations of the authentication and permissions flow.

## Authentication Flow

```mermaid
sequenceDiagram
    participant User
    participant Frontend
    participant OIDC Provider
    participant Backend API
    
    User->>Frontend: Click "Login"
    Frontend->>OIDC Provider: Redirect to /auth?client_id=CoursePlatformWeb
    OIDC Provider->>User: Show login form
    User->>OIDC Provider: Enter credentials
    OIDC Provider->>Frontend: Redirect with auth code
    Frontend->>OIDC Provider: Exchange code for tokens (PKCE)
    OIDC Provider->>Frontend: Return access_token, refresh_token, id_token
    Frontend->>Frontend: Store tokens in sessionStorage
    Frontend->>Backend API: GET /me (with Bearer token)
    Backend API->>Frontend: Return user + permissions
    Frontend->>Frontend: Cache in React Query
    Frontend->>User: Show authenticated UI
```

## Permission Check Flow

```mermaid
flowchart TD
    A[Component Renders] --> B{User Authenticated?}
    B -->|No| C[Return false/null]
    B -->|Yes| D{Permissions Loaded?}
    D -->|No| E[Show Loading]
    D -->|Yes| F[Check Permission]
    F --> G{Has Permission?}
    G -->|Yes| H[Render Content]
    G -->|No| I[Render Fallback]
```

## 401 Error Handling Flow

```mermaid
sequenceDiagram
    participant Component
    participant Axios
    participant Interceptor
    participant OIDC
    participant QueryCache
    
    Component->>Axios: API Request
    Axios->>Interceptor: Add Bearer token
    Axios->>Backend: Request with token
    Backend->>Axios: 401 Unauthorized
    Axios->>Interceptor: Response Error
    Interceptor->>QueryCache: Clear all queries
    Interceptor->>OIDC: removeUser()
    OIDC->>SessionStorage: Clear tokens
    OIDC->>User: Redirect to login
```

## Component Hierarchy

```mermaid
graph TD
    A[main.tsx] --> B[QueryClientProvider]
    B --> C[App.tsx]
    C --> D[BrowserRouter]
    D --> E[AuthProvider OIDC]
    E --> F[AxiosInterceptorProvider]
    F --> G[PermissionsProvider]
    G --> H[Layout]
    H --> I[Routes]
    
    style E fill:#e1f5ff
    style F fill:#fff4e1
    style G fill:#e1ffe1
```

## Data Flow

```mermaid
flowchart LR
    A[OIDC Login] --> B[Access Token]
    B --> C[Axios Interceptor]
    C --> D[API Request]
    D --> E[/me Endpoint]
    E --> F[User + Permissions]
    F --> G[React Query Cache]
    G --> H[PermissionsContext]
    H --> I[Permission Hooks]
    I --> J[Components]
    
    style A fill:#ffebee
    style E fill:#e3f2fd
    style G fill:#f3e5f5
    style I fill:#e8f5e9
```

## Permission Checking Logic

```mermaid
flowchart TD
    A[useHasPermission action, resource, resourceId] --> B[Get permissions from context]
    B --> C{Find matching permission}
    C --> D{Action matches?}
    D -->|No| E[Continue searching]
    D -->|Yes| F{Resource matches?}
    F -->|No| E
    F -->|Yes| G{ResourceId matches or is '*'?}
    G -->|No| E
    G -->|Yes| H{Effect is 'Allow'?}
    H -->|Yes| I[Return true]
    H -->|No| E
    E --> J{More permissions?}
    J -->|Yes| C
    J -->|No| K[Return false]
```

## State Management

```mermaid
stateDiagram-v2
    [*] --> Unauthenticated
    Unauthenticated --> Authenticating: Login clicked
    Authenticating --> Authenticated: OIDC success
    Authenticated --> LoadingPermissions: Fetch /me
    LoadingPermissions --> PermissionsLoaded: Success
    LoadingPermissions --> Error: API Error
    Error --> LoadingPermissions: Retry
    PermissionsLoaded --> Refreshing: Token refresh
    Refreshing --> PermissionsLoaded: Success
    PermissionsLoaded --> Unauthenticated: Logout / 401
    Authenticated --> Unauthenticated: Logout / 401
```

## Token Lifecycle

```mermaid
gantt
    title Token Lifecycle
    dateFormat mm:ss
    section Token
    Valid Token         :active, 00:00, 14:00
    Refresh Window      :crit, 13:00, 15:00
    Silent Refresh      :milestone, 14:00, 0s
    New Token Valid     :active, 14:00, 28:00
    section User Activity
    User Active         :00:00, 28:00
    No Interruption     :00:00, 28:00
```

## Error Handling

```mermaid
flowchart TD
    A[API Call] --> B{Response}
    B -->|200-299| C[Success]
    B -->|401| D[Unauthorized]
    B -->|403| E[Forbidden]
    B -->|Other| F[Generic Error]
    
    D --> G[Clear Query Cache]
    G --> H[Remove OIDC User]
    H --> I[Redirect to Login]
    
    E --> J[Show Error Message]
    F --> J
    
    style D fill:#ffcdd2
    style G fill:#fff9c4
    style I fill:#c5e1a5
```

## Permission Guard Component Flow

```mermaid
flowchart TD
    A[PermissionGuard] --> B[useHasPermission]
    B --> C{Has Permission?}
    C -->|Yes| D[Render children]
    C -->|No| E{Fallback provided?}
    E -->|Yes| F[Render fallback]
    E -->|No| G[Render null]
    
    style C fill:#e1f5fe
    style D fill:#c8e6c9
    style F fill:#fff9c4
```

## System Integration Overview

```mermaid
graph TB
    subgraph Frontend
        A[React App]
        B[OIDC Provider]
        C[Axios Client]
        D[Permissions Provider]
        E[UI Components]
    end
    
    subgraph External Services
        F[Keycloak]
        G[Backend API]
    end
    
    A --> B
    B --> F
    A --> C
    C --> G
    A --> D
    D --> G
    D --> E
    C --> E
    
    style B fill:#e3f2fd
    style D fill:#f3e5f5
    style F fill:#fff3e0
    style G fill:#e8f5e9
```

## Cache Strategy

```mermaid
flowchart LR
    A[User Authenticates] --> B[Fetch /me]
    B --> C{Data in Cache?}
    C -->|No| D[Call API]
    C -->|Yes| E{Cache Stale?}
    E -->|No| F[Use Cache]
    E -->|Yes| G[Refetch in Background]
    D --> H[Store in Cache 5min]
    G --> H
    H --> I[Serve to Components]
    F --> I
    
    style C fill:#e1f5fe
    style E fill:#fff9c4
    style H fill:#c8e6c9
```

## Usage Patterns

### Pattern 1: Hook-based Permission Check
```typescript
const canEdit = useHasPermission("Update", "Course", courseId);
if (canEdit) {
  // Show edit button
}
```

### Pattern 2: Guard Component
```typescript
<PermissionGuard action="Delete" resource="User">
  <DeleteButton />
</PermissionGuard>
```

### Pattern 3: Multiple Permissions
```typescript
const hasAny = useHasAnyPermission([
  ["Create", "Course"],
  ["Update", "Course"]
]);
```

### Pattern 4: Role-based
```typescript
const isAdmin = useHasRole("Admin");
```

---

## Performance Considerations

- **Caching**: Permissions cached for 5 minutes
- **Memory**: In-memory permission checks (fast)
- **Network**: Single `/me` call per session
- **Refresh**: Silent token refresh (no UI interruption)

## Security Features

- ✅ PKCE flow (prevents code interception)
- ✅ Short-lived access tokens
- ✅ Automatic token rotation
- ✅ Secure session storage
- ✅ 401 auto-logout
- ✅ No token logging
