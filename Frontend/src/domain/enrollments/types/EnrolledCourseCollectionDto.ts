import type { LinkDto } from "@/shared/types/LinkDto";
import type { EnrolledCourseDto } from "./EnrolledCourseDto";

export interface EnrolledCourseCollectionDto {
  items: EnrolledCourseDto[] | null;
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages?: number;
  links: LinkDto[] | null;
}
