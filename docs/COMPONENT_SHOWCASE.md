# Auth Management UI/UX Redesign - Visual Component Guide

## Component Showcase

### 1. Badge Component

**Purpose**: Compact visual indicators for status, roles, and permissions

**Variants**:
- `default` - Gray background, neutral
- `success` - Green background, for "Allow" permissions or active status
- `warning` - Orange background, for warnings
- `error` - Red background, for "Deny" permissions or errors
- `info` - Blue background, for informational tags

**Usage Examples**:
```tsx
<Badge variant="default">Admin</Badge>
<Badge variant="success">Allow</Badge>
<Badge variant="error">Deny</Badge>
<Badge variant="info">ULID-123</Badge>
```

**Features**:
- Rounded pill shape
- Small, unobtrusive size
- Clear color coding
- Can contain icons
- Responsive to content size

---

### 2. Drawer Component

**Purpose**: Side panel for contextual editing without losing page context

**Features**:
- Slides in from right side
- Modal overlay dims background
- Close button in header
- Scrollable content area
- Smooth animations (300ms)
- Escape key to close
- Click outside to close
- Body scroll lock when open

**Layout**:
```
┌──────────────────────────────────────┐
│ [Header Title]               [X]     │
├──────────────────────────────────────┤
│                                      │
│  Scrollable Content Area             │
│                                      │
│  - Form fields                       │
│  - Information display               │
│  - Action buttons                    │
│                                      │
│                                      │
└──────────────────────────────────────┘
```

**Use Cases**:
- User role editing
- Quick info display
- Form submissions without page navigation

---

### 3. Switch Component

**Purpose**: Toggle control for binary states (on/off, enable/disable)

**Features**:
- Accessible (ARIA roles)
- Keyboard support (Space/Enter)
- Visual thumb animation
- Disabled state
- Optional label
- Smooth transitions

**States**:
- Unchecked: Gray background, thumb on left
- Checked: Blue background, thumb on right
- Disabled: Reduced opacity, no interaction

**Usage**:
```tsx
<Switch 
  checked={isEnabled}
  onCheckedChange={setIsEnabled}
  label="Enable Feature"
/>
```

---

### 4. MultiSelect Component

**Purpose**: Select multiple items from a dropdown list with search

**Features**:
- **Search functionality** - Filter options as you type
- **Selected items display** - Visual tags with remove buttons
- **Autocomplete** - Shows matching options
- **Keyboard navigation** - Arrow keys, Enter to select
- **Click outside to close**
- **Empty state** - "No options found"
- **Placeholder** when empty

**Visual Flow**:
```
┌────────────────────────────────────┐
│ [Selected 1 ×] [Selected 2 ×]  ▼  │  ← Trigger (closed)
└────────────────────────────────────┘

Click ↓

┌────────────────────────────────────┐
│ [Selected 1 ×] [Selected 2 ×]  ▲  │  ← Trigger (open)
├────────────────────────────────────┤
│ [Search input field...]            │  ← Search bar
├────────────────────────────────────┤
│ Option 1                           │  ← Available options
│ Option 2                           │
│ Option 3                           │
└────────────────────────────────────┘
```

**Key Advantages**:
- ✅ No manual typing of IDs
- ✅ Validation through selection
- ✅ Visual confirmation of selection
- ✅ Easy to remove items

---

### 5. UserTable Component

**Purpose**: High-performance data table for user list

**Features**:
- **TanStack React Table** - Industry standard
- **Global search** - Searches all text fields
- **Column sorting** - Click headers to sort
- **Responsive layout**
- **Row actions** - Edit button per row
- **Empty state** - Graceful no-data display
- **Loading skeleton** - Smooth loading experience

**Columns**:
1. **Name** - First + Last name, bold
2. **Email** - Secondary text color
3. **Assigned Roles** - Badge list
4. **Actions** - Edit button

