import { useQuery } from '@tanstack/react-query';
import { fetchCourseById } from '@/services/CoursesAPI';
import type { Course } from '@/types';

/**
 * Hook to fetch a single course by ID
 */
export function useCourse(id: string | undefined) {
  return useQuery<Course, Error>({
    queryKey: ['courses', id],
    queryFn: () => fetchCourseById(id!),
    enabled: !!id, // Only run query if id exists
  });
}
