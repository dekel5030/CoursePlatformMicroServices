// Components
export { default as Lesson } from "./components/LessonCard";

// Pages
export { default as LessonPage } from "./pages/LessonPage";

// Hooks - Consolidated data access with centralized query keys
export { useLesson, useCreateLesson, lessonsQueryKeys } from "./hooks/use-lessons";

// API
export * from "./api";

// Types
export type { Lesson as LessonType } from "./types/Lesson";
