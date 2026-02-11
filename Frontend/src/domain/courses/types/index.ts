// UI Models
export type { CourseModel } from "./CourseModel";
export { DifficultyLevel, CourseStatus, type DifficultyLevel as DifficultyLevelType, type CourseStatus as CourseStatusType } from "./CourseModel";
export type { ModuleModel } from "./ModuleModel";
export type { Money } from "./money";

// Constants
export { ModuleRels } from "./ModuleRels";
export { CourseRatingRels } from "./CourseRatingRels";

// Backend DTOs
export type {
  CourseSummaryDto,
  CourseSummaryAnalyticsDto,
  CourseSummaryWithAnalyticsDto,
  InstructorDto,
  CategoryDto,
} from "./CourseSummaryDto";
export { DifficultyLevel as DifficultyLevelDto, CourseStatus as CourseStatusDto, type DifficultyLevel as DifficultyLevelDtoType, type CourseStatus as CourseStatusDtoType } from "./CourseSummaryDto";
export type { ModuleDto, ModuleLessonDto } from "./ModuleDto";
export type { CreateCourseRequestDto } from "./CreateCourseRequestDto";
export type { UpdateCourseRequestDto } from "./UpdateCourseRequestDto";
export type {
  CourseRatingDto,
  CourseRatingCollectionDto,
  CourseRatingUserDto,
  CreateCourseRatingRequest,
  UpdateCourseRatingRequest,
} from "./CourseRatingDto";
export type {
  ManagedCourseSummaryDto,
  ManagedCourseStatsDto,
} from "./ManagedCourseSummaryDto";
export type {
  ManagedCoursePageDto,
  ManagedModuleDtoApi,
  ManagedModuleStatsDto,
} from "./ManagedCoursePageDto";
export type {
  CourseDetailedAnalyticsDto,
  CourseViewerDto,
  ModuleAnalyticsSummaryDto,
  EnrollmentCountByDayDto,
} from "./CourseDetailedAnalyticsDto";