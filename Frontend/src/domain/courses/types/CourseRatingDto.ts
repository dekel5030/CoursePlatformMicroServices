import type { LinkDto } from "@/shared/types/LinkDto";

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
  links: LinkDto[];
}

export interface CourseRatingCollectionDto {
  items: CourseRatingDto[];
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages?: number;
  links: LinkDto[];
}

export interface CreateCourseRatingRequest {
  score: number;
  comment?: string;
}

export interface UpdateCourseRatingRequest {
  score?: number;
  comment?: string;
}
