import type { Money } from "./money";
import type { LessonModel } from "@/features/lessons/types/LessonModel";
import type { ModuleModel } from "./ModuleModel";
import type { LinkDto } from "@/types/LinkDto";

/**
 * UI Model: Stable interface for UI components
 * Decoupled from backend API schema
 */
export interface CourseModel {
  id: string;
  title: string;
  description: string;
  imageUrl: string | null;
  instructorName: string | null;
  instructorAvatarUrl: string | null;
  isPublished: boolean;
  price: Money;
  lessons?: LessonModel[];
  modules?: ModuleModel[];
  lessonCount?: number;
  enrollmentCount?: number;
  totalDuration?: string;
  updatedAtUtc?: string;
  categoryName?: string;
  categoryId?: string;
  tags?: string[];
  links?: LinkDto[];
}
