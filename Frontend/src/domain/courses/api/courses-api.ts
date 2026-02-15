import { axiosClient } from "@/app/axios";
import axios from "axios";
import type {
  CourseSummaryWithAnalyticsDto,
  CreateCourseRequestDto,
  UpdateCourseRequestDto,
  ManagedCourseSummaryDto,
  CourseDetailedAnalyticsDto,
} from "../types";
import type { ManagedCourseSummaryItemDtoApi } from "../types/ManagedCourseSummaryDto";
import type { CoursePageDto } from "../types/CoursePageDto";
import type { ManagedCoursePageDto } from "../types/ManagedCoursePageDto";
import type { PagedResponse } from "@/shared/types/LinkDto";
import {
  mapCoursePageDtoToModel,
  mapCourseSummaryToModel,
  mapManagedCoursePageDtoToModel,
} from "../mappers";
import type { CourseModel } from "../types/CourseModel";

export interface CreateCourseResponse {
  courseId: string;
}

export interface FetchAllCoursesResult {
  courses: CourseModel[];
  /** Legacy array or strongly-typed collection links (create, next, prev) */
  links:
    | import("@/shared/types/LinkDto").LinkDto[]
    | import("@/shared/types/LinkRecord").LinksRecord;
}

export interface GenerateUploadUrlRequest {
  fileName: string;
  contentType: string;
}

export interface GenerateUploadUrlResponse {
  uploadUrl: string;
  fileKey: string;
  expiresAt: string;
}

export interface CreateModuleRequest {
  title?: string;
}

export interface CreateModuleResponse {
  moduleId: string;
  courseId: string;
  title: string;
}

/**
 * Fetch featured courses
 */
export async function fetchFeaturedCourses(): Promise<CourseModel[]> {
  const response = await axiosClient.get<
    CourseSummaryWithAnalyticsDto[] | PagedResponse<CourseSummaryWithAnalyticsDto>
  >("/courses/featured");
  const data = response.data;
  const dtos = Array.isArray(data) ? data : data.items || [];
  return dtos.map(mapCourseSummaryToModel);
}

/**
 * Fetch a course by ID (public consumption view)
 */
export async function fetchCourseById(id: string): Promise<CourseModel> {
  const response = await axiosClient.get<CoursePageDto>(`/courses/${id}`);
  return mapCoursePageDtoToModel(response.data);
}

/**
 * Fetch a course for management by instructor
 */
export async function fetchManagedCourseById(id: string): Promise<CourseModel> {
  const response = await axiosClient.get<ManagedCoursePageDto>(
    `/manage/courses/${id}`
  );
  return mapManagedCoursePageDtoToModel(response.data);
}

/**
 * Create a new course
 */
export async function createCourse(
  request: CreateCourseRequestDto,
): Promise<CreateCourseResponse> {
  const response = await axiosClient.post<{
    courseId: string;
    title?: string;
  }>("/courses", request);
  return { courseId: response.data.courseId };
}

/**
 * Update a course (partial update via HATEOAS link)
 */
export async function patchCourse(
  url: string,
  request: UpdateCourseRequestDto,
): Promise<void> {
  await axiosClient.patch(url, request);
}

/**
 * Delete a course (via HATEOAS link)
 */
export async function deleteCourse(url: string): Promise<void> {
  await axiosClient.delete(url);
}

/**
 * Publish a course (POST via HATEOAS link)
 */
export async function publishCourse(url: string): Promise<void> {
  await axiosClient.post(url);
}

/** Raw catalog item from GET /courses when backend returns data+links per item */
interface CourseCatalogItemDtoApi {
  data: {
    id: string;
    title: string;
    shortDescription: string;
    slug: string;
    instructor: { id: string; fullName: string | null; avatarUrl: string | null };
    category: { id: string; name: string | null; slug: string | null };
    price: import("../types/money").Money;
    difficulty: number | string;
    thumbnailUrl: string | null;
    updatedAtUtc: string;
    status: string;
    lessonsCount: number;
    duration: string;
    enrollmentCount: number;
    averageRating: number;
    reviewsCount: number;
    courseViews: number;
  };
  links: CourseCatalogItemLinks;
}

