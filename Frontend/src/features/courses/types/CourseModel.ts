import type { Money } from "./money";
import type { LessonModel } from "@/features/lessons/types/LessonModel";

/**
 * UI Model: Stable interface for UI components
 * Decoupled from backend API schema
 */
export interface CourseModel {
  id: string;
  title: string;
  description: string;
  imageUrl: string | null;
  instructorName: string | null;  // Backend sends name, not ID
  isPublished: boolean;
  price: Money;
  lessons?: LessonModel[];  // Optional - summary doesn't include lessons
  updatedAtUtc?: string;  // Optional - summary doesn't include timestamp
}
