import type { LinkDto } from "@/shared/types/LinkDto";
import type { Money } from "./money";
import type { CategoryDto, CourseStatus, InstructorDto } from "./CourseSummaryDto";
import type { ManagedCourseSummaryLinks } from "./links";

/** Data-only shape returned inside item.data by GET /manage/courses */
export interface ManagedCourseSummaryDataDto {
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
}

/** Backend returns items as { data, links }; view model flattens to this for components */
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
  /** Legacy array or strongly-typed record from API */
  links: LinkDto[] | ManagedCourseSummaryLinks;
}

/** Raw item shape from GET /manage/courses (backend sends data + links) */
export interface ManagedCourseSummaryItemDtoApi {
  data: ManagedCourseSummaryDataDto;
  links: ManagedCourseSummaryLinks;
}


export interface ManagedCourseStatsDto {
  lessonsCount: number;
  duration: string;
}
