# Course Page Updates - HATEOAS Implementation

This document summarizes the updates made to the course page to support the new backend API structure with HATEOAS (Hypermedia as the Engine of Application State) links.

## Changes Summary

### 1. Updated Type Definitions

#### CourseDetailsDto (CoursePageDto)

Updated to match the new backend response structure:

- Added instructor fields: `instructorId`, `instructorName`, `instructorAvatarUrl`
- Kept all metadata: `categoryId`, `categoryName`, `categorySlug`
- All fields for modules, lessons, tags, etc. maintained

#### ModuleDto & ModuleLessonDto

- Changed lesson `access` field from boolean to string ("Private" | "Public")
- This aligns with the backend enum representation

### 2. New Components

#### CourseMetadata Component

A reusable component that displays:

- **Instructor Information**: Avatar and name
- **Course Statistics**:
  - Students enrolled
  - Number of lessons
  - Total duration
  - Category
- **Tags**: Visual badge display of all course tags

**Location**: `src/features/courses/components/CourseMetadata.tsx`

**Usage**:

```tsx
<CourseMetadata course={course} />
```

#### HateoasButton Component

A reusable button component that:

- Automatically checks if a HATEOAS link relation exists
- Renders a button with proper state (enabled/disabled)
- Can optionally hide when the action is not available
- Supports all standard Button props

**Location**: `src/components/common/HateoasButton/HateoasButton.tsx`

**Usage Examples**:

```tsx
// Simple delete button
<HateoasButton
  links={course.links}
  rel="delete"
  icon={Trash2}
  onClick={(link) => handleDelete(link.href)}
  variant="destructive"
  hideIfDisabled
>
  Delete Course
</HateoasButton>

// Update button
<HateoasButton
  links={module.links}
  rel="partial-update"
  icon={Edit}
  onClick={(link) => handleEdit(link.href)}
  variant="outline"
>
  Edit Module
</HateoasButton>

// Create lesson button
<HateoasButton
  links={module.links}
  rel="create-lesson"
  icon={Plus}
  onClick={(link) => handleCreateLesson(link.href)}
>
  Add Lesson
</HateoasButton>
```

### 3. Updated Course Page Structure

The course page now displays information in this order:

1. **Course Header** - Title, image, pricing, action buttons
2. **Course Metadata** - NEW! Instructor, stats, category, tags
3. **About Section** - Course description (editable)
4. **Course Content** - Modules and lessons

### 4. HATEOAS Link Relations

The following link relations are used throughout the application:

#### Course Relations (`CourseRels`)

- `self` - Get course details
- `partial-update` - Update course fields
- `delete` - Delete course
- `create-module` - Create a new module
- `generate-image-upload-url` - Upload course image

#### Module Relations (`ModuleRels`)

- `self` - Get module details
- `create-lesson` - Create a new lesson in module
- `partial-update` - Update module
- `delete` - Delete module

#### Lesson Relations (`LessonRels`)

- `self` - Get lesson details
- `partial-update` - Update lesson
- `delete` - Delete lesson
- `upload-video-url` - Upload lesson video
- `generate-video-upload-url` - Generate video upload URL

### 5. API Mapping Updates

Updated `CoursesAPI.ts` to properly handle:

- New instructor fields from backend
- Lesson `access` property conversion (string → boolean `isPreview`)
- Support for both `LessonSummaryDto` and `ModuleLessonDto` types

### 6. Translation Keys Added

#### English (`en/courses.json`)

```json
{
  "detail": {
    "courseInfo": "Course Information",
    "enrolled": "Students Enrolled",
    "duration": "Total Duration",
    "category": "Category",
    "tags": "Tags"
  }
}
```

#### Hebrew (`he/courses.json`)

```json
{
  "detail": {
    "courseInfo": "מידע על הקורס",
    "enrolled": "תלמידים רשומים",
    "duration": "משך כולל",
    "category": "קטגוריה",
    "tags": "תגיות"
  }
}
```

## Benefits of HATEOAS Approach

1. **Permission-Based UI**: Buttons/actions only appear if the user has permission (determined by backend links)
2. **Reduced Hardcoding**: No need to construct URLs on frontend - backend provides them
3. **API Evolution**: Backend can change URL structure without breaking frontend
4. **Self-Documenting**: Links show what actions are available at any state
5. **Consistent Patterns**: `HateoasButton` provides reusable pattern across app

## Example: Backend Response Structure

```json
{
  "id": "019be5b9-7a87-72b6-86f8-9079d093cc37",
  "title": "קורס לדוגמה",
  "instructorName": "Dekel Rafian",
  "instructorAvatarUrl": null,
  "modules": [
    {
      "id": "module-id",
      "title": "פרק 1",
      "lessons": [
        {
          "lessonId": "lesson-id",
          "title": "שיעור 1",
          "access": "Private",
          "links": [
            {
              "href": "https://api/modules/module-id/lessons/lesson-id",
              "rel": "self",
              "method": "GET"
            },
            {
              "href": "https://api/modules/module-id/lessons/lesson-id",
              "rel": "partial-update",
              "method": "PATCH"
            }
          ]
        }
      ],
      "links": [
        {
          "href": "https://api/modules/module-id/lessons",
          "rel": "create-lesson",
          "method": "POST"
        }
      ]
    }
  ],
  "links": [
    {
      "href": "https://api/courses/course-id",
      "rel": "self",
      "method": "GET"
    },
    {
      "href": "https://api/courses/course-id",
      "rel": "partial-update",
      "method": "PATCH"
    },
    {
      "href": "https://api/courses/course-id/modules",
      "rel": "create-module",
      "method": "POST"
    }
  ]
}
```

## Future Enhancements

Potential areas for improvement:

1. Convert more action buttons to use `HateoasButton`
2. Add loading states tied to link availability
3. Create `HateoasLink` component for navigation links
4. Add tooltips showing why actions are disabled
5. Implement optimistic updates for better UX