**Table Layout**:
```
┌─────────────────────────────────────────────────────────────┐
│ [🔍 Search users by name, email, or role...]               │
├──────────────┬──────────────┬──────────────┬───────────────┤
│ Name ▲       │ Email        │ Assigned     │ Actions       │
│              │              │ Roles        │               │
├──────────────┼──────────────┼──────────────┼───────────────┤
│ John Doe     │ john@ex.com  │ [Admin]      │ [Edit] →      │
│              │              │ [Manager]    │               │
├──────────────┼──────────────┼──────────────┼───────────────┤
│ Jane Smith   │ jane@ex.com  │ [Instructor] │ [Edit] →      │
├──────────────┼──────────────┼──────────────┼───────────────┤
│ Bob Johnson  │ bob@ex.com   │ No roles     │ [Edit] →      │
└──────────────┴──────────────┴──────────────┴───────────────┘
```

**Interaction**:
- Click column header → Sort by that column
- Type in search → Filter rows instantly
- Click Edit → Open drawer for that user
- Hover row → Background highlight

---

### 6. UserDrawer Component

**Purpose**: Edit user details without leaving the list page

**Layout**:
```
┌────────────────────────────────────┐
│ Edit User                      [X] │
├────────────────────────────────────┤
│ User Information                   │
│ ─────────────────────────────      │
│ Name:   John Doe                   │
│ Email:  john@example.com           │
│                                    │
│ Assigned Roles                     │
│ ─────────────────────────────      │
│ ┌──────────────────────────────┐  │
│ │ [Admin ×] [Manager ×]     ▼  │  │ ← MultiSelect
│ └──────────────────────────────┘  │
│                                    │
│ [💾 Saving changes...]  ← Indicator│
│                                    │
│ Permissions                        │
│ ─────────────────────────────      │
│ [Allow] Read on Courses            │
│ [Allow] Write on Users             │
└────────────────────────────────────┘
```

**Features**:
- **Instant save** - Changes save on selection
- **Loading feedback** - Shows saving indicator
- **Error display** - Inline error messages
- **Read-only info** - Name and email
- **Permission display** - Shows direct permissions
- **Role management** - Via MultiSelect

---

### 7. PermissionMatrix Component

**Purpose**: Organized view and management of role permissions

**Layout**:
```
┌─────────────────────────────────────────────────┐
│ Permission Matrix - Admin                   [X] │
├─────────────────────────────────────────────────┤
│ [💾 Updating permissions...]                    │
├─────────────────────────────────────────────────┤
│ ┌─────────────────────────────────────────────┐ │
│ │ Course Management                    [5]    │ │ ← Category
│ ├─────────────────────────────────────────────┤ │
│ │ Read on Course [Allow] [Info: ULID]    ☑   │ │ ← Permission
│ │ Write on Course [Allow]                 ☑   │ │
│ │ Delete on Course [Allow]                ☐   │ │
│ └─────────────────────────────────────────────┘ │
│ ┌─────────────────────────────────────────────┐ │
│ │ User Management                      [3]    │ │
│ ├─────────────────────────────────────────────┤ │
│ │ Read on User [Allow]                    ☑   │ │
│ │ Write on User [Allow]                   ☑   │ │
│ │ Delete on User [Allow]                  ☐   │ │
│ └─────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────┘
```

**Categories**:
- Course Management
- User Management
- Enrollment Management
- Order Management
- Analytics
- System
- Other (uncategorized)

**Permission Display**:
- **Action** (Read, Write, Delete, etc.)
- **Resource** (User, Course, Order, etc.)
- **Effect Badge** (Allow/Deny)
- **Resource ID Badge** (if specific)
- **Toggle Switch** (enable/disable)

**Key Features**:
- ✅ Grouped by domain
- ✅ Toggle switches (no manual entry)
- ✅ Auto-save on toggle
- ✅ Visual feedback during save
- ✅ Collapsible categories (future)

---

### 8. RoleList Component (Enhanced)

**Purpose**: Visual cards for roles with key metrics

**Card Layout**:
```
┌─────────────────────────────────┐
│ Admin                           │ ← Role name
├─────────────────────────────────┤
│                                 │
│  👥 5         🔐 12              │ ← Stats
│  Users        Permissions       │
│                                 │
├─────────────────────────────────┤
│ [12 permissions]                │ ← Footer badge
└─────────────────────────────────┘
```

