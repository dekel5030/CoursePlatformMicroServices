import { axiosClient } from "@/app/axios";
import axios from "axios";
import type {
  CourseSummaryWithAnalyticsDto,
  CreateCourseRequestDto,
  UpdateCourseRequestDto,
} from "../types";
import type { CoursePageDto } from "../types/CoursePageDto";
import type { PagedResponse } from "@/shared/types/LinkDto";
import {
  mapCoursePageDtoToModel,
  mapCourseSummaryToModel,
} from "../mappers";
import type { CourseModel } from "../types/CourseModel";

export interface CreateCourseResponse {
  courseId: string;
}

export interface FetchAllCoursesResult {
  courses: CourseModel[];
  links: import("@/shared/types/LinkDto").LinkDto[];
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
 * Fetch a course by ID
 */
export async function fetchCourseById(id: string): Promise<CourseModel> {
  const response = await axiosClient.get<CoursePageDto>(`/courses/${id}`);
  return mapCoursePageDtoToModel(response.data);
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
 * Fetch all courses with pagination support
 */
export async function fetchAllCourses(
  url?: string,
): Promise<FetchAllCoursesResult> {
  const endpoint = url || "/courses";
  const response = await axiosClient.get<
    CourseSummaryWithAnalyticsDto[] | PagedResponse<CourseSummaryWithAnalyticsDto>
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
