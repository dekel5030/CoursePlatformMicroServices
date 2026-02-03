import { axiosClient } from "@/app/axios";
import type { EnrolledCourseCollectionDto } from "../types";

export async function fetchMyEnrollments(
  pageNumber = 1,
  pageSize = 10
): Promise<EnrolledCourseCollectionDto> {
  const response = await axiosClient.get<EnrolledCourseCollectionDto>(
    "/users/me/enrollments",
    { params: { pageNumber, pageSize } }
  );
  return response.data;
}
