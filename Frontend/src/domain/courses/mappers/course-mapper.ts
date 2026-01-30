import type { CourseDetailsDto, CourseSummaryDto } from "../types";
import type { CourseModel } from "../types/CourseModel";
import { DifficultyLevel } from "../types/CourseModel";
import type { ModuleDto, ModuleLessonDto } from "../types/ModuleDto";
import type { ModuleModel } from "../types/ModuleModel";
import type { LessonSummaryDto, LessonModel } from "@/domain/lessons/types";

/**
 * Maps a lesson summary DTO (from module or standalone) to a LessonModel
 */
function mapLessonSummaryToModel(
  dto: LessonSummaryDto | ModuleLessonDto,
  courseId?: string,
): LessonModel {
  // Handle both LessonSummaryDto and ModuleLessonDto
  const access =
    "access" in dto ? dto.access : dto.isPreview ? "Public" : "Private";

  return {
    courseId:
      "courseId" in dto ? dto.courseId || courseId || "" : courseId || "",
    lessonId: dto.lessonId,
    title: dto.title,
    description: "description" in dto ? dto.description || "" : "",
    videoUrl: null,
    transcriptUrl: null,
    thumbnailImage: dto.thumbnailUrl,
    isPreview: access === "Public",
    order: dto.index,
    duration: dto.duration,
    links: dto.links,
  };
}

/**
 * Maps a module DTO to a ModuleModel
 */
export function mapModuleToModel(dto: ModuleDto, courseId: string): ModuleModel {
  return {
    id: dto.id,
    title: dto.title,
    order: dto.index,
    lessonCount: dto.lessonCount,
    duration: dto.duration,
    lessons: dto.lessons.map((lesson) =>
      mapLessonSummaryToModel(lesson, courseId),
    ),
    links: dto.links,
  };
}

/**
 * Maps a course details DTO to a CourseModel
 */
export function mapCourseDetailsToModel(dto: CourseDetailsDto): CourseModel {
  return {
    id: dto.id,
    title: dto.title,
    description: dto.description,
    imageUrl: dto.imageUrls?.[0] || null,
    instructorName: dto.instructorName,
    instructorAvatarUrl: dto.instructorAvatarUrl,
    isPublished: dto.status === "Published",
    price: {
      amount: dto.price.amount,
      currency: dto.price.currency,
    },
    modules:
      dto.modules?.map((module) => mapModuleToModel(module, dto.id)) || [],
    lessonCount: dto.lessonsCount,
    enrollmentCount: dto.enrollmentCount,
    totalDuration: dto.totalDuration,
    updatedAtUtc: dto.updatedAtUtc,
    categoryName: dto.categoryName,
    categoryId: dto.categoryId,
    tags: dto.tags,
    links: dto.links,
  };
}

/**
 * Maps a course summary DTO to a CourseModel
 */
export function mapCourseSummaryToModel(dto: CourseSummaryDto): CourseModel {
  // Convert difficulty from string to enum if needed
  const getDifficultyEnum = (difficulty: unknown): DifficultyLevel | undefined => {
    if (typeof difficulty === "number") {
      return difficulty as DifficultyLevel;
    }
    if (typeof difficulty === "string") {
      const difficultyMap: Record<string, DifficultyLevel> = {
        Beginner: DifficultyLevel.Beginner,
        Intermediate: DifficultyLevel.Intermediate,
        Advanced: DifficultyLevel.Advanced,
        Expert: DifficultyLevel.Expert,
      };
      return difficultyMap[difficulty];
    }
    return undefined;
  };

  return {
    id: dto.id,
    title: dto.title,
    description: dto.shortDescription || "",
    shortDescription: dto.shortDescription,
    slug: dto.slug,
    imageUrl: dto.thumbnailUrl,
    instructorId: dto.instructor.id,
    instructorName: dto.instructor.fullName,
    instructorAvatarUrl: dto.instructor.avatarUrl,
    isPublished: dto.status === "Published",
    status: dto.status,
    price: dto.price,
    originalPrice: dto.originalPrice,
    badges: dto.badges,
    averageRating: dto.averageRating,
    reviewsCount: dto.reviewsCount,
    difficulty: getDifficultyEnum(dto.difficulty),
    courseViews: dto.courseViews,
    lessonCount: dto.lessonsCount,
    enrollmentCount: dto.enrollmentCount,
    totalDuration: dto.duration,
    updatedAtUtc: dto.updatedAtUtc,
    categoryName: dto.category.name || undefined,
    categoryId: dto.category.id,
    categorySlug: dto.category.slug || undefined,
    links: dto.links,
  };
}
