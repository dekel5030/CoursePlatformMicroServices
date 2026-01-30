---
name: Frontend Architecture Redesign
overview: Restructure the React frontend from entity-based features to a hybrid architecture with clear separation between domain (data/API), features (business logic/UI), and shared code. This will eliminate cross-feature dependencies, reduce duplication, and improve maintainability.
todos:
  - id: create-domain-structure
    content: "Create domain layer structure: courses, lessons, users, auth, iam with api/, mappers/, types/, query-keys.ts subdirectories"
    status: completed
  - id: migrate-courses-domain
    content: "Migrate courses: Extract API functions, mappers, types, and query keys from features/courses to domain/courses"
    status: completed
    dependencies:
      - create-domain-structure
  - id: migrate-lessons-domain
    content: "Migrate lessons: Extract API functions, mappers, types, and query keys from features/lessons to domain/lessons. Remove dependency on coursesQueryKeys"
    status: completed
    dependencies:
      - create-domain-structure
  - id: migrate-other-domains
    content: "Migrate remaining domains: users, auth, iam following the same pattern"
    status: completed
    dependencies:
      - create-domain-structure
  - id: create-feature-structure
    content: "Create new feature folders: course-catalog, course-management, lesson-viewer, user-profile, admin-dashboard"
    status: completed
    dependencies:
      - migrate-courses-domain
      - migrate-lessons-domain
  - id: migrate-course-features
    content: Reorganize course-related components and pages into course-catalog and course-management features
    status: completed
    dependencies:
      - create-feature-structure
  - id: migrate-lesson-features
    content: Reorganize lesson components and pages into lesson-viewer feature, removing courses dependency
    status: completed
    dependencies:
      - create-feature-structure
  - id: migrate-shared-layer
    content: Move components/ui, components/common, utils, and shared hooks to shared/ directory. Extract entity-specific constants to domains
    status: completed
  - id: migrate-app-layer
    content: Move providers, routes, and layout components to app/ directory
    status: completed
  - id: update-imports
    content: Update all import statements across the codebase to use new paths. Update tsconfig.json aliases if needed
    status: completed
    dependencies:
      - migrate-course-features
      - migrate-lesson-features
      - migrate-shared-layer
      - migrate-app-layer
  - id: cleanup-old-structure
    content: Remove old feature folders and verify no circular dependencies exist
    status: completed
    dependencies:
      - update-imports
  - id: verify-build
    content: Run build and verify all imports resolve correctly. Fix any remaining issues
    status: completed
    dependencies:
      - cleanup-old-structure
---

# Frontend Architecture Redesign

## Current Problems Identified

1. **Entity vs Feature Confusion**: Features are organized by database entities (courses, lessons, users) rather than business capabilities
2. **Cross-Feature Dependencies**: `lessons` feature imports from `courses` (e.g., `lessonsQueryKeys` depends on `coursesQueryKeys`)
3. **Logic Leakage**: API calls, mappers, and types are mixed within feature folders
4. **Duplication**: Similar patterns repeated across features (API structure, hooks, types)
5. **Unclear Boundaries**: No clear separation between domain logic, business features, and shared utilities

## Proposed Architecture

### New Directory Structure

```
src/
├── app/                    # Application-level concerns
│   ├── routes/            # Route definitions
│   ├── providers/         # Global providers (Auth, Query, etc.)
│   ├── layouts/           # Layout components
│   └── axios/             # HTTP client configuration
│
├── domain/                # Domain layer - Pure business entities & data access
│   ├── courses/
│   │   ├── api/          # API client functions
│   │   ├── mappers/      # DTO → Model transformations
│   │   ├── types/        # Domain types & models
│   │   └── query-keys.ts # React Query key factories
│   ├── lessons/
│   ├── users/
│   ├── auth/
│   └── iam/
│
├── features/              # Business features (not entities)
│   ├── course-catalog/   # Browse & discover courses
│   │   ├── components/
│   │   ├── hooks/        # Feature-specific hooks (compose domain hooks)
│   │   └── pages/
│   ├── course-management/ # Create/edit/manage courses
│   ├── lesson-viewer/     # View & interact with lessons
│   ├── user-profile/      # User profile management
│   └── admin-dashboard/   # Admin IAM features
│
├── shared/                # Truly shared code
│   ├── ui/               # Design system (shadcn components)
│   ├── common/           # Reusable business components
│   ├── utils/            # Pure utility functions
│   ├── hooks/            # Shared React hooks
│   └── types/            # Shared TypeScript types
│
└── assets/               # Static assets
```

