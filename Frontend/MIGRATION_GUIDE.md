# Migration Guide: Adopting the Adapter Pattern

This guide helps developers understand how to work with the new Adapter Pattern architecture.

## Quick Start

### For New Developers

If you're new to this codebase, follow these simple rules:

1. **In Components**: Only import and use `*Model` types
2. **In API Files**: Only create DTOs and mappers
3. **In Hooks**: Return Models from API functions

That's it! The architecture will guide you naturally.

### For Existing Developers

The codebase now separates concerns between:
- **DTOs**: What the backend sends
- **Models**: What the UI needs
- **Mappers**: How we transform between them

## Common Tasks

### Task 1: Fetching Data from Backend

**Old Way**:
```typescript
// Component directly uses backend shape
const { data: lesson } = useQuery({
  queryFn: () => axios.get<Lesson>('/lessons/123')
});

// Component depends on backend field names
<div>{lesson.lessonId}</div>
```

**New Way**:
```typescript
// 1. Define DTO (matches backend)
export interface LessonDetailsDto {
  lessonId: string;  // Backend field name
  // ...
}

// 2. Define Model (UI needs)
export interface LessonModel {
  lessonId: string;  // UI field name
  // ...
}

// 3. Create mapper in API file
function mapToLessonModel(dto: LessonDetailsDto): LessonModel {
  return {
    lessonId: dto.lessonId,
    // ... transform other fields
  };
}

// 4. API function uses mapper
export async function fetchLessonById(id: string): Promise<LessonModel> {
  const response = await axios.get<LessonDetailsDto>(`/lessons/${id}`);
  return mapToLessonModel(response.data);
}

// 5. Hook returns Model
export function useLesson(id: string) {
  return useQuery<LessonModel, Error>({
    queryFn: () => fetchLessonById(id)
  });
}

// 6. Component uses Model
const { data: lesson } = useLesson('123');
<div>{lesson.lessonId}</div>  // Uses Model field
```

### Task 2: Handling Backend Changes

**Scenario**: Backend changes `lessonId` to `id`

**Steps**:

1. Update the DTO:
```typescript
// LessonDetailsDto.ts
export interface LessonDetailsDto {
  id: string;  // CHANGED from lessonId
  title: string;
  // ...
}
```

2. Update the mapper:
```typescript
// LessonsAPI.ts
function mapToLessonModel(dto: LessonDetailsDto): LessonModel {
  return {
    lessonId: dto.id,  // CHANGED: map new field to old Model field
    title: dto.title,
    // ...
  };
}
```

3. Done! No component changes needed ✅

### Task 3: Adding a New Field

**Scenario**: Backend adds `difficulty` field to lessons

**Steps**:

1. Add to DTO:
```typescript
// LessonDetailsDto.ts
export interface LessonDetailsDto {
  lessonId: string;
  difficulty?: string;  // NEW FIELD
  // ...
}
```

2. Decide if UI needs it:

**Option A - UI needs it:**
```typescript
// LessonModel.ts
export interface LessonModel {
  lessonId: string;
  difficulty: string | null;  // Add to Model
  // ...
}

// LessonsAPI.ts - Update mapper
function mapToLessonModel(dto: LessonDetailsDto): LessonModel {
  return {
    lessonId: dto.lessonId,
    difficulty: dto.difficulty ?? null,  // Map it
    // ...
  };
}
```

**Option B - UI doesn't need it:**
```typescript
// Don't add to Model - just ignore it in mapper
function mapToLessonModel(dto: LessonDetailsDto): LessonModel {
  return {
    lessonId: dto.lessonId,
    // difficulty is intentionally not mapped
    // ...
  };
}
```

### Task 4: Creating a New Feature

**Steps**:

1. **Create DTOs** matching backend API:
```typescript
// features/newfeature/types/NewFeatureDto.ts
export interface NewFeatureDto {
  id: string;
  name: string;
  // ... exact backend shape
}
```

2. **Create Model** for UI:
```typescript
// features/newfeature/types/NewFeatureModel.ts
export interface NewFeatureModel {
  id: string;
  name: string;
  // ... desired UI shape
}
```

3. **Create API with mapper**:
```typescript
// features/newfeature/api/NewFeatureAPI.ts
function mapToNewFeatureModel(dto: NewFeatureDto): NewFeatureModel {
  return {
    id: dto.id,
    name: dto.name,
    // ... transformation
  };
}

export async function fetchNewFeature(id: string): Promise<NewFeatureModel> {
  const response = await axios.get<NewFeatureDto>(`/api/newfeature/${id}`);
  return mapToNewFeatureModel(response.data);
}
```

4. **Create hook**:
```typescript
// features/newfeature/hooks/use-newfeature.ts
export function useNewFeature(id: string) {
  return useQuery<NewFeatureModel, Error>({
    queryKey: ['newfeature', id],
    queryFn: () => fetchNewFeature(id),
  });
}
```

