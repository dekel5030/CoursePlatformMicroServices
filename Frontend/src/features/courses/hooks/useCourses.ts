import { useQuery } from "@tanstack/react-query";
import { fetchFeaturedCourses } from "@/features/courses/api";
import type { Course } from "../types";

export function useCourses() {
  return useQuery<Course[], Error>({
    queryKey: ["courses", "featured"],
    queryFn: fetchFeaturedCourses,
  });
}
