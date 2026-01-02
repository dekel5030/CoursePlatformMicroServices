export interface Lesson {
  id: string;
  title: string;
  description?: string | null;
  videoUrl?: string | null;
  thumbnailImage?: string | null;
  isPreview: boolean;
  order: number;
  duration?: string | null;
  courseId: string;
}
