# Tailwind CSS & Shadcn/ui Migration Guide

## Overview
This guide documents the migration from custom CSS Modules to Tailwind CSS v4 and Shadcn/ui components for the CoursePlatform frontend.

## Current Progress
- **CSS Reduction**: 21% complete (3,293 → 2,598 lines, -695 lines)
- **Target**: 60-70% reduction (~1,000-1,300 final lines)
- **Components Migrated**: Layout components (Navbar, Footer, Breadcrumb, Layout)
- **Build Status**: ✅ Passing

## Architecture

### Tailwind CSS v4
We're using Tailwind CSS v4 with the new CSS-first configuration:
- Configuration in `src/index.css` using `@theme` directive
- PostCSS plugin: `@tailwindcss/postcss`
- No `tailwind.config.js` needed

### Shadcn/ui Components
Created 15+ reusable components in `src/components/ui/`:
- **Primitives**: Button, Input, Badge, Switch, Skeleton, Card, Table
- **Overlays**: Dialog, Sheet (Drawer), Popover, Dropdown Menu
- **Forms**: Select, FormField, Combobox (MultiSelect)
- **Feedback**: Toaster (Sonner)
- **Navigation**: Command (for search/command palette)

### Compatibility Layer
Three wrapper components maintain backward compatibility:
- `Modal` → wraps `Dialog`
- `Drawer` → wraps `Sheet`  
- `MultiSelect` → wraps `Combobox`

## Migration Patterns

### 1. Button Migration
**Old:**
```tsx
import { Button } from "@/components/ui";
<Button variant="filled">Save</Button>
<Button variant="outlined">Cancel</Button>
```

**New:**
```tsx
import { Button } from "@/components/ui";
<Button variant="default">Save</Button>
<Button variant="outline">Cancel</Button>
```

Available variants: `default`, `destructive`, `outline`, `secondary`, `ghost`, `link`

### 2. Badge Migration
**Old:**
```tsx
<Badge variant="success">Active</Badge>
<Badge variant="error">Failed</Badge>
<Badge variant="info">Info</Badge>
```

**New:**
```tsx
<Badge variant="default">Active</Badge>
<Badge variant="destructive">Failed</Badge>
<Badge variant="secondary">Info</Badge>
```

Available variants: `default`, `secondary`, `destructive`, `outline`

### 3. Input with Label Migration
**Old:**
```tsx
import Input from "@/components/ui/Input/Input";
<Input
  label="Email"
  name="email"
  value={email}
  onChange={handleChange}
  error={errors.email}
/>
```

**New:**
```tsx
import { FormField } from "@/components/ui";
<FormField
  label="Email"
  name="email"
  value={email}
  onChange={handleChange}
  error={errors.email}
/>
```

### 4. Modal Migration
Already compatible! The wrapper handles the conversion:
```tsx
import { Modal } from "@/components/ui";
<Modal
  isOpen={isOpen}
  onClose={onClose}
  title="Edit Profile"
  error={error?.message}
>
  {/* Modal content */}
</Modal>
```

### 5. Converting CSS Modules to Tailwind

**Old (CSS Module):**
```css
.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.title {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--text-primary);
}
```

```tsx
import styles from "./Component.module.css";
<div className={styles.container}>
  <div className={styles.header}>
    <h1 className={styles.title}>Title</h1>
  </div>
</div>
```

**New (Tailwind):**
```tsx
<div className="max-w-7xl mx-auto px-8">
  <div className="flex justify-between items-center mb-6">
    <h1 className="text-2xl font-semibold text-foreground">Title</h1>
  </div>
</div>
```

## Common Tailwind Utility Mappings

### Spacing
- `padding: 1rem` → `p-4`
- `padding: 0.5rem 1rem` → `py-2 px-4`
- `margin: 0 auto` → `mx-auto`
- `gap: 1rem` → `gap-4`

### Layout
- `display: flex` → `flex`
- `flex-direction: column` → `flex-col`
- `justify-content: space-between` → `justify-between`
- `align-items: center` → `items-center`

### Typography
- `font-size: 1.5rem` → `text-2xl`
- `font-weight: 600` → `font-semibold`
- `font-weight: 700` → `font-bold`
- `text-align: center` → `text-center`

### Colors (Theme Variables)
- `color: var(--text-primary)` → `text-foreground`
- `color: var(--primary-color)` → `text-primary`
- `background: var(--background-primary)` → `bg-background`
- `border: 1px solid var(--border-color)` → `border border-border`

