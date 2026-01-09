/**
 * Backend DTO: Request payload for creating a course
 * Used for network layer communication
 */
export interface CreateCourseRequestDto {
  title: string;
  description?: string;
  instructorId?: string;
}
