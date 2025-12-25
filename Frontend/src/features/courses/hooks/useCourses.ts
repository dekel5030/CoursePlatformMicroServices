import { useQuery } from '@tanstack/react-query';
import { fetchFeaturedCourses } from '@/services/CoursesAPI';
import type { Course } from '@/types';

/**
 * Hook to fetch featured courses
 */
export function useCourses() {
  return useQuery<Course[], Error>({
    queryKey: ['courses', 'featured'],
    queryFn: fetchFeaturedCourses,
  });
}
