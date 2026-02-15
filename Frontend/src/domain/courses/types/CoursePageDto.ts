import type { Money } from "./money";
import type { CategoryDto } from "./CourseSummaryDto";
import type {
  CoursePageCourseLinks,
  CoursePageLessonLinks,
} from "./links";

/**
 * API UserDto: instructor record in CoursePageDto
 */
export interface UserDtoApi {
  id: string;
  firstName: string | null;
  lastName: string | null;
  avatarUrl: string | null;
}

export interface CourseAnalyticsDtoApi {
  enrollmentCount: number;
  lessonsCount: number;
  totalDuration: string;
  averageRating?: number;
  reviewsCount?: number;
  viewCount?: number;
}

/** Course page: course.data */
export interface CoursePageCourseData {
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
}

/** Course page: course (data + links) */
export interface CoursePageCourseDto {
  data: CoursePageCourseData;
  links: CoursePageCourseLinks;
}

/** Course page: module (data + links); links may be empty object */
export interface CoursePageModuleData {
  id: string;
  title: string | null;
  lessonCount: number;
  totalDuration: string;
}

export interface CoursePageModuleLinks {
  // empty in API
}

export interface CoursePageModuleDto {
  data: CoursePageModuleData;
  links?: CoursePageModuleLinks;
}

/** Course page: lesson (data + links) */
export interface CoursePageLessonData {
  id: string;
  title: string | null;
  index: number;
  duration: string;
  thumbnailUrl: string | null;
  access: "Private" | "Public";
  moduleId: string;
  courseId: string;
  description: string | null;
  videoUrl: string | null;
  transcriptUrl: string | null;
}

export interface CoursePageLessonDto {
  data: CoursePageLessonData;
  links: CoursePageLessonLinks;
}

export interface CourseStructureDtoApi {
  moduleIds: string[];
  moduleLessonIds: Record<string, string[]>;
}

/**
 * API CoursePageDto: GET /courses/{id} response (strongly-typed links).
 */
export interface CoursePageDto {
  course: CoursePageCourseDto;
  analytics?: CourseAnalyticsDtoApi;
  structure: CourseStructureDtoApi;
  modules: Record<string, CoursePageModuleDto> | null;
  lessons: Record<string, CoursePageLessonDto> | null;
  instructors?: Record<string, UserDtoApi> | null;
  categories: Record<string, CategoryDto> | null;
}