### Borders & Shadows
- `border-radius: 4px` → `rounded`
- `border-radius: 8px` → `rounded-lg`
- `box-shadow: ...` → `shadow-sm` / `shadow` / `shadow-lg`

### Responsive Design
- `@media (max-width: 768px)` → `md:` prefix
- `@media (max-width: 640px)` → `sm:` prefix
- Mobile-first: Apply mobile styles by default, use breakpoints for larger screens

Example:
```tsx
// Hide on mobile, show on desktop
<nav className="hidden md:block">...</nav>

// Stack on mobile, row on desktop
<div className="flex flex-col md:flex-row gap-4">...</div>
```

## Remaining Work

### Phase 5: Common Components (5 files)
**Files to migrate:**
1. `src/components/common/ProfileMenu/ProfileMenu.tsx` + `.module.css`
   - Migrate to Dropdown Menu component
   - Use Avatar component for profile icon
2. `src/components/common/Dropdown/Dropdown.tsx` + `.module.css`
   - Replace with Shadcn/ui Dropdown Menu
3. `src/components/common/SearchBox/SearchBox.tsx` + `.module.css`
   - Use Input component with search icon
4. `src/components/common/CourseCard/CourseCard.tsx` + `.module.css`
   - Migrate to Card component
5. `src/components/common/EditProfileModal/EditProfileModal.tsx` + `.module.css`
   - Already using Modal/FormField, just remove CSS module

### Phase 6: Feature Components (~10 files)
**auth-management components:**
- `AddPermissionModal.tsx` + `.module.css`
- `AddRoleModal.tsx` + `.module.css`
- `ConfirmationModal.tsx` (no CSS)
- `PermissionMatrix/PermissionMatrix.tsx` + `.module.css`
- `RoleList.tsx` + `.module.css`
- `UserDrawer/UserDrawer.tsx` (uses compatibility Drawer)
- `UserPermissionMatrix/UserPermissionMatrix.tsx` + `.module.css`
- `UserTable/UserTable.tsx` + `.module.css`

**courses, lessons, users features:**
- Similar patterns, migrate case-by-case

### Phase 7: Page Components (~7 files)
- `UsersListPage` + `.module.css`
- `UserManagementPage` + `.module.css`
- `CoursePage` + `.module.css`
- Admin pages (RoleManagementPage, RoleDetailPage)
- Home page components
- Lesson/User profile pages

### Phase 8: Final Cleanup
1. Delete `App.module.css`
2. Remove `ui-deprecated-backup` folder
3. Run full lint check
4. Test all responsive breakpoints
5. Document any custom Tailwind extensions

## Testing Checklist

Before merging:
- [ ] `npm run build` succeeds
- [ ] `npm run lint` passes
- [ ] All pages render without console errors
- [ ] Test responsive behavior (375px, 768px, 1024px, 1920px)
- [ ] Test all interactive components (modals, dropdowns, forms)
- [ ] Verify ARIA attributes are present (Radix provides these)
- [ ] Check color contrast for accessibility

## Helpful Commands

```bash
# Count remaining CSS module files (excluding backup)
find src -name "*.module.css" -not -path "*/ui-deprecated-backup/*" | wc -l

# Count remaining CSS lines (excluding backup)
find src -name "*.module.css" -not -path "*/ui-deprecated-backup/*" -exec wc -l {} + | tail -1

# Find all files importing a specific old component
grep -r "from '@/components/ui/Badge/Badge'" src --include="*.tsx" --include="*.ts"

# Build and watch for errors
npm run build 2>&1 | grep -E "error|Error"
```

## Tips

1. **Incremental Migration**: Migrate one component at a time, test, commit
2. **Utility First**: Use Tailwind utilities directly, avoid creating custom CSS
3. **Component Reuse**: Use Shadcn/ui components for consistency
4. **Mobile First**: Apply base styles for mobile, use `md:`, `lg:` for larger screens
5. **Theme Variables**: Use semantic color names (`text-foreground`, `bg-background`)
6. **cn() Helper**: Use for conditional classes: `cn("base-class", condition && "conditional-class")`

## Resources

- [Tailwind CSS v4 Docs](https://tailwindcss.com/docs)
- [Shadcn/ui Components](https://ui.shadcn.com/docs/components)
- [Radix UI Primitives](https://www.radix-ui.com/primitives)
- [Tailwind CSS Cheat Sheet](https://nerdcave.com/tailwind-cheat-sheet)
