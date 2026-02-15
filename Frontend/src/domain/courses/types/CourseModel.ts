import type { Money } from "./money";
import type { LinksRecord } from "@/shared/types/LinkRecord";
// Temporary import - will be fixed when lessons domain is migrated
import type { LessonModel } from "@/domain/lessons/types/LessonModel";
import type { ModuleModel } from "./ModuleModel";

export const DifficultyLevel = {
  Beginner: 0,
  Intermediate: 1,
  Advanced: 2,
  Expert: 3,
} as const;

export type DifficultyLevel = typeof DifficultyLevel[keyof typeof DifficultyLevel];

export const CourseStatus = {
  Draft: "Draft",
  Published: "Published",
  Deleted: "Deleted",
} as const;

export type CourseStatus = typeof CourseStatus[keyof typeof CourseStatus];

/**
 * UI Model: Stable interface for UI components
 * Decoupled from backend API schema
 */
export interface CourseModel {
  id: string;
  title: string;
  description: string;
  shortDescription?: string;
  slug?: string;
  imageUrl: string | null;
  instructorId?: string;
  instructorName: string | null;
  instructorAvatarUrl: string | null;
  isPublished: boolean;
  status?: CourseStatus;
  price: Money;
  originalPrice?: Money | null;
  badges?: string[];
  averageRating?: number;
  reviewsCount?: number;
  difficulty?: DifficultyLevel;
  courseViews?: number;
  lessons?: LessonModel[];
  modules?: ModuleModel[];
  lessonCount?: number;
  enrollmentCount?: number;
  totalDuration?: string;
  updatedAtUtc?: string;
  categoryName?: string;
  categoryId?: string;
  categorySlug?: string;
  tags?: string[];
  /** Strongly-typed links (camelCase keys: partialUpdate, delete, manage, etc.) */
  links?: LinksRecord;
}
