# Localization Structure

This document describes the localization setup for the frontend application.

## Overview

The application uses a **feature-based localization structure** where each feature owns and manages its own locale files. This improves ownership, scalability, and maintainability.

## Directory Structure

```
Frontend/
├── src/
│   ├── locales/                    # Global/shared translations
│   │   ├── en/
│   │   │   └── translation.json    # Common translations (navbar, breadcrumbs, common, etc.)
│   │   └── he/
│   │       └── translation.json
│   ├── features/
│   │   ├── courses/
│   │   │   └── locales/            # Course-specific translations
│   │   │       ├── en/
│   │   │       │   └── courses.json
│   │   │       └── he/
│   │   │           └── courses.json
│   │   ├── lessons/
│   │   │   └── locales/            # Lesson-specific translations
│   │   │       ├── en/
│   │   │       │   └── lessons.json
│   │   │       └── he/
│   │   │           └── lessons.json
│   │   └── ...
│   └── i18n.ts                     # i18n configuration
└── public/
    └── locales/                    # DEPRECATED - can be removed
```

## Configuration

The i18n configuration (`src/i18n.ts`) imports locale files directly for better reliability:

```typescript
import translationEN from './locales/en/translation.json';
import translationHE from './locales/he/translation.json';
import coursesEN from './features/courses/locales/en/courses.json';
import coursesHE from './features/courses/locales/he/courses.json';
// ...

const resources = {
  en: {
    translation: translationEN,
    courses: coursesEN,
    lessons: lessonsEN,
  },
  he: {
    translation: translationHE,
    courses: coursesHE,
    lessons: lessonsHE,
  },
};
```

## Usage

### In Components

```tsx
import { useTranslation } from 'react-i18next';

function MyComponent() {
  // Load multiple namespaces
  const { t } = useTranslation(['courses', 'translation']);
  
  return (
    <div>
      {/* Use default namespace (translation) */}
      <h1>{t('common.title')}</h1>
      
      {/* Use specific namespace */}
      <p>{t('courses:catalog.subtitle')}</p>
      
      {/* With interpolation */}
      <span>{t('common.error', { message: 'Something went wrong' })}</span>
      
      {/* With plural support */}
      <span>{t('courses:card.lessonsCount', { count: 5 })}</span>
    </div>
  );
}
```

### Namespaces

- **translation** (default): Common translations used across the app
  - `common.*` - Common UI elements (buttons, labels, etc.)
  - `navbar.*` - Navigation bar items and dropdowns
  - `breadcrumbs.*` - Breadcrumb navigation
  - `admin.*` - Admin dashboard
  - `authManagement.*` - User and role management
  - `modals.*` - Modal dialogs
  - `pages.*` - Shared page elements
  - `landing.*` - Landing page content
  - `footer.*` - Footer links and content

- **courses**: Course-specific translations
  - `courses:catalog.*` - Course catalog page
  - `courses:card.*` - Course card component
  - `courses:detail.*` - Course detail page
  - `courses:addDialog.*` - Add course dialog
  - `courses:actions.*` - Course actions

- **lessons**: Lesson-specific translations
  - `lessons:card.*` - Lesson card component
  - `lessons:detail.*` - Lesson detail page
  - `lessons:addDialog.*` - Add lesson dialog
  - `lessons:actions.*` - Lesson actions

## Supported Languages

- **English (en)** - Default language
- **Hebrew (he)** - RTL support included

## Key Features

1. **Feature-based structure**: Each feature owns its translations
2. **Direct imports**: Locale files are imported directly (no HTTP backend)
3. **Namespacing**: Keys are organized by feature (e.g., `courses.*`, `lessons.*`)
4. **Plural support**: Uses i18next plural handling (e.g., `lessonsCount`, `lessonsCount_plural`)
5. **RTL support**: Automatic direction switching for Hebrew
6. **Interpolation**: Dynamic values in translations (e.g., `{{message}}`, `{{count}}`)
7. **Fallback handling**: Falls back to English if translation is missing

## Adding New Translations

### 1. Add to existing namespace

Edit the appropriate JSON file:
- For global translations: `src/locales/{lang}/translation.json`
- For feature translations: `src/features/{feature}/locales/{lang}/{feature}.json`

### 2. Add new feature namespace

1. Create locale directory structure:
   ```
   src/features/{feature}/locales/
   ├── en/{feature}.json
   └── he/{feature}.json
   ```

2. Update `src/i18n.ts`:
   ```typescript
   import featureEN from './features/{feature}/locales/en/{feature}.json';
   import featureHE from './features/{feature}/locales/he/{feature}.json';
   
   const resources = {
     en: {
       // ...
       {feature}: featureEN,
     },
     he: {
       // ...
       {feature}: featureHE,
     },
   };
   ```

3. Add namespace to config:
   ```typescript
   ns: ['translation', 'courses', 'lessons', '{feature}'],
   ```

## Best Practices

1. **Always use translation keys** - Never hardcode user-facing strings
2. **Provide both languages** - Always add translations for EN and HE
3. **Use descriptive keys** - Keys should clearly indicate what they translate
4. **Group related keys** - Use nested objects to organize related translations
5. **Use plurals correctly** - Use `_plural` suffix for plural forms
6. **Keep keys consistent** - Follow existing naming conventions
7. **Test both languages** - Verify translations work in both EN and HE modes
8. **Handle RTL** - Ensure layout works properly in Hebrew (RTL) mode

## Migration from Old Structure

The old locale files in `public/locales/` are deprecated and can be removed. All translations have been migrated to the new feature-based structure in `src/`.

## Troubleshooting

### Translation keys showing in UI

- Check that the key exists in the locale file
- Verify the namespace is loaded in `useTranslation()`
- Ensure the locale file is imported in `i18n.ts`

### Missing translations

- Add the key to both EN and HE locale files
- Restart the dev server after adding new locale files

### TypeScript errors

- Ensure `resolveJsonModule: true` is set in `tsconfig.app.json`
- Check that JSON files are properly formatted
