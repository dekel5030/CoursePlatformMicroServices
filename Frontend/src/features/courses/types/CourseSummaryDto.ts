import type { LinkDto } from "@/types/LinkDto";
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

export enum DifficultyLevel {
  Beginner = 0,
  Intermediate = 1,
  Advanced = 2,
  Expert = 3,
}

export enum CourseStatus {
  Draft = "Draft",
  Published = "Published",
  Deleted = "Deleted",
}

/**
 * Backend DTO: Matches CourseSummaryDto from CourseService
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
  originalPrice: Money | null;
  badges: string[];
  averageRating: number;
  reviewsCount: number;
  thumbnailUrl: string | null;
  lessonsCount: number;
  duration: string;
  difficulty: DifficultyLevel | string; // API returns string, but we convert to enum
  enrollmentCount: number;
  courseViews: number;
  updatedAtUtc: string;
  status: CourseStatus;
  links: LinkDto[];
}
