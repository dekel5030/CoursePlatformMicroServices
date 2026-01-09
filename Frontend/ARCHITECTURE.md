# Frontend Architecture: Adapter Pattern Implementation

## Overview

This frontend implements an **Adapter Pattern** to decouple UI components from backend API schemas. This architectural decision prevents "domino effect" failures when backend contracts change.

## Architecture Layers

### 1. Backend DTOs (Data Transfer Objects)

**Location**: `src/features/*/types/*Dto.ts`

These interfaces **exactly mirror** the backend API response structure. They change when the backend API changes.

**Examples**:
- `LessonDetailsDto` - Matches CourseService's LessonDetailsDto
- `CourseDetailsDto` - Matches CourseService's CourseDetailsDto  
- `CurrentUserDto` - Matches AuthService's UserDto

**Naming Convention**: Always suffix with `Dto`

```typescript
// Example: LessonDetailsDto.ts
export interface LessonDetailsDto {
  courseId: string;
  lessonId: string;  // This is what the backend sends
  title: string;
  // ... matches backend exactly
}
```

### 2. UI Models

**Location**: `src/features/*/types/*Model.ts`

These interfaces represent the **stable, internal data structure** consumed by UI components. They **do not change** when the backend API changes.

**Examples**:
- `LessonModel` - Stable lesson interface for UI
- `CourseModel` - Stable course interface for UI
- `UserModel` - Stable user interface for UI

**Naming Convention**: Always suffix with `Model`

```typescript
// Example: LessonModel.ts
export interface LessonModel {
  courseId: string;
  lessonId: string;  // This is what the UI expects
  title: string;
  // ... stable UI contract
}
```

### 3. Adapter/Mapper Layer

**Location**: `src/features/*/api/*API.ts`

These functions transform Backend DTOs into UI Models. This is the **single point of change** when backend schemas evolve.

**Key Function**: `mapTo[Entity]Model(dto: EntityDto): EntityModel`

```typescript
// Example: LessonsAPI.ts
function mapToLessonModel(dto: LessonDetailsDto): LessonModel {
  return {
    courseId: dto.courseId,
    lessonId: dto.lessonId,  // Maps backend field to UI field
    title: dto.title,
    description: dto.description || null,
    thumbnailImage: dto.thumbnailUrl,  // Different field names!
    isPreview: dto.isPreview,
    order: dto.index,  // Backend uses "index", UI uses "order"
    // ... transformation logic
  };
}
```

### 4. React Query Hooks

**Location**: `src/features/*/hooks/use-*.ts`

Hooks call the API layer and return UI Models to components. Components are **unaware** of the DTO layer.

```typescript
// Example: use-lessons.ts
export function useLesson(courseId: string, lessonId: string) {
  return useQuery<LessonModel, Error>({  // Returns LessonModel!
    queryKey: lessonsQueryKeys.detail(courseId, lessonId),
    queryFn: () => fetchLessonById(courseId, lessonId),
    enabled: !!courseId && !!lessonId,
  });
}
```

### 5. UI Components

**Location**: `src/features/*/components/*.tsx` and `src/features/*/pages/*.tsx`

Components **only consume UI Models**. They never import or reference DTOs.

```typescript
// Example: LessonCard.tsx
import type { LessonModel } from "../types/LessonModel";

interface LessonProps {
  lesson: LessonModel;  // Always uses the Model
  index: number;
}

export default function LessonCard({ lesson }: LessonProps) {
  return <div>{lesson.lessonId}</div>;  // Uses stable field name
}
```

## Data Flow Diagram

```
Backend API (CourseService)
    ↓
    ↓ Returns LessonDetailsDto { lessonId, index, thumbnailUrl }
    ↓
[API Layer: LessonsAPI.ts]
    ↓ mapToLessonModel()
    ↓ Transforms to LessonModel { lessonId, order, thumbnailImage }
    ↓
[Hook Layer: use-lessons.ts]
    ↓ Returns LessonModel
    ↓
[UI Components]
    ↓ Consume LessonModel
    ↓ Never know about DTOs
```

## Benefits

### 1. Isolated Changes
Backend schema changes **only affect the mapper function**, not the entire UI.

**Example**: Backend changes `lessonId` → `id`
- **Before**: 5+ files need updates
- **After**: 2 lines in mapper need updates

### 2. Type Safety
TypeScript ensures:
- DTOs match backend contracts
- Mappers correctly transform data
- Components receive correct data types

### 3. Improved Testability
Mappers can be unit tested independently:
```typescript
test('mapToLessonModel transforms DTO correctly', () => {
  const dto: LessonDetailsDto = { /* backend shape */ };
  const model = mapToLessonModel(dto);
  expect(model).toEqual({ /* UI shape */ });
});
```