/** Raw GET /courses response when backend returns CourseCatalogDto (items[].data + items[].links) */
interface CourseCatalogDtoApi {
  items: CourseCatalogItemDtoApi[];
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  links: CourseCatalogCollectionLinks;
}

function isCatalogItemWithData(
  item: unknown
): item is CourseCatalogItemDtoApi {
  return (
    item != null &&
    typeof item === "object" &&
    "data" in item &&
    "links" in item &&
    typeof (item as CourseCatalogItemDtoApi).data === "object"
  );
}

/** Build CourseSummaryWithAnalyticsDto from catalog item (data + links) for mapCourseSummaryToModel */
function catalogItemToSummaryWithAnalytics(
  item: CourseCatalogItemDtoApi
): CourseSummaryWithAnalyticsDto {
  const d = item.data;
  const linkDto = (href: string | null | undefined, rel: string): import("@/shared/types/LinkDto").LinkDto =>
    ({ href: href ?? "", rel, method: "GET" });
  const links: import("@/shared/types/LinkDto").LinkDto[] = [
    linkDto(item.links.self?.href, "self"),
    ...(item.links.watch?.href ? [linkDto(item.links.watch.href, "watch")] : []),
  ];
  return {
    course: {
      id: d.id,
      title: d.title,
      shortDescription: d.shortDescription,
      slug: d.slug,
      instructor: d.instructor,
      category: d.category,
      price: d.price,
      difficulty: (typeof d.difficulty === "number" ? String(d.difficulty) : d.difficulty) as import("../types/CourseSummaryDto").DifficultyLevel | string,
      thumbnailUrl: d.thumbnailUrl,
      updatedAtUtc: d.updatedAtUtc,
      status: d.status as import("../types/CourseSummaryDto").CourseStatus,
      links,
    },
    analytics: {
      lessonsCount: d.lessonsCount,
      duration: d.duration,
      enrollmentCount: d.enrollmentCount,
      averageRating: d.averageRating,
      reviewsCount: d.reviewsCount,
      courseViews: d.courseViews,
    },
  };
}

/**
 * Fetch all courses with pagination support.
 * Normalizes backend shape when items are { data, links } (CourseCatalogDto).
 */
export async function fetchAllCourses(
  url?: string,
): Promise<FetchAllCoursesResult> {
  const endpoint = url || "/courses";
  const response = await axiosClient.get<
    | CourseSummaryWithAnalyticsDto[]
    | PagedResponse<CourseSummaryWithAnalyticsDto>
    | CourseCatalogDtoApi
  >(endpoint);
  const data = response.data;

  if (Array.isArray(data)) {
    return {
      courses: data.map(mapCourseSummaryToModel),
      links: [],
    };
  }

  const items = data.items ?? [];
  if (items.length > 0 && isCatalogItemWithData(items[0])) {
    const catalogData = data as CourseCatalogDtoApi;
    const normalized = (catalogData.items as CourseCatalogItemDtoApi[]).map(
      (item) => catalogItemToSummaryWithAnalytics(item)
    );
    return {
      courses: normalized.map(mapCourseSummaryToModel),
      links: (catalogData.links ?? {}) as import("@/shared/types/LinkRecord").LinksRecord,
    };
  }

  return {
    courses: (items as CourseSummaryWithAnalyticsDto[]).map(mapCourseSummaryToModel),
    links: (data as PagedResponse<CourseSummaryWithAnalyticsDto>).links ?? [],
  };
}

/**
 * Generate an upload URL for course image
 */
