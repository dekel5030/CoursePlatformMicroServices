# Visual Guide: Inline Editing Feature

This document provides a visual representation of the inline editing feature behavior.

## States and Transitions

### State 1: View Mode (Default)

```
┌────────────────────────────────────────────────────┐
│  Course Title: Introduction to Revit            📝 │  ← Edit icon on hover
└────────────────────────────────────────────────────┘
```

**User Experience:**
- Normal text display
- Edit icon (📝) appears when hovering
- Click edit icon to enter edit mode

---

### State 2: Edit Mode (Active)

```
┌────────────────────────────────────────────────────┐
│  [Introduction to Revit                        ] ✓ ✗│
│   ↑ Text input field          Save → ✓    Cancel → ✗│
└────────────────────────────────────────────────────┘
```

**User Experience:**
- Text becomes editable input
- Text is pre-selected for easy replacement
- Save (✓) and Cancel (✗) buttons visible
- Press Enter to save
- Press Escape to cancel

---

### State 3: Saving

```
┌────────────────────────────────────────────────────┐
│  [Introduction to Revit BIM                    ] ⏳ │
│                                    Saving...      │
└────────────────────────────────────────────────────┘
```

**User Experience:**
- Buttons disabled during save
- Loading indicator shows
- Cannot interact until complete

---

### State 4: Success

```
┌────────────────────────────────────────────────────┐
│  Introduction to Revit BIM                         │
└────────────────────────────────────────────────────┘

Toast Notification:
┌────────────────────────────────┐
│ ✅ Title updated successfully   │
└────────────────────────────────┘
```

**User Experience:**
- Returns to view mode
- Green toast notification appears
- Updated content displayed

---

### State 5: Error

```
┌────────────────────────────────────────────────────┐
│  [Introduction to Revit                        ] ✓ ✗│
└────────────────────────────────────────────────────┘

Toast Notification:
┌────────────────────────────────┐
│ ❌ Failed to update title       │
└────────────────────────────────┘
```

**User Experience:**
- Stays in edit mode
- Red toast notification appears
- Original value restored
- User can try again

---

## Multi-line Editing (Description)

### View Mode with Hover

```
┌──────────────────────────────────────────────────────┐
│ About This Course                                  📝 │
├──────────────────────────────────────────────────────┤
│ Learn the fundamentals of BIM modeling with Revit.  │
│ This comprehensive course covers everything from     │
│ basic concepts to advanced techniques.               │
└──────────────────────────────────────────────────────┘
```

---

### Edit Mode

```
┌──────────────────────────────────────────────────────┐
│ About This Course                                    │
├──────────────────────────────────────────────────────┤
│ ┌────────────────────────────────────────────────┐   │
│ │ Learn the fundamentals of BIM modeling with    │   │
│ │ Revit. This comprehensive course covers        │   │
│ │ everything from basic concepts to advanced     │   │
│ │ techniques.                                    │   │
│ │                                                │   │
│ └────────────────────────────────────────────────┘   │
│                                                      │
│  [Save] [Cancel]  Ctrl+Enter to save • Esc to cancel│
└──────────────────────────────────────────────────────┘
```

**User Experience:**
- Textarea expands for editing
- Save/Cancel buttons below
- Keyboard shortcuts shown
- Can use Ctrl+Enter to save quickly

---

## Authorization Check

### User WITH Update Permission

```
┌────────────────────────────────────────────────────┐
│  Course Title: Introduction to Revit            📝 │  ← Editable
└────────────────────────────────────────────────────┘
```

### User WITHOUT Update Permission

```
┌────────────────────────────────────────────────────┐
│  Course Title: Introduction to Revit               │  ← Static text only
└────────────────────────────────────────────────────┘
```

**Implementation:**
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

---

## Lesson List with Inline Editing

