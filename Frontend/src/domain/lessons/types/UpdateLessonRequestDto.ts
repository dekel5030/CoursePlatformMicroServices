/**
 * Backend DTO: Request payload for updating a lesson
 * Used for network layer communication
 */
export interface UpdateLessonRequestDto {
  title?: string;
  description?: string;
  access?: "Private" | "Public";
}
