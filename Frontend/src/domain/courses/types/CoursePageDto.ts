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
 * API CourseDto: the course object in CoursePageDto
 * Matches Courses.Application.Courses.Dtos.CourseDto
 */
export interface CourseDtoApi {
  id: string;
  title: string | null;
  description: string | null;
  status: "Draft" | "Published" | "Deleted";
  price: Money;
  enrollmentCount: number;
  lessonsCount: number;
  totalDuration: string;
  updatedAtUtc: string;
  imageUrls: string[] | null;
  tags: string[] | null;
  instructorId: string;
  categoryId: string;
  moduleIds: string[] | null;
  links: LinkDto[] | null;
}

/**
 * API ModuleDto: module in CoursePageDto (has lessonIds, not lessons)
 * Matches Courses.Application.Courses.Dtos.ModuleDto
 */
export interface ModuleDtoApi {
  id: string;
  title: string | null;
  index: number;
  lessonCount: number;
  duration: string;
  lessonIds: string[] | null;
  links: LinkDto[] | null;
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
 * API CoursePageDto: flat response from GET /courses/{id}
 * Matches Courses.Application.Courses.Dtos.CoursePageDto
 */
export interface CoursePageDto {
  course: CourseDtoApi;
  modules: Record<string, ModuleDtoApi> | null;
  lessons: Record<string, LessonDtoApi> | null;
  instructors: Record<string, UserDtoApi> | null;
  categories: Record<string, CategoryDto> | null;
}
