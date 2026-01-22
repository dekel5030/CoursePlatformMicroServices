import type { LinkDto } from "@/types/LinkDto";
import type { LessonModel } from "@/features/lessons/types/LessonModel";

/**
 * UI Model: Represents a module with its lessons
 */
export interface ModuleModel {
  id: string;
  title: string;
  order: number;
  lessonCount: number;
  duration: string;
  lessons: LessonModel[];
  links: LinkDto[];
}
