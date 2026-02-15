import type { LinksRecord } from "@/shared/types/LinkRecord";
// Temporary import - will be fixed when lessons domain is migrated
import type { LessonModel } from "@/domain/lessons/types/LessonModel";

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
  /** Strongly-typed links (createLesson, partialUpdate, delete, changePosition) */
  links: LinksRecord;
}
