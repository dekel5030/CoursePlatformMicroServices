/**
 * Backend DTO: Request payload for creating a lesson
 * Used for network layer communication
 */
export interface CreateLessonRequestDto {
  title: string;
  description?: string;
}
