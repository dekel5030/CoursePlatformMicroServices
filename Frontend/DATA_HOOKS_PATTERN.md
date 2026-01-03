# Data Hooks Pattern

## Overview

This document describes the consolidated data access pattern used across features in this application. Each feature exports a cohesive set of data hooks with centralized query key management.

## Pattern

### Structure

Each feature has a single consolidated hook file (e.g., `use-{feature}.ts`) that exports:
1. **Query Keys**: Centralized, type-safe query key factory
2. **Query Hooks**: React Query hooks for data fetching
3. **Mutation Hooks**: React Query hooks for data modification

### Benefits

- **Single Source of Truth**: Query keys defined once and reused consistently
- **Improved Discoverability**: All data operations for a feature in one place
- **Better Maintainability**: Easier to refactor and update caching logic
- **Type Safety**: Centralized types reduce duplication
- **Reduced Magic Strings**: Query keys are defined as constants

## Example: IAM Dashboard

### File: `features/iam-dashboard/hooks/use-iam.ts`

```typescript
// Centralized Query Keys
export const iamQueryKeys = {
  all: ["auth"] as const,
  users: {
    all: () => [...iamQueryKeys.all, "users"] as const,
    detail: (userId: string) => [...iamQueryKeys.users.all(), userId] as const,
  },
  roles: {
    all: () => [...iamQueryKeys.all, "roles"] as const,
    detail: (roleName: string) => [...iamQueryKeys.roles.all(), roleName] as const,
  },
} as const;

// Query Hooks
export function useUsers() { /* ... */ }
export function useUser(userId: string | undefined) { /* ... */ }
export function useRoles() { /* ... */ }
export function useRole(roleName: string | undefined) { /* ... */ }

// Mutation Hooks
export function useUserManagement(userId: string) { /* ... */ }
export function useRoleManagement(roleName: string) { /* ... */ }
```

### Usage

```typescript
import { useUser, useUserManagement, iamQueryKeys } from "@/features/iam-dashboard";

function UserComponent({ userId }: { userId: string }) {
  // Query
  const { data: user, isLoading } = useUser(userId);
  
  // Mutations
  const { addRole, removeRole } = useUserManagement(userId);
  
  // Access query keys if needed for manual invalidation
  queryClient.invalidateQueries({ queryKey: iamQueryKeys.users.detail(userId) });
}
```

## Features Implementing This Pattern

### IAM Dashboard (`use-iam.ts`)
- **Queries**: `useUsers`, `useUser`, `useRoles`, `useRole`
- **Mutations**: `useUserManagement`, `useRoleManagement`
- **Query Keys**: `iamQueryKeys`

### Courses (`use-courses.ts`)
- **Queries**: `useCourses`, `useCourse`
- **Mutations**: `useCreateCourse`
- **Query Keys**: `coursesQueryKeys`

### Users (`use-users.ts`)
- **Queries**: `useUser`
- **Mutations**: `useUpdateUser`
- **Query Keys**: `usersQueryKeys`

### Lessons (`use-lessons.ts`)
- **Queries**: `useLesson`
- **Mutations**: `useCreateLesson`
- **Query Keys**: `lessonsQueryKeys`

## Creating New Feature Hooks

When creating data hooks for a new feature:

1. **Create consolidated file**: `features/{feature}/hooks/use-{feature}.ts`

2. **Define query keys**:
   ```typescript
   export const {feature}QueryKeys = {
     all: ["{feature}"] as const,
     lists: () => [...{feature}QueryKeys.all, "list"] as const,
     detail: (id: string) => [...{feature}QueryKeys.all, id] as const,
   } as const;
   ```

3. **Create query hooks**: Use the centralized query keys
   ```typescript
   export function use{Feature}(id: string | undefined) {
     return useQuery({
       queryKey: id ? {feature}QueryKeys.detail(id) : ["{feature}", "undefined"],
       queryFn: () => fetch{Feature}(id!),
       enabled: !!id,
     });
   }
   ```

4. **Create mutation hooks**: Invalidate using centralized keys
   ```typescript
   export function useUpdate{Feature}(id: string) {
     const queryClient = useQueryClient();
     return useMutation({
       mutationFn: (data) => update{Feature}(id, data),
       onSuccess: () => {
         queryClient.invalidateQueries({ queryKey: {feature}QueryKeys.detail(id) });
       },
     });
   }
   ```

5. **Export from feature index**:
   ```typescript
   // features/{feature}/index.ts
   export { 
     use{Feature}, 
     useUpdate{Feature}, 
     {feature}QueryKeys 
   } from "./hooks/use-{feature}";
   ```

## Query Key Structure

Query keys follow a hierarchical structure:

```typescript
["feature"]                    // All feature data
["feature", "list"]            // List queries
["feature", "list", filters]   // Filtered lists
["feature", id]                // Detail queries
["feature", id, "related"]     // Related data
```

This structure enables:
- **Selective invalidation**: Invalidate all lists, all details, or specific items
- **Parallel queries**: React Query can efficiently cache and deduplicate
- **Type safety**: TypeScript ensures correct usage

## Best Practices

1. **Always use query keys**: Never use magic strings directly in queries
2. **Export query keys**: Make them available for manual cache operations
3. **Group related operations**: Keep queries and mutations together in the same file
4. **Use meaningful names**: Hook names should describe the data they access
5. **Handle edge cases**: Always handle `undefined` parameters gracefully
6. **Consistent invalidation**: Use the same query keys for queries and invalidation
7. **Document complex queries**: Add comments for non-obvious query logic

## Migration Checklist

When migrating existing hooks to this pattern:

- [ ] Create new consolidated `use-{feature}.ts` file
- [ ] Define centralized query keys
- [ ] Move query hooks and update to use centralized keys
- [ ] Move mutation hooks and update invalidation logic
- [ ] Update all imports in components and pages
- [ ] Delete old individual hook files
- [ ] Update feature `index.ts` exports
- [ ] Run TypeScript build to verify no errors
- [ ] Run linter to check for issues
- [ ] Test functionality to ensure no regressions
