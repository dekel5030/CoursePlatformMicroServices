import { useQuery } from "@tanstack/react-query";
import { fetchFeaturedCourses } from "@/features/courses/api";
import type { CourseModel } from "../types";

export function useFeaturedCourses() {
  return useQuery<CourseModel[], Error>({
    queryKey: ["courses", "featured"],
    queryFn: fetchFeaturedCourses,
  });
}
