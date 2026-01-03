# Inline Editing Feature

## Overview

This feature adds inline editing capabilities for Course and Lesson titles and descriptions with autosave functionality. Users with appropriate permissions can edit content directly without navigating to separate edit pages.

## Features Implemented

### 1. **Inline Editing Components**

Two reusable components were created for inline editing:

#### `InlineEditableText`
- Single-line text editing (for titles)
- Edit mode triggered by hovering and clicking edit icon
- Autosave on blur or Enter key
- Escape key to cancel
- Loading state during save
- Visual feedback with check/cancel buttons

#### `InlineEditableTextarea`
- Multi-line text editing (for descriptions)
- Edit mode triggered by hovering and clicking edit icon
- Autosave with Ctrl+Enter keyboard shortcut
- Escape to cancel
- Save/Cancel buttons with visual feedback
- Configurable rows and max length

### 2. **API Integration**

#### Backend Endpoints (Already Existed)
- `PATCH /courses/{id}` - Partial update for courses
- `PATCH /lessons/{id}` - Partial update for lessons

Both endpoints accept nullable fields:
- `title?: string`
- `description?: string`
- Additional fields (instructorId, price, access, etc.)

#### Frontend API Functions
```typescript
// CoursesAPI.ts
export async function patchCourse(
  id: string,
  request: PatchCourseRequest
): Promise<void>;

// LessonsAPI.ts
export async function patchLesson(
  id: string,
  request: PatchLessonRequest
): Promise<void>;
```

### 3. **React Query Hooks**

```typescript
// use-courses.ts
export function usePatchCourse(id: string);

// use-lessons.ts
export function usePatchLesson(id: string, courseId?: string);
```

Both hooks:
- Automatically invalidate relevant queries on success
- Integrate with React Query for optimistic updates
- Handle error states

### 4. **Integration Points**

#### Course Pages
1. **CourseHeader.tsx** - Inline edit course title
   - Only visible to users with Update permission on Course resource
   - Hover to reveal edit button
   - Click to enter edit mode
   - Save with Enter or blur

2. **CoursePage.tsx** - Inline edit course description
   - Permission-based access control
   - Multi-line textarea with save/cancel buttons
   - Ctrl+Enter keyboard shortcut for quick save

#### Lesson Pages
1. **LessonCard.tsx** - Inline edit lesson title and description
   - Inline editing in the lesson list view
   - Click stops propagation to prevent navigation
   - Individual edit controls for title and description

2. **LessonPage.tsx** - Inline edit lesson title and description
   - Full page editing experience
   - Same permission-based controls
   - Keyboard shortcuts for efficiency

## User Experience

### Visual States

1. **View Mode (Default)**
   - Content displayed normally
   - Edit icon appears on hover (opacity transition)
   - Clear indication of editable fields for authorized users

2. **Edit Mode**
   - Input field or textarea replaces static text
   - Save (✓) and Cancel (✗) buttons visible
   - Focus automatically set to input
   - Text pre-selected for easy replacement

3. **Saving State**
   - Buttons disabled during save
   - Visual feedback prevents double-submission
   - Toast notification on success/failure

### Keyboard Shortcuts

- **Enter** (single-line): Save changes
- **Ctrl+Enter** (multi-line): Save changes
- **Escape**: Cancel and revert changes
- **Tab**: Navigate between fields (normal behavior)

## Authorization

The feature integrates with the existing authorization system:

```tsx
<Authorized
  action={ActionType.Update}
  resource={ResourceType.Course}
  resourceId={ResourceId.create(courseId)}
  fallback={<StaticDisplay />}
>
  <InlineEditableText {...props} />
</Authorized>
```

Users without Update permission see static text instead of editable fields.

## Toast Notifications

User feedback for all actions:

- ✅ "Course title updated successfully"
- ✅ "Course description updated successfully"
- ✅ "Lesson title updated successfully"
- ✅ "Lesson description updated successfully"
- ❌ "Failed to update course title"
- ❌ "Failed to update course description"
- ❌ "Failed to update lesson title"
- ❌ "Failed to update lesson description"

## Internationalization

All messages support both English (en) and Hebrew (he) translations:

