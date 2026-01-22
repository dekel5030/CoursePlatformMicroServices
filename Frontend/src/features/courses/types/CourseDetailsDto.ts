import type { ModuleDto } from "./ModuleDto";
import type { LinkDto } from "@/types/LinkDto";

/**
 * Backend DTO: Matches CourseDetailsDto from CourseService
 * Used for network layer communication
 */
export interface CourseDetailsDto {
  id: string;
  title: string;
  description: string;
  instructorId: string;
  instructorName: string;
  instructorAvatarUrl: string | null;
  status: number;
  price: {
    amount: number;
    currency: string;
  };
  enrollmentCount: number;
  lessonsCount: number;
  totalDuration: string;
  updatedAtUtc: string;
  imageUrls: string[];
  tags: string[];
  categoryId: string;
  categoryName: string;
  categorySlug: string;
  modules: ModuleDto[];
  links: LinkDto[];
}