**Grid Layout**:
```
┌──────────┬──────────┬──────────┐
│  Admin   │ Manager  │ Student  │
│          │          │          │
│ 👥 5     │ 👥 10    │ 👥 100   │
│ 🔐 12    │ 🔐 8     │ 🔐 3     │
└──────────┴──────────┴──────────┘
```

**Features**:
- **Hover effect** - Lift and border color change
- **User count** - Shows how many users have role
- **Permission count** - Total permissions
- **Click to navigate** - To role detail page
- **Responsive grid** - Adjusts to screen size

---

## Color Palette

### Status Colors
- **Primary**: `#3b82f6` (Blue) - Actions, focus states
- **Success**: `#065f46` (Green) - Allow permissions, success states
- **Warning**: `#92400e` (Orange) - Warnings
- **Error**: `#991b1b` (Red) - Deny permissions, errors
- **Info**: `#1e40af` (Dark Blue) - Informational badges

### Neutral Colors
- **Text Primary**: `#111827` (Almost Black) - Main text
- **Text Secondary**: `#6b7280` (Gray) - Secondary text
- **Border**: `#e5e7eb` (Light Gray) - Borders
- **Background**: `#ffffff` (White) - Cards, modals
- **Background Secondary**: `#f9fafb` (Off-white) - Table headers, hover states

---

## Typography

### Font Sizes
- **2rem** - Page titles (h1)
- **1.25rem** - Section titles (h2)
- **1rem** - Body text, labels
- **0.875rem** - Small text, secondary info
- **0.75rem** - Captions, uppercase labels

### Font Weights
- **700** - Bold (titles, emphasis)
- **600** - Semi-bold (section headers)
- **500** - Medium (buttons, labels)
- **400** - Regular (body text)

---

## Spacing System

### Padding/Margins
- **0.25rem** (4px) - Tiny gap
- **0.5rem** (8px) - Small gap
- **0.75rem** (12px) - Medium gap
- **1rem** (16px) - Standard gap
- **1.5rem** (24px) - Large gap
- **2rem** (32px) - Extra large gap

### Component Spacing
- Badge padding: `2px 8px`
- Button padding: `0.625rem 1.25rem`
- Card padding: `1.5rem`
- Drawer padding: `1.5rem`
- Table cell padding: `1rem`

---

## Animation Timings

- **150ms** - Hover effects, quick feedback
- **200ms** - Button transitions
- **300ms** - Drawer slide-in, modal fade
- **1500ms** - Loading skeleton animation

---

## Responsive Breakpoints

### Table
- **Desktop**: Full table with all columns
- **Tablet**: Horizontal scroll if needed
- **Mobile**: Consider stacked cards (future)

### Drawer
- **Desktop**: 500px wide
- **Tablet**: 500px wide
- **Mobile**: Full width (future enhancement)

### Role Grid
- **Desktop**: 3-4 columns
- **Tablet**: 2 columns
- **Mobile**: 1 column

---

## Accessibility Features

### Keyboard Navigation
- **Tab**: Move between interactive elements
- **Enter/Space**: Activate buttons, switches
- **Escape**: Close drawer, modal
- **Arrow keys**: Navigate dropdown options

### Screen Readers
- ARIA labels on all interactive elements
- Role attributes (button, switch, dialog)
- Focus trap in modals
- Semantic HTML structure

### Visual
- High contrast colors
- Clear focus indicators
- Large clickable targets (min 44x44px)
- Readable font sizes
- Icon + text labels

---

## Best Practices Used

1. **Compound Components** - Complex components split into smaller, focused pieces
2. **Controlled Components** - All form elements controlled via React state
3. **Error Boundaries** - Graceful error handling (component-level)
4. **Loading States** - Every async operation shows feedback
5. **Optimistic UI** - Instant feedback, confirmed by server
6. **Semantic HTML** - Proper tags for better accessibility
7. **CSS Modules** - Scoped styles prevent conflicts
8. **TypeScript** - Type safety throughout
9. **Custom Hooks** - Reusable logic separation
10. **React Query** - Automatic caching and revalidation

---

This redesign provides a modern, accessible, and user-friendly interface that eliminates manual data entry and provides a professional IAM experience.
