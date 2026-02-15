import type { CourseSummaryWithAnalyticsDto, ManagedCourseSummaryDto } from "../types";
import type { CourseDetailsDto } from "../types/CourseDetailsDto";
import type { CourseModel } from "../types/CourseModel";
import { DifficultyLevel } from "../types/CourseModel";
import type { ModuleDto, ModuleLessonDto } from "../types/ModuleDto";
import type { ModuleModel } from "../types/ModuleModel";
import type {
  CoursePageDto,
  CoursePageCourseDto,
  CoursePageModuleDto,
  CoursePageLessonDto,
} from "../types/CoursePageDto";
import type {
  ManagedCoursePageDto,
  ManagedCoursePageModuleDto,
  ManagedCoursePageLessonDto,
} from "../types/ManagedCoursePageDto";
import { linkDtoArrayToRecord } from "@/shared/utils/link-helpers";
import type { LinkDto } from "@/shared/types/LinkDto";
import type { LinksRecord } from "@/shared/types/LinkRecord";
import type { LessonSummaryDto, LessonModel } from "@/domain/lessons/types";

/**
 * Maps a lesson summary DTO (from module or standalone) to a LessonModel
 */
function mapLessonSummaryToModel(
  dto: LessonSummaryDto | ModuleLessonDto,
  courseId?: string,
): LessonModel {
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
    links:
      "links" in dto && dto.links != null
        ? Array.isArray(dto.links)
          ? (linkDtoArrayToRecord(dto.links) as LessonModel["links"])
          : (dto.links as LessonModel["links"])
        : undefined,
  };
}

/**
 * Maps CoursePageLessonDto (data + links) to LessonModel
 */
function mapCoursePageLessonToModel(
  lesson: CoursePageLessonDto,
  courseId: string,
): LessonModel {
  const d = lesson.data;
  return {
    courseId: d.courseId ?? courseId,
    lessonId: d.id,
    title: d.title ?? "",
    description: d.description ?? "",
    videoUrl: d.videoUrl ?? null,
    transcriptUrl: d.transcriptUrl ?? null,
    thumbnailImage: d.thumbnailUrl ?? null,
    isPreview: d.access === "Public",
    order: d.index,
    duration: d.duration ?? null,
    links: lesson.links as LessonModel["links"],
  };
}

/**
 * Maps ManagedCoursePageLessonDto (data + links) to LessonModel
 */
function mapManagedLessonToModel(
  lesson: ManagedCoursePageLessonDto,
  courseId: string,
): LessonModel {
  const d = lesson.data;
  return {
    courseId: d.courseId ?? courseId,
    lessonId: d.id,
    title: d.title ?? "",
    description: d.description ?? "",
    videoUrl: d.videoUrl ?? null,
    transcriptUrl: d.transcriptUrl ?? null,
    thumbnailImage: d.thumbnailUrl ?? null,
    isPreview: d.access === "Public",
    order: d.index,
    duration: d.duration ?? null,
    links: lesson.links as LessonModel["links"],
  };
}

/**
 * Maps CoursePageModuleDto to ModuleModel (public course page)
 */
function mapCoursePageModuleToModuleModel(
  moduleDto: CoursePageModuleDto,
  lessonsRecord: Record<string, CoursePageLessonDto>,
  courseId: string,
  lessonIds: string[],
  order: number,
): ModuleModel {
  const d = moduleDto.data;
  const lessonDtos = lessonIds
    .map((id) => lessonsRecord[id])
    .filter((l): l is CoursePageLessonDto => l != null)
    .sort((a, b) => a.data.index - b.data.index);
  const lessons = lessonDtos.map((l) => mapCoursePageLessonToModel(l, courseId));

  return {
    id: d.id,
    title: d.title ?? "",
    order,
    lessonCount: d.lessonCount,
    duration: d.totalDuration ?? "PT0S",
    lessons,
    links: (moduleDto.links ?? {}) as ModuleModel["links"],
  };
}

/**
 * Maps ManagedCoursePageModuleDto to ModuleModel
 */
