import type { LinkDto } from "@/shared/types/LinkDto";
import type { CourseRatingItemLinks } from "./links";

export interface CourseRatingUserDto {
  id: string;
  firstName: string;
  lastName: string;
  avatarUrl: string | null;
}

export interface CourseRatingDto {
  id: string;
  courseId: string;
  user: CourseRatingUserDto;
  rating: number;
  comment: string;
  createdAt: string;
  updatedAt: string | null;
  /** Legacy array or strongly-typed record (update, delete) */
  links: LinkDto[] | CourseRatingItemLinks;
}

export interface CourseRatingCollectionDto {
  items: CourseRatingDto[];
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages?: number;
  links: LinkDto[] | import("./links").GetCourseRatingsCollectionLinks;
}

export interface CreateCourseRatingRequest {
  score: number;
  comment?: string;
}

export interface UpdateCourseRatingRequest {
  score?: number;
  comment?: string;
}
