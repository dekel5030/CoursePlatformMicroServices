import { useQuery } from "@tanstack/react-query";
import {
  fetchMyManagedCourses,
  type ManagedCoursesResponse,
} from "../api/courses-api";
import { coursesQueryKeys } from "../query-keys";

export function useManagedCourses(
  pageNumber: number,
  pageSize: number
): ReturnType<typeof useQuery<ManagedCoursesResponse, Error>> {
  return useQuery<ManagedCoursesResponse, Error>({
    queryKey: coursesQueryKeys.managed(pageNumber, pageSize),
    queryFn: () => fetchMyManagedCourses(pageNumber, pageSize),
  });
}
