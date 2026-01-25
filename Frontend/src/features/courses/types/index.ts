// Legacy interface (deprecated - use CourseModel instead)
export type { Course } from "./course";
export type { Money } from "./money";

// Backend DTOs
export type {
  CourseSummaryDto,
  InstructorDto,
  CategoryDto,
} from "./CourseSummaryDto";
export { DifficultyLevel, CourseStatus } from "./CourseSummaryDto";
export type { CourseDetailsDto } from "./CourseDetailsDto";
export type { ModuleDto, ModuleLessonDto } from "./ModuleDto";
export type { CreateCourseRequestDto } from "./CreateCourseRequestDto";
export type { UpdateCourseRequestDto } from "./UpdateCourseRequestDto";

// UI Model
export type { CourseModel } from "./CourseModel";
export {
  DifficultyLevel as CourseDifficultyLevel,
  CourseStatus as CourseStatusEnum,
} from "./CourseModel";
export type { ModuleModel } from "./ModuleModel";
