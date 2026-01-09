/**
 * UI Model: Stable interface for UI components
 * Decoupled from backend API schema
 */
export interface LessonModel {
  courseId: string;
  lessonId: string;
  title: string;
  description: string;  // Non-nullable - mapper ensures a value
  videoUrl: string | null;
  thumbnailImage: string | null;
  isPreview: boolean;
  order: number;
  duration: string | null;
}
