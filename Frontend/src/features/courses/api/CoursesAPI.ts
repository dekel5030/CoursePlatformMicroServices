import type {
  CourseModel,
  CourseDetailsDto,
  CourseSummaryDto,
  CreateCourseRequestDto,
  UpdateCourseRequestDto,
} from "../types";
import { axiosClient } from "@/axios/axiosClient";
import type { LessonSummaryDto, LessonModel } from "@/features/lessons/types";

/**
 * Adapter/Mapper: Converts backend LessonSummaryDto to UI LessonModel
 */
function mapLessonSummaryToModel(dto: LessonSummaryDto): LessonModel {
  return {
    courseId: dto.courseId,
    lessonId: dto.lessonId,
    title: dto.title,
    description: dto.description || null,
    videoUrl: null, // Summary doesn't include videoUrl
    thumbnailImage: dto.thumbnailUrl,
    isPreview: dto.isPreview,
    order: dto.index,
    duration: dto.duration,
  };
}

/**
 * Adapter/Mapper: Converts backend CourseDetailsDto to UI CourseModel
 * This is the single place where backend schema changes need to be handled
 */
function mapCourseDetailsToModel(dto: CourseDetailsDto): CourseModel {
  return {
    id: dto.id,
    title: dto.title,
    description: dto.description,
    imageUrl: dto.imageUrls?.[0] || null,
    instructorUserId: dto.instructorName || null, // Note: backend sends instructorName, we may need instructorId
    isPublished: true, // Backend doesn't send this in details, assuming published
    price: {
      amount: dto.price,
      currency: dto.currency,
    },
    lessons: dto.lessons.map(mapLessonSummaryToModel),
    updatedAtUtc: dto.updatedAtUtc,
  };
}

/**
 * Adapter/Mapper: Converts backend CourseSummaryDto to UI CourseModel
 */
function mapCourseSummaryToModel(dto: CourseSummaryDto): CourseModel {
  return {
    id: dto.id,
    title: dto.title,
    description: "", // Summary doesn't include description
    imageUrl: dto.thumbnailUrl,
    instructorUserId: dto.instructorName || null, // Note: backend sends instructorName, we may need instructorId
    isPublished: true, // Backend doesn't send this in summary, assuming published
    price: {
      amount: dto.price,
      currency: dto.currency,
    },
    lessons: [], // Summary doesn't include lessons
    updatedAtUtc: new Date().toISOString(), // Summary doesn't include updatedAtUtc
  };
}

export async function fetchFeaturedCourses(): Promise<CourseModel[]> {
  const response = await axiosClient.get<CourseSummaryDto[] | { items: CourseSummaryDto[] }>(
    "/courses/featured"
  );
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
  request: CreateCourseRequestDto
): Promise<CreateCourseResponse> {
  const response = await axiosClient.post<{ id: string }>(
    "/courses",
    request
  );
  return { courseId: response.data.id };
}

export async function patchCourse(
  id: string,
  request: UpdateCourseRequestDto
): Promise<void> {
  await axiosClient.patch(`/courses/${id}`, request);
}

export async function deleteCourse(id: string): Promise<void> {
  await axiosClient.delete(`/courses/${id}`);
}

export async function fetchAllCourses(): Promise<CourseModel[]> {
  const response = await axiosClient.get<CourseSummaryDto[] | { items: CourseSummaryDto[] }>(
    "/courses"
  );
  const data = response.data;
  const dtos = Array.isArray(data) ? data : data.items || [];
  return dtos.map(mapCourseSummaryToModel);
}
