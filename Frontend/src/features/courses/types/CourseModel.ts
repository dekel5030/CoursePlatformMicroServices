import type { Money } from "./money";
import type { LessonModel } from "@/features/lessons/types/LessonModel";
import type { ModuleModel } from "./ModuleModel";
import type { LinkDto } from "@/types/LinkDto";

export enum DifficultyLevel {
  Beginner = 0,
  Intermediate = 1,
  Advanced = 2,
  Expert = 3,
}

export enum CourseStatus {
  Draft = "Draft",
  Published = "Published",
  Deleted = "Deleted",
}

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
  links?: LinkDto[];
}