```json
// courses.json
{
  "detail": {
    "enterTitle": "Enter course title...",
    "enterDescription": "Enter course description...",
    "titleUpdated": "Course title updated successfully",
    "descriptionUpdated": "Course description updated successfully",
    // ... error messages
  }
}
```

## Technical Implementation

### Partial Updates

Only changed fields are sent to the backend:

```typescript
// Example: Updating only the title
await patchCourse.mutateAsync({ title: "New Title" });

// Example: Updating only the description
await patchCourse.mutateAsync({ description: "New Description" });
```

This follows REST best practices for PATCH operations.

### Query Invalidation

After successful updates:
- Specific resource query is invalidated (e.g., `courses.detail(id)`)
- Related queries are invalidated (e.g., `courses.featured()`)
- React Query automatically refetches to ensure UI consistency

### Error Handling

```typescript
const handleTitleUpdate = async (newTitle: string) => {
  try {
    await patchCourse.mutateAsync({ title: newTitle });
    toast.success(t('courses:detail.titleUpdated'));
  } catch (error) {
    toast.error(t('courses:detail.titleUpdateFailed'));
    throw error; // Revert UI changes
  }
};
```

## Accessibility

- Focus management (auto-focus on edit)
- Keyboard navigation support
- Screen reader friendly (proper ARIA labels)
- Clear visual feedback for all states
- Hover states with appropriate contrast

## Browser Compatibility

The feature uses standard HTML5 and CSS3 features:
- Flexbox layout
- CSS transitions
- Modern event handlers
- No browser-specific hacks needed

## Future Enhancements

Potential improvements:
1. Optimistic updates (show change before server confirmation)
2. Debounced autosave (save after typing stops)
3. Conflict detection (multiple users editing same field)
4. Revision history
5. Rich text editing for descriptions
6. Inline editing for additional fields (price, tags, etc.)

## Testing Recommendations

### Manual Testing Checklist
- [ ] Edit course title with update permission
- [ ] Edit course description with update permission
- [ ] Edit lesson title in list view
- [ ] Edit lesson description in list view
- [ ] Edit lesson title on detail page
- [ ] Edit lesson description on detail page
- [ ] Verify toast notifications appear
- [ ] Test keyboard shortcuts (Enter, Ctrl+Enter, Escape)
- [ ] Test without update permission (should show static text)
- [ ] Test with network errors (should show error toast)
- [ ] Test with long text (max length validation)
- [ ] Test in RTL language (Hebrew)

### Unit Tests (Recommended)
```typescript
describe('InlineEditableText', () => {
  it('should show edit button on hover');
  it('should enter edit mode on click');
  it('should save on Enter key');
  it('should cancel on Escape key');
  it('should revert on save error');
  // ... more tests
});
```

## Code Structure

```
Frontend/src/
├── components/
│   ├── InlineEditableText.tsx          # Single-line inline editor
│   ├── InlineEditableTextarea.tsx      # Multi-line inline editor
│   └── ui/
│       └── textarea.tsx                 # New Textarea component
├── features/
│   ├── courses/
│   │   ├── api/CoursesAPI.ts           # Added patchCourse
│   │   ├── hooks/use-courses.ts        # Added usePatchCourse
│   │   ├── components/
│   │   │   └── CourseHeader.tsx        # Integrated inline editing
│   │   ├── pages/
│   │   │   └── CoursePage.tsx          # Integrated inline editing
│   │   └── locales/
│   │       ├── en/courses.json         # Added translation keys
│   │       └── he/courses.json         # Added translation keys
│   └── lessons/
│       ├── api/LessonsAPI.ts           # Added patchLesson
│       ├── hooks/use-lessons.ts        # Added usePatchLesson
│       ├── components/
│       │   └── LessonCard.tsx          # Integrated inline editing
│       ├── pages/
│       │   └── LessonPage.tsx          # Integrated inline editing
│       └── locales/
│           ├── en/lessons.json         # Added translation keys
│           └── he/lessons.json         # Added translation keys
```

## Summary

This feature provides a modern, user-friendly way to edit course and lesson content with:
- ✅ Minimal friction (no page navigation)
- ✅ Partial updates (only changed fields sent)
- ✅ Permission-based access control
- ✅ Clear visual feedback
- ✅ Keyboard shortcuts
- ✅ Internationalization support
- ✅ Error handling with user notifications
- ✅ Accessibility compliance
