import { useQuery } from "@tanstack/react-query";
import { fetchMyEnrollments } from "../api";
import { enrollmentsQueryKeys } from "../query-keys";
import type { EnrolledCourseCollectionDto } from "../types";

export function useMyEnrollments(
  pageNumber: number,
  pageSize: number
): ReturnType<typeof useQuery<EnrolledCourseCollectionDto, Error>> {
  return useQuery<EnrolledCourseCollectionDto, Error>({
    queryKey: enrollmentsQueryKeys.myEnrollments(pageNumber, pageSize),
    queryFn: () => fetchMyEnrollments(pageNumber, pageSize),
  });
}
