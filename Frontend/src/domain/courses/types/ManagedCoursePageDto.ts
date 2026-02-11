import type { CategoryDto } from "./CourseSummaryDto";
import type { CourseDtoApi, CourseStructureDtoApi, LessonDtoApi, ModuleDtoApi } from "./CoursePageDto";

/**
 * Backend DTO: Matches ManagedCoursePageDto from CourseService
 * Course page for instructor management view - no analytics, no instructors
 */
export interface ManagedCoursePageDto {
  course: CourseDtoApi;
  structure: CourseStructureDtoApi;
  modules: Record<string, ManagedModuleDtoApi> | null;
  lessons: Record<string, LessonDtoApi> | null;
  categories: Record<string, CategoryDto> | null;
}

/**
 * Backend DTO: Matches ManagedModuleDto from CourseService
 * Module with computed stats (no analytics from read models)
 */
export interface ManagedModuleDtoApi {
  module: ModuleDtoApi;
  stats: ManagedModuleStatsDto;
}

/**
 * Computed statistics for managed modules
 */
export interface ManagedModuleStatsDto {
  lessonCount: number;
  duration: string;
}