## Key Architectural Principles

### 1. Domain Layer (`src/domain/`)

**Purpose**: Pure data access and business entities. No UI dependencies.

**Structure per domain**:

- `api/` - API client functions (fetch, create, update, delete)
- `mappers/` - DTO to Model transformations (decouple from backend schema)
- `types/` - Domain models and DTOs
- `query-keys.ts` - React Query key factories (centralized, no cross-domain deps)

**Rules**:

- ✅ Can import from `shared/utils`, `shared/types`
- ❌ Cannot import from other domains
- ❌ Cannot import from `features/` or `app/`
- ❌ No React components

**Example**: `src/domain/courses/api/courses-api.ts` contains all course API calls, `src/domain/courses/mappers/course-mapper.ts` handles DTO→Model mapping.

### 2. Features Layer (`src/features/`)

**Purpose**: Business capabilities that compose domain logic into user-facing features.

**Structure per feature**:

- `components/` - Feature-specific UI components
- `hooks/` - Feature hooks that compose domain hooks (e.g., `useCourseCatalog` uses `useCourses` from domain)
- `pages/` - Route pages
- `constants.ts` - Feature constants

**Rules**:

- ✅ Can import from `domain/` (multiple domains if needed)
- ✅ Can import from `shared/`
- ✅ Can import from `app/` (routes, layouts)
- ❌ Cannot import from other features directly
- ❌ No API calls or mappers (use domain layer)

**Example**: `course-catalog` feature uses `domain/courses` and `domain/auth` to build the catalog UI.

### 3. Shared Layer (`src/shared/`)

**Purpose**: Reusable code with no business logic dependencies.

**Structure**:

- `ui/` - Design system components (shadcn)
- `common/` - Business-agnostic reusable components (SearchBox, ProfileMenu, etc.)
- `utils/` - Pure functions (linkHelpers, animations, etc.)
- `hooks/` - Shared React hooks (not domain-specific)
- `types/` - Shared TypeScript types (LinkDto, etc.)

**Rules**:

- ✅ Can only import from `shared/` (internal)
- ❌ Cannot import from `domain/`, `features/`, or `app/`

### 4. App Layer (`src/app/`)

**Purpose**: Application configuration and global concerns.

**Structure**:

- `routes/` - Route definitions
- `providers/` - Global providers (AuthProvider, QueryClientProvider, etc.)
- `layouts/` - Layout components
- `axios/` - HTTP client setup

**Rules**:

- ✅ Can import from `domain/`, `features/`, `shared/`
- ✅ Orchestrates the application

## Migration Strategy

### Phase 1: Create Domain Layer

1. Extract API functions from `features/*/api/` → `domain/*/api/`
2. Extract mappers from API files → `domain/*/mappers/`
3. Extract types from `features/*/types/` → `domain/*/types/`
4. Create `domain/*/query-keys.ts` for each domain
5. Update domain hooks to use new structure

### Phase 2: Reorganize Features

1. Identify true business features:

   - `course-catalog` (from `courses` pages: Catalog, AllCourses)
   - `course-management` (from `courses` pages: CoursePage with edit)
   - `lesson-viewer` (from `lessons`)
   - `user-profile` (from `users`)
   - `admin-dashboard` (from `iam-dashboard`)

2. Move feature-specific components to new feature folders
3. Create feature hooks that compose domain hooks
4. Update feature pages

### Phase 3: Organize Shared Code

1. Move `components/ui/` → `shared/ui/`
2. Move `components/common/` → `shared/common/`
3. Move `utils/` → `shared/utils/`
4. Extract entity-specific constants from `utils/linkHelpers.ts` to domain layers
5. Move shared hooks to `shared/hooks/`

