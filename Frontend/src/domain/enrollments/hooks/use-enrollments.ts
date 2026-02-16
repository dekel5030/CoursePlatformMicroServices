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

/**
 * Returns the enrollment ID for the current user in the given course, or null if not enrolled.
 * Uses the first page of "my enrollments" to find a match.
 */
export function useEnrollmentIdByCourseId(courseId: string | undefined): string | null {
  const { data } = useMyEnrollments(1, 100);
  if (!courseId || !data?.items?.length) return null;
  const enrollment = data.items.find((item) => item.courseId === courseId);
  return enrollment?.enrollmentId ?? null;
}