```
┌──────────────────────────────────────────────────────────────┐
│ Lessons                                       [+ Add Lesson] │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  1  │ Getting Started with Revit              📝  ⏱ 15m│   │
│  │     │ Introduction to the interface...       📝      ▶│   │
│  └──────────────────────────────────────────────────────┘   │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  2  │ Creating Your First Project            📝  ⏱ 30m│   │
│  │     │ Step-by-step guide to starting...     📝      ▶│   │
│  └──────────────────────────────────────────────────────┘   │
│                                                              │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  3  │ Working with Walls and Doors           📝  ⏱ 45m│   │
│  │     │ Learn to place and modify elements... 📝      ▶│   │
│  └──────────────────────────────────────────────────────┘   │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

**Key Points:**
- Each lesson has TWO inline edit icons (title + description)
- Clicking edit icon only affects that specific field
- Click stops event propagation (doesn't navigate to lesson)
- Play button (▶) still navigates to lesson page

---

## Interaction Flow Diagram

```
    ┌─────────────┐
    │  View Mode  │
    │  (Default)  │
    └──────┬──────┘
           │
           │ Hover → Show Edit Icon
           │ Click Edit Icon
           ▼
    ┌─────────────┐
    │  Edit Mode  │
    │  (Active)   │
    └──────┬──────┘
           │
           ├─────────────────┐
           │                 │
           │ Enter/Blur      │ Escape
           │ (Save)          │ (Cancel)
           ▼                 ▼
    ┌─────────────┐   ┌─────────────┐
    │   Saving    │   │  View Mode  │
    │   State     │   │  (Reverted) │
    └──────┬──────┘   └─────────────┘
           │
           ├─────────────────┐
           │                 │
           │ Success         │ Error
           ▼                 ▼
    ┌─────────────┐   ┌─────────────┐
    │  View Mode  │   │  Edit Mode  │
    │  (Updated)  │   │  (Retry)    │
    └─────────────┘   └─────────────┘
           │                 │
           │                 │
           │   Toast         │   Toast
           │   ✅ Success    │   ❌ Error
           └─────────────────┘
```

---

## Component Props

### InlineEditableText

```typescript
interface InlineEditableTextProps {
  value: string;                    // Current value
  onSave: (newValue: string) => Promise<void>; // Save handler
  className?: string;               // Container class
  inputClassName?: string;          // Input field class
  displayClassName?: string;        // Display text class
  placeholder?: string;             // Placeholder text
  canEdit?: boolean;                // Enable/disable editing
  maxLength?: number;               // Max character limit
}
```

### InlineEditableTextarea

```typescript
interface InlineEditableTextareaProps {
  value: string;                    // Current value
  onSave: (newValue: string) => Promise<void>; // Save handler
  className?: string;               // Container class
  textareaClassName?: string;       // Textarea field class
  displayClassName?: string;        // Display text class
  placeholder?: string;             // Placeholder text
  canEdit?: boolean;                // Enable/disable editing
  rows?: number;                    // Number of rows
  maxLength?: number;               // Max character limit
}
```

---

## Real-world Example: Course Page

```tsx
// Course Title (in header)
<InlineEditableText
  value={course.title}
  onSave={handleTitleUpdate}
  displayClassName="text-3xl font-bold"
  inputClassName="text-3xl font-bold"
  placeholder="Enter course title..."
  maxLength={200}
/>

// Course Description (in about section)
<InlineEditableTextarea
  value={course.description || ""}
  onSave={handleDescriptionUpdate}
  displayClassName="text-muted-foreground"
  placeholder="Enter course description..."
  rows={5}
  maxLength={2000}
/>

// Save handler example
const handleTitleUpdate = async (newTitle: string) => {
  try {
    await patchCourse.mutateAsync({ title: newTitle });
    toast.success('Course title updated successfully');
  } catch (error) {
    toast.error('Failed to update course title');
    throw error; // Component will revert
  }
};
```

---

## Keyboard Shortcuts Summary

| Key | Action | Context |
|-----|--------|---------|
| Hover | Show edit icon | View mode |
| Click Edit | Enter edit mode | View mode |
| Enter | Save changes | Single-line edit mode |
| Ctrl+Enter | Save changes | Multi-line edit mode |
| Escape | Cancel and revert | Edit mode |
| Tab | Normal navigation | All modes |

---

## Accessibility Features

✅ **Focus Management**
- Auto-focus on edit mode entry
- Text pre-selected for replacement
- Focus returns on cancel

✅ **Keyboard Navigation**
- All actions accessible via keyboard
- No mouse required

✅ **Screen Readers**
- Proper ARIA labels
- State changes announced
- Button labels clear

✅ **Visual Feedback**
- Clear hover states
- Distinct edit/view modes
- Loading indicators
- Success/error notifications

---

## Browser Compatibility

✅ All modern browsers supported:
- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

Uses standard web APIs:
- Flexbox layout
- CSS transitions
- Promise-based async
- Standard event handlers

---

## Performance Considerations

✅ **Optimized for Speed**
- Minimal re-renders (React.memo not needed yet)
- Efficient state management
- Debouncing can be added if needed

✅ **Network Efficiency**
- Only changed fields sent
- PATCH requests (not full PUT)
- React Query caching
- Automatic retry on failure

✅ **User Experience**
- Non-blocking operations
- Instant feedback
- Graceful error handling
- Optimistic updates possible

---

This visual guide provides a comprehensive overview of the inline editing feature's behavior, states, and user interactions.