function mapManagedModuleToModuleModel(
  moduleDto: ManagedCoursePageModuleDto,
  lessonsRecord: Record<string, ManagedCoursePageLessonDto>,
  courseId: string,
  lessonIds: string[],
  order: number,
): ModuleModel {
  const d = moduleDto.data;
  const lessonDtos = lessonIds
    .map((id) => lessonsRecord[id])
    .filter((l): l is ManagedCoursePageLessonDto => l != null)
    .sort((a, b) => a.data.index - b.data.index);
  const lessons = lessonDtos.map((l) => mapManagedLessonToModel(l, courseId));

  return {
    id: d.id,
    title: d.title ?? "",
    order,
    lessonCount: d.lessonCount,
    duration: d.duration ?? "PT0S",
    lessons,
    links: moduleDto.links as ModuleModel["links"],
  };
}

/**
 * Maps CoursePageDto (GET /courses/{id}) to CourseModel
 */
export function mapCoursePageDtoToModel(dto: CoursePageDto): CourseModel {
  const courseDto = dto.course as CoursePageCourseDto;
  const c = courseDto.data;
  const courseLinks = courseDto.links;
  const modulesRecord = dto.modules ?? {};
  const lessonsRecord = dto.lessons ?? {};
  const a = dto.analytics ?? (() => {
    const lessonCount = Object.values(modulesRecord).reduce(
      (sum, m) => sum + (m?.data?.lessonCount ?? 0),
      0
    );
    return {
      enrollmentCount: 0,
      lessonsCount: lessonCount,
      totalDuration: "PT0S",
      averageRating: 0,
      reviewsCount: 0,
      viewCount: 0,
    };
  })();
  const instructors = dto.instructors ?? {};
  const categories = dto.categories ?? {};
  const structure = dto.structure;
  const moduleIds = structure.moduleIds ?? [];
  const moduleLessonIds = structure.moduleLessonIds ?? {};

  const instructor = c.instructorId ? instructors[c.instructorId] : undefined;
  const instructorName = instructor
    ? [instructor.firstName, instructor.lastName].filter(Boolean).join(" ") || null
    : null;
  const instructorAvatarUrl = instructor?.avatarUrl ?? null;
  const category = c.categoryId ? categories[c.categoryId] : undefined;
  const categoryName = category?.name ?? undefined;
  const categorySlug = category?.slug ?? undefined;

  const modules = moduleIds
    .map((moduleId) => modulesRecord[moduleId])
    .filter((m): m is CoursePageModuleDto => m != null)
    .map((m, index) =>
      mapCoursePageModuleToModuleModel(
        m,
        lessonsRecord,
        c.id,
        moduleLessonIds[m.data.id] ?? [],
        index,
      ),
    );

  return {
    id: c.id,
    title: c.title ?? "",
    description: c.description ?? "",
    imageUrl: c.imageUrls?.[0] ?? null,
    instructorName,
    instructorAvatarUrl,
    isPublished: c.status === "Published",
    price: {
      amount: c.price.amount,
      currency: c.price.currency ?? "",
    },
    modules,
    lessonCount: a.lessonsCount,
    enrollmentCount: a.enrollmentCount,
    totalDuration: a.totalDuration,
    averageRating: a.averageRating,
    reviewsCount: a.reviewsCount,
    courseViews: a.viewCount ?? 0,
    updatedAtUtc: c.updatedAtUtc,
    categoryName,
    categoryId: c.categoryId,
    categorySlug,
    tags: c.tags ?? undefined,
    links: courseLinks as CourseModel["links"],
  };
}

/**
 * Maps a module DTO to a ModuleModel (legacy ModuleDto with links array)
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
    links: Array.isArray(dto.links)
      ? (linkDtoArrayToRecord(dto.links) as ModuleModel["links"])
      : (dto.links ?? {}),
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
    links: linkDtoArrayToRecord((dto as { links?: import("@/shared/types/LinkDto").LinkDto[] }).links) as CourseModel["links"],
  };
}

function getDifficultyEnum(difficulty: unknown): DifficultyLevel | undefined {
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
}

/**
 * Maps a course summary DTO (with analytics) to a CourseModel
 */
