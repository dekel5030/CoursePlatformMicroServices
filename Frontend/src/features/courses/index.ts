// Components
export { default as CourseCard } from "./components/CourseCard";
export { default as Catalog } from "./components/Catalog";

// Pages
export { default as CoursePage } from "./pages/CoursePage";
export { default as CourseCatalogPage } from "./pages/CourseCatalogPage";

// Hooks - Consolidated data access with centralized query keys
export {
  useFeaturedCourses,
  useCourse,
  useCreateCourse,
  usePatchCourse,
  coursesQueryKeys,
} from "./hooks/use-courses";

// API
export * from "./api";

// Types
export type { Course } from "./types/course";
export type { Money } from "./types/money";