export async function generateImageUploadUrl(
  uploadUrl: string,
  request: GenerateUploadUrlRequest,
): Promise<GenerateUploadUrlResponse> {
  const response = await axiosClient.post<GenerateUploadUrlResponse>(
    uploadUrl,
    request,
  );
  return response.data;
}

/**
 * Upload image to storage
 */
export async function uploadImageToStorage(
  uploadUrl: string,
  file: File,
): Promise<void> {
  // Use a raw axios instance for binary upload to avoid default JSON headers
  await axios.put(uploadUrl, file, {
    headers: {
      "Content-Type": file.type,
    },
  });
}

/**
 * Create a new module in a course
 */
export async function createModule(
  url: string,
  request: CreateModuleRequest = {},
): Promise<CreateModuleResponse> {
  const response = await axiosClient.post<CreateModuleResponse>(url, request);
  return response.data;
}

/**
 * Update a module (partial update via HATEOAS link)
 */
export async function patchModule(
  url: string,
  request: { title?: string },
): Promise<void> {
  await axiosClient.patch(url, request);
}

/**
 * Delete a module (via HATEOAS link)
 */
export async function deleteModule(url: string): Promise<void> {
  await axiosClient.delete(url);
}

export interface ReorderModulesRequest {
  moduleIds: string[];
}

/**
 * Reorder modules within a course
 */
export async function reorderModules(
  courseId: string,
  request: ReorderModulesRequest,
): Promise<void> {
  await axiosClient.patch(
    `/courses/${courseId}/structure/modules`,
    request,
  );
}

export interface ReorderLessonsRequest {
  lessonIds: string[];
}

/**
 * Reorder lessons within a module
 */
export async function reorderLessons(
  moduleId: string,
  request: ReorderLessonsRequest,
): Promise<void> {
  await axiosClient.patch(
    `/modules/${moduleId}/lessons/reorder`,
    request,
  );
}

import type { GetManagedCoursesCollectionLinks } from "../types/links";
import type { CourseCatalogItemLinks, CourseCatalogCollectionLinks } from "../types/links";

/** Raw API response: items are { data, links } */
interface GetManagedCoursesDtoApi {
  items: ManagedCourseSummaryItemDtoApi[];
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages?: number;
  links: GetManagedCoursesCollectionLinks;
}

export interface ManagedCoursesResponse {
  items: ManagedCourseSummaryDto[];
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages?: number;
  links: GetManagedCoursesDtoApi["links"];
}

/**
 * Fetch courses managed by the current user (instructor).
 * Normalizes backend shape (items[].data + items[].links) to flat ManagedCourseSummaryDto[].
 */
export async function fetchMyManagedCourses(
  pageNumber = 1,
  pageSize = 10
): Promise<ManagedCoursesResponse> {
  const response = await axiosClient.get<GetManagedCoursesDtoApi>(
    "/manage/courses",
    { params: { pageNumber, pageSize } }
  );
  const raw = response.data;
  return {
    pageNumber: raw.pageNumber,
    pageSize: raw.pageSize,
    totalItems: raw.totalItems,
    totalPages: raw.totalPages,
    links: raw.links,
    items: (raw.items ?? []).map((item) => ({
      ...item.data,
      links: item.links,
    })),
  };
}

type CourseAnalyticsResponse =
  | CourseDetailedAnalyticsDto
  | { data: Omit<CourseDetailedAnalyticsDto, "links">; links?: CourseDetailedAnalyticsDto["links"] };

/**
 * Fetch detailed analytics for a course (instructor only)
 */
export async function fetchCourseAnalytics(
  courseId: string
): Promise<CourseDetailedAnalyticsDto> {
  const response = await axiosClient.get<CourseAnalyticsResponse>(
    `/manage/courses/${courseId}/analytics`
  );
  const raw = response.data;
  if (raw != null && "data" in raw && typeof raw.data === "object") {
    return { ...raw.data, links: raw.links };
  }
  return raw as CourseDetailedAnalyticsDto;
}
