import type { LinkDto } from "@/types/LinkDto";

/**
 * Backend DTO: Represents a lesson within a module
 */
export interface ModuleLessonDto {
  lessonId: string;
  title: string;
  index: number;
  duration: string;
  thumbnailUrl: string | null;
  access: string;
  links: LinkDto[];
}

/**
 * Backend DTO: Represents a course module with its lessons
 */
export interface ModuleDto {
  id: string;
  title: string;
  index: number;
  lessonCount: number;
  duration: string;
  lessons: ModuleLessonDto[];
  links: LinkDto[];
}
