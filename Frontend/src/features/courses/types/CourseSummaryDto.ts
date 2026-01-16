import type { LinkDto } from "@/types/LinkDto";

/**
 * Backend DTO: Matches CourseSummaryDto from CourseService
 * Used for network layer communication
 */
export interface CourseSummaryDto {
  id: string;
  title: string;
  instructorName: string | null;
  price: number;
  currency: string;
  thumbnailUrl: string | null;
  lessonsCount: number;
  enrollmentCount: number;
  links: LinkDto[];
}
