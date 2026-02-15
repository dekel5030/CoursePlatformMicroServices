import type { CategoryDto } from "./CourseSummaryDto";
import type { CourseStructureDtoApi } from "./CoursePageDto";
import type { Money } from "./money";
import type {
  ManagedCourseLinks,
  ManagedModuleLinks,
  ManagedLessonLinks,
} from "./links";

/** Managed course page: course.data (CourseCoreDto) */
export interface CourseCoreDto {
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

export interface ManagedCoursePageCourseDto {
  data: CourseCoreDto;
  links: ManagedCourseLinks;
}

/** Managed module: data */
export interface ModuleCoreDto {
  id: string;
  title: string | null;
  lessonCount: number;
  duration: string;
}

export interface ManagedCoursePageModuleDto {
  data: ModuleCoreDto;
  links: ManagedModuleLinks;
}

/** Managed lesson: data (LessonCoreDto) */
export interface LessonCoreDto {
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

export interface ManagedCoursePageLessonDto {
  data: LessonCoreDto;
  links: ManagedLessonLinks;
}

/**
 * Backend DTO: GET /manage/courses/{id} (strongly-typed links).
 */
export interface ManagedCoursePageDto {
  course: ManagedCoursePageCourseDto;
  structure: CourseStructureDtoApi;
  modules: Record<string, ManagedCoursePageModuleDto> | null;
  lessons: Record<string, ManagedCoursePageLessonDto> | null;
  categories: Record<string, CategoryDto> | null;
}
