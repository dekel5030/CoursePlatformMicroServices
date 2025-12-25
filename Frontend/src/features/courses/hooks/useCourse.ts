import { useQuery } from '@tanstack/react-query';
import { fetchCourseById } from '@/services/CoursesAPI';
import type { Course } from '@/types';

export function useCourse(id: string | undefined) {
  return useQuery<Course, Error>({
    queryKey: ['courses', id],
    queryFn: () => fetchCourseById(id!),
    enabled: !!id,
  });
}
