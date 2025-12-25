# Data Fetching Architecture

This document describes the React Query + Axios data fetching architecture implemented in this frontend application.

## Overview

We use **TanStack Query v5** (React Query) combined with **Axios** to provide a robust, type-safe, and maintainable data fetching layer.

## Architecture Layers

### 1. API Client (`src/api/axiosClient.ts`)

The Axios client is configured with:
- **Base URL**: Automatically configured from environment variables or uses `/api` proxy
- **Request Interceptor**: Automatically injects JWT tokens from OIDC session storage
- **Response Interceptor**: Handles global errors (401, 403, 404, 500, etc.)

```typescript
import { axiosClient } from '@/api/axiosClient';

// The client automatically handles:
// - Bearer token injection
// - Content-Type headers
// - Global error handling
```

### 2. Service Layer (`src/services/`)

Services are **stateless TypeScript functions** that use the `axiosClient`. They contain **no React hooks**.

**Key Principles:**
- ✅ Pure TypeScript functions
- ✅ Strongly typed with interfaces from `src/types/`
- ✅ Use `axiosClient` for all HTTP requests
- ❌ No React hooks
- ❌ No state management

**Example:**
```typescript
// src/services/CoursesAPI.ts
import { axiosClient } from '@/api/axiosClient';
import type { Course } from '@/types';

export async function fetchCourseById(id: string): Promise<Course> {
  const response = await axiosClient.get<Course>(`/courses/${id}`);
  return response.data;
}
```

### 3. Feature Hooks (`src/features/[feature]/hooks/`)

React Query hooks are **co-located with their respective features**. These hooks:
- Wrap TanStack Query's `useQuery` and `useMutation`
- Provide type-safe data fetching for components
- Handle loading, error, and data states automatically

**Example:**
```typescript
// src/features/courses/hooks/useCourse.ts
import { useQuery } from '@tanstack/react-query';
import { fetchCourseById } from '@/services/CoursesAPI';

export function useCourse(id: string | undefined) {
  return useQuery({
    queryKey: ['courses', id],
    queryFn: () => fetchCourseById(id!),
    enabled: !!id,
  });
}
```

### 4. Components

Components consume the feature hooks and destructure the query state:

```typescript
import { useCourse } from '@/features/courses';

export default function CoursePage() {
  const { id } = useParams();
  const { data: course, isLoading, error } = useCourse(id);

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;
  if (!course) return <div>Not found</div>;

  return <div>{course.title}</div>;
}
```

## Query Configuration

The QueryClient is configured in `src/main.tsx` with sensible defaults:

```typescript
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000, // 5 minutes
      gcTime: 10 * 60 * 1000,   // 10 minutes (garbage collection)
      retry: 1,
      refetchOnWindowFocus: false,
    },
  },
});
```

## Available Hooks

### Courses
- `useCourses()` - Fetch featured courses
- `useCourse(id)` - Fetch a single course by ID

### Lessons
- `useLesson(id)` - Fetch a single lesson by ID

### Users
- `useUser(id)` - Fetch user profile by ID
- `useUpdateUser(userId)` - Update user profile (mutation)

## Benefits

### 1. Automatic Request Deduplication
React Query automatically deduplicates requests with the same query key, even in `<StrictMode>`.

### 2. Built-in Caching
Data is cached for 5 minutes (configurable), reducing unnecessary API calls.

### 3. Loading & Error States
No need for manual `useState` for loading/error - React Query provides these automatically.

### 4. Type Safety
Full TypeScript support from API layer to components.

### 5. Automatic Retries
Failed requests are automatically retried once.

### 6. Background Refetching
Data can be automatically refetched in the background to keep UI fresh.

### 7. Optimistic Updates
Mutations can optimistically update the UI before server confirmation.

## Development Tools

React Query DevTools are included in development mode:

```tsx
<ReactQueryDevtools initialIsOpen={false} />
```

Access the DevTools by clicking the React Query icon in the bottom-left corner of the browser.

## Authentication

JWT tokens are automatically injected by the Axios interceptor. The interceptor:
1. Reads OIDC user data from `sessionStorage`
2. Extracts the `access_token`
3. Adds `Authorization: Bearer <token>` header to all requests

No manual token management is needed in services or components.

## Error Handling

Errors are handled at multiple levels:

1. **Global Level**: Response interceptor logs errors to console
2. **Hook Level**: React Query captures errors in the `error` state
3. **Component Level**: Components can display error messages from `error.message`

## Migration Checklist

When adding a new feature that requires data fetching:

- [ ] Add service functions to `src/services/` (use `axiosClient`, no hooks)
- [ ] Create custom hooks in `src/features/[feature]/hooks/`
- [ ] Export hooks from `src/features/[feature]/index.ts`
- [ ] Use hooks in components
- [ ] Verify TypeScript types are correct
- [ ] Test in development with React Query DevTools

## Anti-Patterns to Avoid

❌ **DO NOT** use `useEffect` for data fetching
❌ **DO NOT** use React hooks in service files
❌ **DO NOT** manually manage loading/error states
❌ **DO NOT** pass `fetch` or `axios` as function parameters
❌ **DO NOT** manually inject JWT tokens in service calls

## Further Reading

- [TanStack Query Documentation](https://tanstack.com/query/latest)
- [Axios Documentation](https://axios-http.com/)
- [React Query Best Practices](https://tkdodo.eu/blog/practical-react-query)
