import type { LinkDto } from "@/shared/types/LinkDto";
import type { Money } from "./money";

export interface InstructorDto {
  id: string;
  fullName: string | null;
  avatarUrl: string | null;
}

export interface CategoryDto {
  id: string;
  name: string | null;
  slug: string | null;
}

export const DifficultyLevel = {
  Beginner: 0,
  Intermediate: 1,
  Advanced: 2,
  Expert: 3,
} as const;

export type DifficultyLevel = typeof DifficultyLevel[keyof typeof DifficultyLevel];

export const CourseStatus = {
  Draft: "Draft",
  Published: "Published",
  Deleted: "Deleted",
} as const;

export type CourseStatus = typeof CourseStatus[keyof typeof CourseStatus];

/**
 * Backend DTO: Matches CourseSummaryDto from CourseService (pure aggregate)
 * Used for network layer communication
 */
export interface CourseSummaryDto {
  id: string;
  title: string;
  shortDescription: string;
  slug: string;
  instructor: InstructorDto;
  category: CategoryDto;
  price: Money;
  difficulty: DifficultyLevel | string; // API returns string, but we convert to enum
  thumbnailUrl: string | null;
  updatedAtUtc: string;
  status: CourseStatus;
  links: LinkDto[];
}

/**
 * Backend DTO: Matches CourseSummaryAnalyticsDto from CourseService
 * Computed/derived fields only
 */
export interface CourseSummaryAnalyticsDto {
  lessonsCount: number;
  duration: string;
  enrollmentCount: number;
  averageRating: number;
  reviewsCount: number;
  courseViews: number;
}

/**
 * Backend DTO: Matches CourseSummaryWithAnalyticsDto from CourseService
 */
export interface CourseSummaryWithAnalyticsDto {
  course: CourseSummaryDto;
  analytics: CourseSummaryAnalyticsDto;
}
