import type { LinkDto } from "@/shared/types/LinkDto";

/**
 * Backend DTO: Matches LessonSummaryDto from CourseService
 * Used for network layer communication
 */
export interface LessonSummaryDto {
  courseId?: string;
  lessonId: string;
  title: string;
  description?: string;
  index: number;
  duration: string | null;
  isPreview?: boolean;
  thumbnailUrl: string | null;
  access?: string;
  links: LinkDto[];
}