5. **Use in component**:
```typescript
// features/newfeature/components/NewFeatureCard.tsx
import type { NewFeatureModel } from "../types/NewFeatureModel";

interface Props {
  feature: NewFeatureModel;  // Use Model!
}

export function NewFeatureCard({ feature }: Props) {
  return <div>{feature.name}</div>;
}
```

## Common Pitfalls

### ❌ Don't: Import DTOs in Components

```typescript
// BAD - Component imports DTO
import type { LessonDetailsDto } from "../types/LessonDetailsDto";

function MyComponent({ lesson }: { lesson: LessonDetailsDto }) {
  // ...
}
```

```typescript
// GOOD - Component imports Model
import type { LessonModel } from "../types/LessonModel";

function MyComponent({ lesson }: { lesson: LessonModel }) {
  // ...
}
```

### ❌ Don't: Return DTOs from Hooks

```typescript
// BAD - Hook returns DTO
export function useLesson(id: string) {
  return useQuery<LessonDetailsDto, Error>({
    // ...
  });
}
```

```typescript
// GOOD - Hook returns Model
export function useLesson(id: string) {
  return useQuery<LessonModel, Error>({
    queryFn: () => fetchLessonById(id),  // Returns Model
  });
}
```

### ❌ Don't: Skip the Mapper

```typescript
// BAD - Directly return DTO as Model
export async function fetchLesson(id: string): Promise<LessonModel> {
  const response = await axios.get<LessonDetailsDto>(`/lessons/${id}`);
  return response.data as unknown as LessonModel;  // Type casting!
}
```

```typescript
// GOOD - Use the mapper
export async function fetchLesson(id: string): Promise<LessonModel> {
  const response = await axios.get<LessonDetailsDto>(`/lessons/${id}`);
  return mapToLessonModel(response.data);  // Proper transformation
}
```

### ❌ Don't: Change Models Frequently

```typescript
// BAD - Changing Model to match every backend change
export interface LessonModel {
  id: string;  // Changed from lessonId to match backend
}
```

```typescript
// GOOD - Keep Model stable, update mapper
export interface LessonModel {
  lessonId: string;  // Keep UI field stable
}

function mapToLessonModel(dto: LessonDetailsDto): LessonModel {
  return {
    lessonId: dto.id,  // Mapper handles backend change
  };
}
```

## Checklist for Code Reviews

When reviewing code, check:

- [ ] Components only import `*Model` types, never `*Dto` types
- [ ] API functions return Models, not DTOs
- [ ] Hooks specify Model as the return type in `useQuery<Model, Error>`
- [ ] New DTOs match backend documentation
- [ ] Mappers handle null/undefined appropriately
- [ ] Models remain stable unless there's a compelling UI reason to change

## FAQ

### Q: When should I change a Model?

**A**: Only when the UI requirements genuinely change. Don't change Models just because the backend changed. That's what mappers are for.

### Q: Should every DTO have a corresponding Model?

**A**: Usually yes, but not always. Some DTOs (like request bodies) may not need Models if they're only used in form submissions.

### Q: Can I have multiple DTOs for one Model?

**A**: Yes! For example, `LessonSummaryDto` and `LessonDetailsDto` both map to `LessonModel`. The summary has fewer fields, so the mapper fills in defaults.

### Q: What if the transformation is complex?

**A**: Keep the mapper pure and focused. If transformation gets complex, break it into helper functions:

```typescript
function mapToLessonModel(dto: LessonDetailsDto): LessonModel {
  return {
    lessonId: dto.lessonId,
    duration: formatDuration(dto.duration),  // Helper function
    metadata: transformMetadata(dto.metadata),  // Helper function
  };
}

function formatDuration(duration: string | null): string | null {
  // Complex duration formatting logic
}
```

### Q: Should I write tests for mappers?

**A**: Yes! Mappers are perfect for unit testing:

```typescript
describe('mapToLessonModel', () => {
  it('maps all required fields', () => {
    const dto: LessonDetailsDto = { /* test data */ };
    const model = mapToLessonModel(dto);
    expect(model.lessonId).toBe(dto.lessonId);
  });
  
  it('handles null values correctly', () => {
    const dto: LessonDetailsDto = { duration: null };
    const model = mapToLessonModel(dto);
    expect(model.duration).toBeNull();
  });
});
```

## Resources

- **ARCHITECTURE.md** - Comprehensive architecture guide
- **ADAPTER_PATTERN_VALIDATION.md** - Example validation scenario
- **LessonsAPI.ts** - Reference implementation
- **CoursesAPI.ts** - Reference implementation

## Getting Help

If you're unsure about how to apply this pattern to your task:

1. Look at existing implementations (LessonsAPI.ts, CoursesAPI.ts)
2. Check the FAQ above
3. Ask in team chat with specific questions
4. Reference this guide during code review discussions

Remember: **The pattern exists to protect us from backend changes. When in doubt, add a mapper!**