### Phase 4: Update App Layer

1. Move `providers/` → `app/providers/`
2. Move `routes/` → `app/routes/`
3. Move layout components → `app/layouts/`
4. Keep `axios/` in `app/axios/` or move to `shared/` if truly shared

### Phase 5: Update Imports & Cleanup

1. Update all import paths across the codebase
2. Remove old feature folders
3. Update `tsconfig.json` path aliases if needed
4. Verify no circular dependencies

## Specific File Migrations

### Domain Layer Examples

**`src/domain/courses/api/courses-api.ts`** (from `src/features/courses/api/CoursesAPI.ts`)

- Pure API functions
- No mapping logic (moved to mappers/)

**`src/domain/courses/mappers/course-mapper.ts`** (new, extracted from CoursesAPI.ts)

- `mapCourseDetailsToModel()`, `mapCourseSummaryToModel()`, etc.

**`src/domain/courses/query-keys.ts`** (from `src/features/courses/hooks/use-courses.ts`)

- `coursesQueryKeys` factory
- No cross-domain dependencies

**`src/domain/courses/hooks/use-courses.ts`** (refactored)

- Uses domain API and mappers
- Exports hooks: `useCourses()`, `useCourse()`, `useCreateCourse()`, etc.

### Feature Layer Examples

**`src/features/course-catalog/`** (new feature)

- Components: `Catalog.tsx`, `CourseGrid.tsx`, `CourseCard.tsx`
- Pages: `CourseCatalogPage.tsx`, `AllCoursesPage.tsx`
- Hooks: `useCourseCatalog()` (composes `useCourses()` from domain)

**`src/features/course-management/`** (new feature)

- Components: `CourseEditor.tsx`, `AddCourseDialog.tsx`, `ModuleCard.tsx`
- Pages: `CoursePage.tsx` (management view)
- Hooks: `useCourseManagement()` (composes domain hooks)

**`src/features/lesson-viewer/`** (from `features/lessons`)

- Components: `LessonViewer.tsx`, `LessonVideoPlayer.tsx`
- Pages: `LessonPage.tsx`
- Hooks: `useLessonViewer()` (uses `domain/lessons` only, no courses dependency)

### Shared Layer Examples

**`src/shared/utils/link-helpers.ts`** (from `src/utils/linkHelpers.ts`)

- Generic `hasLink()`, `getLink()` functions
- Remove `CourseRels`, `LessonRels` (move to respective domains)

**`src/domain/courses/constants.ts`** (new)

- `CourseRels` constants

**`src/domain/lessons/constants.ts`** (new)

- `LessonRels` constants

## Dependency Flow

```mermaid
graph TD
    App[app/] --> Features[features/]
    App --> Domain[domain/]
    App --> Shared[shared/]
    Features --> Domain
    Features --> Shared
    Domain --> Shared
    Features -.x.-> Features
    Domain -.x.-> Domain
```

**Import Rules**:

- `app/` → can import from anywhere
- `features/` → can import from `domain/` and `shared/`, not other features
- `domain/` → can import from `shared/` only, not other domains
- `shared/` → can only import from `shared/` (internal)

## Benefits

1. **Clear Boundaries**: Domain logic separated from UI features
2. **No Cross-Dependencies**: Domains are independent; features compose domains
3. **Reusability**: Domain logic can be used by multiple features
4. **Maintainability**: Easy to find where code lives (domain = data, feature = UI)
5. **Testability**: Domain layer is easily testable without UI
6. **Scalability**: Adding new features doesn't require touching domain code

## Breaking Changes

- All import paths will change
- Feature exports will be reorganized
- Some components may move between layers
- Query keys structure may change (but functionality preserved)

## Implementation Notes

- Use TypeScript path aliases: `@/domain/*`, `@/features/*`, `@/shared/*`, `@/app/*`
- Maintain backward compatibility during migration by creating re-export files if needed
- Update ESLint rules to enforce layer boundaries
- Consider adding a custom ESLint rule to prevent cross-layer violations