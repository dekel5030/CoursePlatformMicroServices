import type { LinkDto } from "@/shared/types/LinkDto";
import type { Money } from "./money";
import type { CategoryDto } from "./CourseSummaryDto";

/**
 * API UserDto: instructor record in CoursePageDto
 * Matches Courses.Application.Courses.Dtos.UserDto
 */
export interface UserDtoApi {
  id: string;
  firstName: string | null;
  lastName: string | null;
  avatarUrl: string | null;
}

/**
 * API CourseAnalyticsDto: analytics for course in CoursePageDto
 * Matches Courses.Application.Courses.Dtos.CourseAnalyticsDto
 */
export interface CourseAnalyticsDtoApi {
  enrollmentCount: number;
  lessonsCount: number;
  totalDuration: string;
  averageRating?: number;
  reviewsCount?: number;
  viewCount?: number;
}

/**
 * API CourseDto: the course object in CoursePageDto
 * Matches Courses.Application.Courses.Dtos.CourseDto
 */
export interface CourseDtoApi {
  id: string;
  title: string | null;
  description: string | null;
  status: "Draft" | "Published" | "Deleted";
  price: Money;
  updatedAtUtc: string;
  imageUrls: string[] | null;
  tags: string[] | null;
  instructorId: string;
  categoryId: string;
  links: LinkDto[] | null;
}

/**
 * API ModuleDto: module in CoursePageDto (pure aggregate, no structure)
 * Matches Courses.Application.Courses.Dtos.ModuleDto
 */
export interface ModuleDtoApi {
  id: string;
  title: string | null;
  links: LinkDto[] | null;
}

/**
 * API ModuleAnalyticsDto: analytics for module
 * Matches Courses.Application.Modules.Dtos.ModuleAnalyticsDto
 */
export interface ModuleAnalyticsDtoApi {
  lessonCount: number;
  duration: string;
}

/**
 * API ModuleWithAnalyticsDto: module with analytics in CoursePageDto
 * Matches Courses.Application.Modules.Dtos.ModuleWithAnalyticsDto
 */
export interface ModuleWithAnalyticsDtoApi {
  module: ModuleDtoApi;
  analytics: ModuleAnalyticsDtoApi;
}

/**
 * API LessonDto: lesson in CoursePageDto
 * Matches Courses.Application.Courses.Dtos.LessonDto
 */
export interface LessonDtoApi {
  id: string;
  title: string | null;
  index: number;
  duration: string;
  thumbnailUrl: string | null;
  access: "Private" | "Public";
  moduleId: string | null;
  courseId: string | null;
  courseName: string | null;
  description: string | null;
  videoUrl: string | null;
  transcriptUrl: string | null;
  links: LinkDto[] | null;
}

/**
 * API CourseStructureDto: structure (order) of modules and lessons
 * Matches Courses.Application.Courses.Dtos.CourseStructureDto
 */
export interface CourseStructureDtoApi {
  moduleIds: string[];
  moduleLessonIds: Record<string, string[]>;
}

/**
 * API CoursePageDto: flat response from GET /courses/{id} or GET /manage/courses/{id} (ManagedCoursePageDto).
 * For managed view, analytics and instructors are omitted (instructor is the current user).
 */
export interface CoursePageDto {
  course: CourseDtoApi;
  /** Omitted in ManagedCoursePageDto (GET /manage/courses/{id}) */
  analytics?: CourseAnalyticsDtoApi;
  structure: CourseStructureDtoApi;
  modules: Record<string, ModuleWithAnalyticsDtoApi> | null;
  lessons: Record<string, LessonDtoApi> | null;
  /** Omitted in ManagedCoursePageDto (GET /manage/courses/{id}) */
  instructors?: Record<string, UserDtoApi> | null;
  categories: Record<string, CategoryDto> | null;
}
