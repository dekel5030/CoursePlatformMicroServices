// Components
export { default as CourseCard } from "./components/CourseCard";
export { default as Catalog } from "./components/Catalog";

// Pages
export { default as CoursePage } from "./pages/CoursePage";
export { default as CourseCatalogPage } from "./pages/CourseCatalogPage";
export { default as AllCoursesPage } from "./pages/AllCoursesPage";

// Hooks - Consolidated data access with centralized query keys
export {
  useFeaturedCourses,
  useAllCourses,
  useCourse,
  useCreateCourse,
  usePatchCourse,
  useDeleteCourse,
  coursesQueryKeys,
} from "./hooks/use-courses";

// API
export * from "./api";

// Types (UI Models - stable interfaces)
export type { CourseModel } from "./types/CourseModel";
export type { Money } from "./types/money";

// Legacy types (deprecated - use CourseModel instead)
export type { Course } from "./types/course";
