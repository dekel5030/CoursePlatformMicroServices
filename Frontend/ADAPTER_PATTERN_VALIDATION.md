# Adapter Pattern Validation Test

This document demonstrates how the Adapter Pattern protects the UI from backend schema changes.

## Scenario: Backend Changes Field Name

### Example 1: Lesson Field Rename

**Backend Change:** The backend changes `lessonId` to `id` in the API response.

**Before Adapter Pattern:**
- Without the adapter pattern, this change would require updates in:
  - `LessonCard.tsx` (multiple references)
  - `LessonPage.tsx` (multiple references)
  - `CourseLessonsSection.tsx` (references in map)
  - `use-lessons.ts` (query key construction)
  - Any other component using lessons
  - **Total: 5+ files affected**

**After Adapter Pattern:**
- With the adapter pattern, this change only requires:
  - Update `LessonDetailsDto.ts`: Change `lessonId: string` to `id: string`
  - Update `mapToLessonModel()` in `LessonsAPI.ts`: Change `lessonId: dto.lessonId` to `lessonId: dto.id`
  - **Total: 2 lines in 1 file (the mapper)**
  - All UI components continue to work without any changes!

## Test Case Implementation

### Step 1: Current Implementation
```typescript
// LessonDetailsDto.ts (matches backend)
export interface LessonDetailsDto {
  courseId: string;
  lessonId: string;  // Backend sends this field
  title: string;
  // ... other fields
}

// LessonsAPI.ts (mapper)
function mapToLessonModel(dto: LessonDetailsDto): LessonModel {
  return {
    courseId: dto.courseId,
    lessonId: dto.lessonId,  // Maps to stable UI field
    title: dto.title,
    // ... other mappings
  };
}

// LessonModel.ts (stable UI interface)
export interface LessonModel {
  courseId: string;
  lessonId: string;  // UI always expects this field name
  title: string;
  // ... other fields
}
```

### Step 2: Simulated Backend Change
Backend changes API to return `id` instead of `lessonId`:

```typescript
// LessonDetailsDto.ts (UPDATE: matches new backend schema)
export interface LessonDetailsDto {
  courseId: string;
  id: string;  // CHANGED: Backend now sends "id"
  title: string;
  // ... other fields
}

// LessonsAPI.ts (UPDATE: adapter handles the change)
function mapToLessonModel(dto: LessonDetailsDto): LessonModel {
  return {
    courseId: dto.courseId,
    lessonId: dto.id,  // CHANGED: Maps backend's "id" to UI's "lessonId"
    title: dto.title,
    // ... other mappings
  };
}

// LessonModel.ts (NO CHANGE: UI interface remains stable)
export interface LessonModel {
  courseId: string;
  lessonId: string;  // UI still expects this field name
  title: string;
  // ... other fields
}
```

### Step 3: Verification
All UI components continue to work without any modifications:
- ✅ `LessonCard.tsx` - No changes needed
- ✅ `LessonPage.tsx` - No changes needed
- ✅ `CourseLessonsSection.tsx` - No changes needed
- ✅ `use-lessons.ts` - No changes needed
- ✅ All other lesson components - No changes needed

## Benefits Demonstrated

1. **Single Responsibility**: The mapper is the only place that knows about both schemas
2. **Reduced Risk**: UI components are isolated from backend changes
3. **Easier Maintenance**: One file to update instead of 5+
4. **Type Safety**: TypeScript ensures the mapper correctly transforms data
5. **Testability**: Mappers can be unit tested independently

## Conclusion

The Adapter Pattern successfully decouples the UI from the API schema. A backend field rename that would have previously affected 5+ files now only requires updating 1-2 lines in the mapper function.
