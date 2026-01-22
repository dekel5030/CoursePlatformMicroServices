import type {
  CourseModel,
  CourseDetailsDto,
  CourseSummaryDto,
  CreateCourseRequestDto,
  UpdateCourseRequestDto,
  ModuleDto,
  ModuleModel,
} from "../types";
import { axiosClient } from "@/axios/axiosClient";
import axios from "axios";
import type { LessonSummaryDto, LessonModel } from "@/features/lessons/types";
import type { PagedResponse } from "@/types/LinkDto";

function mapLessonSummaryToModel(
  dto: LessonSummaryDto,
  courseId?: string,
): LessonModel {
  return {
    courseId: dto.courseId || courseId || "",
    lessonId: dto.lessonId,
    title: dto.title,
    description: dto.description || "",
    videoUrl: null,
    thumbnailImage: dto.thumbnailUrl,
    isPreview: dto.isPreview || dto.access === "Public",
    order: dto.index,
    duration: dto.duration,
    links: dto.links,
  };
}

function mapModuleToModel(dto: ModuleDto, courseId: string): ModuleModel {
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

function mapCourseDetailsToModel(dto: CourseDetailsDto): CourseModel {
  return {
    id: dto.id,
    title: dto.title,
    description: dto.description,
    imageUrl: dto.imageUrls?.[0] || null,
    instructorName: dto.instructorName,
    instructorAvatarUrl: dto.instructorAvatarUrl,
    isPublished: dto.status === 1,
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
    tags: dto.tags,
    links: dto.links,
  };
}

function mapCourseSummaryToModel(dto: CourseSummaryDto): CourseModel {
  return {
    id: dto.id,
    title: dto.title,
    description: "",
    imageUrl: dto.thumbnailUrl,
    instructorName: dto.instructorName,
    instructorAvatarUrl: null,
    isPublished: true,
    price: {
      amount: dto.price,
      currency: dto.currency,
    },
    lessonCount: dto.lessonsCount,
    links: dto.links,
  };
}

export async function fetchFeaturedCourses(): Promise<CourseModel[]> {
  const response = await axiosClient.get<
    CourseSummaryDto[] | PagedResponse<CourseSummaryDto>
  >("/courses/featured");
  const data = response.data;
  const dtos = Array.isArray(data) ? data : data.items || [];
  return dtos.map(mapCourseSummaryToModel);
}

export async function fetchCourseById(id: string): Promise<CourseModel> {
  const response = await axiosClient.get<CourseDetailsDto>(`/courses/${id}`);
  return mapCourseDetailsToModel(response.data);
}

export interface CreateCourseResponse {
  courseId: string;
}

export async function createCourse(
  request: CreateCourseRequestDto,
): Promise<CreateCourseResponse> {
  const response = await axiosClient.post<{ id: string }>("/courses", request);
  return { courseId: response.data.id };
}

export async function patchCourse(
  url: string,
  request: UpdateCourseRequestDto,
): Promise<void> {
  await axiosClient.patch(url, request);
}

export async function deleteCourse(url: string): Promise<void> {
  await axiosClient.delete(url);
}

export interface FetchAllCoursesResult {
  courses: CourseModel[];
  links: import("@/types/LinkDto").LinkDto[];
}

export async function fetchAllCourses(
  url?: string,
): Promise<FetchAllCoursesResult> {
  const endpoint = url || "/courses";
  const response = await axiosClient.get<
    CourseSummaryDto[] | PagedResponse<CourseSummaryDto>
  >(endpoint);
  const data = response.data;

  // Handle both legacy array format and new PagedResponse format
  if (Array.isArray(data)) {
    return {
      courses: data.map(mapCourseSummaryToModel),
      links: [],
    };
  }

  return {
    courses: data.items.map(mapCourseSummaryToModel),
    links: data.links,
  };
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

export interface CreateModuleRequest {
  title?: string;
}

export interface CreateModuleResponse {
  moduleId: string;
  courseId: string;
  title: string;
}

export async function createModule(
  url: string,
  request: CreateModuleRequest = {},
): Promise<CreateModuleResponse> {
  const response = await axiosClient.post<CreateModuleResponse>(url, request);
  return response.data;
}
