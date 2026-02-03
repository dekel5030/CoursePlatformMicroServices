import type { LinkDto } from "@/shared/types/LinkDto";

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
