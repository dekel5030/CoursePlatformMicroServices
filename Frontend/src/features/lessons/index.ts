// Components
export { default as LessonCard } from "./components/LessonCard";

// Pages
export { default as LessonPage } from "./pages/LessonPage";

// Hooks - Consolidated data access with centralized query keys
export {
  useLesson,
  useCreateLesson,
  usePatchLesson,
  lessonsQueryKeys,
} from "./hooks/use-lessons";

// API
export * from "./api";

// Constants
export { LessonRoutes, LessonResources } from "./constants";

// Types (UI Models - stable interfaces)
export type { LessonModel } from "./types/LessonModel";

// Legacy types (deprecated - use LessonModel instead)
export type { Lesson as LessonType } from "./types/Lesson";
