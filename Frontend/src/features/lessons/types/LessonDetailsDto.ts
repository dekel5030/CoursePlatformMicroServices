/**
 * Backend DTO: Matches LessonDetailsDto from CourseService
 * Used for network layer communication
 */
export interface LessonDetailsDto {
  courseId: string;
  lessonId: string;
  title: string;
  description: string;
  index: number;
  duration: string | null;
  isPreview: boolean;
  thumbnailUrl: string | null;
  videoUrl: string | null;
}
