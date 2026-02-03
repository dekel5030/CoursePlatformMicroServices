import type { LinkDto } from "@/shared/types/LinkDto";
import type { EnrolledCourseWithAnalyticsDto } from "./EnrolledCourseWithAnalyticsDto";

/**
 * API response: Matches EnrolledCourseCollectionDto from CourseService
 */
export interface EnrolledCourseCollectionDto {
  items: EnrolledCourseWithAnalyticsDto[] | null;
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages?: number;
  links: LinkDto[] | null;
}
