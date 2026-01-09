/**
 * Backend DTO: Request payload for updating a course
 * Used for network layer communication
 */
export interface UpdateCourseRequestDto {
  title?: string;
  description?: string;
  instructorId?: string;
  priceAmount?: number;
  priceCurrency?: string;
}
