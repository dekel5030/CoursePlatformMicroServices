import type { IdValueObject } from "./idValueObject";

export interface Lesson {
  id: IdValueObject;
  title: string;
  description?: string | null;
  videoUrl?: string | null;
  thumbnailImage?: string | null;
  isPreview: boolean;
  order: number;
  duration?: string | null;
  courseId: string;
}
