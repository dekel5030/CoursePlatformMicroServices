import type { LinkDto } from "@/shared/types/LinkDto";

/**
 * Backend DTO: Matches EnrolledCourseDto from CourseService (pure, no analytics)
 */
export interface EnrolledCourseDtoApi {
  enrollmentId: string;
  courseId: string;
  courseTitle: string | null;
  courseImageUrl: string | null;
  courseSlug: string | null;
  lastAccessedAt: string | null;
  enrolledAt: string;
  status: string | null;
  links: LinkDto[] | null;
}

/**
 * View model: Flattened shape for components (includes progressPercentage from analytics)
 */
export interface EnrolledCourseDto {
  enrollmentId: string;
  courseId: string;
  courseTitle: string | null;
  courseImageUrl: string | null;
  courseSlug: string | null;
  progressPercentage: number;
  lastAccessedAt: string | null;
  enrolledAt: string;
  status: string | null;
  links: LinkDto[] | null;
}