export function mapCourseSummaryToModel(
  dto: CourseSummaryWithAnalyticsDto,
): CourseModel {
  const c = dto.course;
  const a = dto.analytics;

  return {
    id: c.id,
    title: c.title,
    description: c.shortDescription || "",
    shortDescription: c.shortDescription,
    slug: c.slug,
    imageUrl: c.thumbnailUrl,
    instructorId: c.instructor.id,
    instructorName: c.instructor.fullName,
    instructorAvatarUrl: c.instructor.avatarUrl,
    isPublished: c.status === "Published",
    status: c.status,
    price: c.price,
    originalPrice: undefined,
    badges: [],
    averageRating: a.averageRating,
    reviewsCount: a.reviewsCount,
    difficulty: getDifficultyEnum(c.difficulty),
    courseViews: a.courseViews,
    lessonCount: a.lessonsCount,
    enrollmentCount: a.enrollmentCount,
    totalDuration: a.duration,
    updatedAtUtc: c.updatedAtUtc,
    categoryName: c.category.name || undefined,
    categoryId: c.category.id,
    categorySlug: c.category.slug || undefined,
    links: linkDtoArrayToRecord((c as { links?: import("@/shared/types/LinkDto").LinkDto[] }).links) as CourseModel["links"],
  };
}

/**
 * Maps ManagedCoursePageDto (GET /manage/courses/{id}) to CourseModel
 */
export function mapManagedCoursePageDtoToModel(
  dto: ManagedCoursePageDto,
): CourseModel {
  const courseDto = dto.course;
  const c = courseDto.data;
  const courseLinks = courseDto.links;
  const modulesRecord = dto.modules ?? {};
  const lessonsRecord = dto.lessons ?? {};
  const categories = dto.categories ?? {};
  const structure = dto.structure;
  const moduleIds = structure.moduleIds ?? [];
  const moduleLessonIds = structure.moduleLessonIds ?? {};

  const category = c.categoryId ? categories[c.categoryId] : undefined;
  const categoryName = category?.name ?? undefined;
  const categorySlug = category?.slug ?? undefined;

  const modules = moduleIds
    .map((moduleId) => modulesRecord[moduleId])
    .filter((m): m is ManagedCoursePageModuleDto => m != null)
    .map((m, index) =>
      mapManagedModuleToModuleModel(
        m,
        lessonsRecord,
        c.id,
        moduleLessonIds[m.data.id] ?? [],
        index,
      ),
    );

  const lessonCount = modules.reduce((sum, m) => sum + m.lessonCount, 0);

  return {
    id: c.id,
    title: c.title ?? "",
    description: c.description ?? "",
    imageUrl: c.imageUrls?.[0] ?? null,
    instructorName: null,
    instructorAvatarUrl: null,
    isPublished: c.status === "Published",
    price: {
      amount: c.price.amount,
      currency: c.price.currency ?? "",
    },
    modules,
    lessonCount,
    enrollmentCount: 0,
    totalDuration: "PT0S",
    averageRating: 0,
    reviewsCount: 0,
    courseViews: 0,
    updatedAtUtc: c.updatedAtUtc,
    categoryName,
    categoryId: c.categoryId,
    categorySlug,
    tags: c.tags ?? undefined,
    links: courseLinks as CourseModel["links"],
  };
}

/**
 * Maps a managed course summary DTO to a CourseModel.
 * Accepts links as either legacy LinkDto[] or strongly-typed record.
 */
export function mapManagedCourseSummaryToModel(
  dto: ManagedCourseSummaryDto,
): CourseModel {
  const linksRaw = dto.links;
  const links: LinksRecord =
    linksRaw == null
      ? {}
      : Array.isArray(linksRaw)
        ? linkDtoArrayToRecord(linksRaw as LinkDto[])
        : (linksRaw as LinksRecord);

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
    originalPrice: undefined,
    badges: [],
    averageRating: 0,
    reviewsCount: 0,
    difficulty: getDifficultyEnum(dto.difficulty),
    courseViews: 0,
    lessonCount: dto.stats.lessonsCount,
    enrollmentCount: 0,
    totalDuration: dto.stats.duration,
    updatedAtUtc: dto.updatedAtUtc,
    categoryName: dto.category.name || undefined,
    categoryId: dto.category.id,
    categorySlug: dto.category.slug || undefined,
    links,
  };
}
