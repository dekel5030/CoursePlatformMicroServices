import type { LinkDto } from "@/shared/types/LinkDto";
import type { Money } from "./money";
import type { CategoryDto, CourseStatus, InstructorDto } from "./CourseSummaryDto";


export interface ManagedCourseSummaryDto {
  id: string;
  title: string;
  shortDescription: string;
  slug: string;
  instructor: InstructorDto;
  category: CategoryDto;
  price: Money;
  difficulty: string;
  thumbnailUrl: string | null;
  updatedAtUtc: string;
  status: CourseStatus;
  stats: ManagedCourseStatsDto;
  links: LinkDto[];
}


export interface ManagedCourseStatsDto {
  lessonsCount: number;
  duration: string;
}
