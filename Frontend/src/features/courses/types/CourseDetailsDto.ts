import type { LessonSummaryDto } from "@/features/lessons/types/LessonSummaryDto";
import type { LinkDto } from "@/types/LinkDto";

/**
 * Backend DTO: Matches CourseDetailsDto from CourseService
 * Used for network layer communication
 */
export interface CourseDetailsDto {
  id: string;
  title: string;
  description: string;
  instructorName: string | null;
  price: number;
  currency: string;
  enrollmentCount: number;
  updatedAtUtc: string;
  imageUrls: string[];
  lessons: LessonSummaryDto[];
  links: LinkDto[];
}
