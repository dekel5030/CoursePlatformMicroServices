import { useQuery } from '@tanstack/react-query';
import { fetchLessonById } from '@/services/CoursesAPI';
import type { Lesson } from '@/types';

/**
 * Hook to fetch a single lesson by ID
 */
export function useLesson(id: string | undefined) {
  return useQuery<Lesson, Error>({
    queryKey: ['lessons', id],
    queryFn: () => fetchLessonById(id!),
    enabled: !!id, // Only run query if id exists
  });
}