### 4. Clear Contracts
- DTOs document the backend API
- Models document the UI requirements
- Mappers document the transformation logic

## Naming Conventions

| Type | Suffix | Example | Purpose |
|------|--------|---------|---------|
| Backend Response | `Dto` | `LessonDetailsDto` | Matches backend API |
| UI Data | `Model` | `LessonModel` | Stable UI interface |
| Request Payload | `RequestDto` | `CreateLessonRequestDto` | Request body schema |
| API Function | `fetch*` | `fetchLessonById()` | Fetches and maps data |
| Mapper Function | `mapTo*Model` | `mapToLessonModel()` | Transforms DTO to Model |

## File Organization

```
src/features/lessons/
├── api/
│   ├── LessonsAPI.ts          # API calls + Mappers
│   └── index.ts
├── types/
│   ├── LessonDetailsDto.ts     # Backend DTO
│   ├── LessonSummaryDto.ts     # Backend DTO
│   ├── CreateLessonRequestDto.ts # Request DTO
│   ├── LessonModel.ts          # UI Model (stable)
│   ├── Lesson.ts               # Legacy (deprecated)
│   └── index.ts
├── hooks/
│   └── use-lessons.ts          # Returns LessonModel
├── components/
│   └── LessonCard.tsx          # Consumes LessonModel
├── pages/
│   └── LessonPage.tsx          # Consumes LessonModel
└── constants.ts                # Routes and resource IDs
```

## Migration Guide

### For New Features
1. Create `*Dto.ts` interfaces matching backend
2. Create `*Model.ts` interfaces for UI consumption
3. Implement `mapTo*Model()` functions in API layer
4. Return Models from hooks
5. Use Models in components

### For Existing Features
1. Identify current interface (e.g., `Lesson`)
2. Rename to `LessonDto` and align with backend
3. Create new `LessonModel` with desired UI structure
4. Add mapper in API layer
5. Update hooks to return Models
6. Gradually update components to use Models
7. Mark old interface as `@deprecated`

## Common Patterns

### Handling Null/Undefined
```typescript
function mapToLessonModel(dto: LessonDetailsDto): LessonModel {
  return {
    description: dto.description || null,  // Normalize to null
    videoUrl: dto.videoUrl ?? null,        // Use nullish coalescing
  };
}
```

### Field Name Mapping
```typescript
function mapToLessonModel(dto: LessonDetailsDto): LessonModel {
  return {
    order: dto.index,              // Backend: index, UI: order
    thumbnailImage: dto.thumbnailUrl,  // Backend: thumbnailUrl, UI: thumbnailImage
  };
}
```

### Nested Object Mapping
```typescript
function mapToCourseModel(dto: CourseDetailsDto): CourseModel {
  return {
    lessons: dto.lessons.map(mapLessonSummaryToModel),  // Map nested arrays
    price: {
      amount: dto.price,
      currency: dto.currency,
    },
  };
}
```

## Testing Strategy

### 1. Mapper Unit Tests
Test that DTOs correctly transform to Models:
```typescript
describe('mapToLessonModel', () => {
  it('should map all fields correctly', () => {
    const dto: LessonDetailsDto = { /* ... */ };
    const model = mapToLessonModel(dto);
    expect(model.lessonId).toBe(dto.lessonId);
    expect(model.order).toBe(dto.index);
  });
});
```

### 2. Component Tests
Mock the UI Model, not the DTO:
```typescript
const mockLesson: LessonModel = { /* ... */ };
render(<LessonCard lesson={mockLesson} />);
```

### 3. Integration Tests
Verify the full flow from API to UI:
```typescript
test('displays lesson data correctly', async () => {
  mockAxios.get.mockResolvedValue({ data: lessonDto });
  render(<LessonPage />);
  expect(await screen.findByText(lessonDto.title)).toBeInTheDocument();
});
```

## Best Practices

1. **Never import DTOs in components** - Only hooks and API layer should use DTOs
2. **Keep Models stable** - Avoid changing Model interfaces unless absolutely necessary
3. **Document transformations** - Add comments in mappers explaining non-obvious transformations
4. **Use TypeScript strictly** - Enable `strict: true` in tsconfig.json
5. **Version your DTOs** - Consider versioned DTOs if breaking changes are frequent (e.g., `LessonDtoV1`, `LessonDtoV2`)

## Validation

See `ADAPTER_PATTERN_VALIDATION.md` for a detailed validation scenario demonstrating the adapter pattern's effectiveness.

## Questions?

For questions about this architecture, consult:
- `ADAPTER_PATTERN_VALIDATION.md` - Validation scenario
- API layer files - Implementation examples
- This README - Architecture overview
