import type { LinkDto } from "@/shared/types/LinkDto";

/**
 * UI Model: Stable interface for UI components
 * Decoupled from backend API schema
 */
export interface LessonModel {
  courseId: string;
  lessonId: string;
  title: string;
  description: string;
  videoUrl: string | null;
  transcriptUrl: string | null;
  thumbnailImage: string | null;
  isPreview: boolean;
  order: number;
  duration: string | null;
  links?: LinkDto[];
}
