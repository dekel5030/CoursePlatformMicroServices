import { axiosClient } from "@/app/axios";
import type { EnrolledCourseCollectionDto, EnrolledCourseDto } from "../types";
import { mapEnrolledCourseWithAnalyticsToDto } from "../mappers/enrolled-course-mapper";

export interface EnrolledCourseCollectionViewDto {
  items: EnrolledCourseDto[] | null;
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages?: number;
  links: import("@/shared/types/LinkDto").LinkDto[] | null;
}

export async function fetchMyEnrollments(
  pageNumber = 1,
  pageSize = 10
): Promise<EnrolledCourseCollectionViewDto> {
  const response = await axiosClient.get<EnrolledCourseCollectionDto>(
    "/users/me/courses/enrolled",
    { params: { pageNumber, pageSize } }
  );
  const data = response.data;
  return {
    ...data,
    items: data.items?.map(mapEnrolledCourseWithAnalyticsToDto) ?? null,
  };
}
