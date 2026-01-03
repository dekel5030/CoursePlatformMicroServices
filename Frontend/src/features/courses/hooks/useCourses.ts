import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { fetchFeaturedCourses, createCourse, type CreateCourseRequest } from "@/features/courses/api";
import type { Course } from "../types";

export function useCourses() {
  return useQuery<Course[], Error>({
    queryKey: ["courses", "featured"],
    queryFn: fetchFeaturedCourses,
  });
}

export function useCreateCourse() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateCourseRequest) => createCourse(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["courses"] });
    },
  });
}
