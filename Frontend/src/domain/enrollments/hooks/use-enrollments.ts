import { useQuery } from "@tanstack/react-query";
import {
  fetchMyEnrollments,
  type EnrolledCourseCollectionViewDto,
} from "../api";
import { enrollmentsQueryKeys } from "../query-keys";

export function useMyEnrollments(
  pageNumber: number,
  pageSize: number
): ReturnType<typeof useQuery<EnrolledCourseCollectionViewDto, Error>> {
  return useQuery<EnrolledCourseCollectionViewDto, Error>({
    queryKey: enrollmentsQueryKeys.myEnrollments(pageNumber, pageSize),
    queryFn: () => fetchMyEnrollments(pageNumber, pageSize),
  });
}
