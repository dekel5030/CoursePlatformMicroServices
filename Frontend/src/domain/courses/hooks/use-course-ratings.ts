import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  fetchCourseRatings,
  fetchCourseRatingsByUrl,
  createCourseRating,
  patchCourseRating,
  deleteCourseRating,
} from "../api/course-ratings-api";
import type {
  CreateCourseRatingRequest,
  UpdateCourseRatingRequest,
} from "../types";
import { coursesQueryKeys } from "../query-keys";

export function useCourseRatings(
  courseId: string,
  ratingsUrl: string | undefined,
  options?: { pageNumber?: number; pageSize?: number; pageUrl?: string }
) {
  const pageNumber = options?.pageNumber ?? 1;
  const pageSize = options?.pageSize ?? 10;
  const pageUrl = options?.pageUrl;
  const pageKey = pageUrl ?? pageNumber;

  return useQuery({
    queryKey: coursesQueryKeys.ratings(courseId, pageKey, pageSize),
    queryFn: () =>
      pageUrl
        ? fetchCourseRatingsByUrl(pageUrl)
        : fetchCourseRatings(ratingsUrl!, pageNumber, pageSize),
    enabled: !!courseId && !!(pageUrl || ratingsUrl),
  });
}

export function useCreateRating(courseId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      url,
      request,
    }: {
      url: string;
      request: CreateCourseRatingRequest;
    }) => createCourseRating(url, request),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [...coursesQueryKeys.all, courseId, "ratings"],
      });
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.detail(courseId),
      });
    },
  });
}

export function usePatchRating(courseId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      url,
      request,
    }: {
      url: string;
      request: UpdateCourseRatingRequest;
    }) => patchCourseRating(url, request),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [...coursesQueryKeys.all, courseId, "ratings"],
      });
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.detail(courseId),
      });
    },
  });
}

export function useDeleteRating(courseId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (url: string) => deleteCourseRating(url),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: [...coursesQueryKeys.all, courseId, "ratings"],
      });
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.detail(courseId),
      });
    },
  });
}
