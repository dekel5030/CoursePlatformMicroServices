import type {
  CourseModel,
  CourseDetailsDto,
  CourseSummaryDto,
  CreateCourseRequestDto,
  UpdateCourseRequestDto,
} from "../types";
import { axiosClient } from "@/axios/axiosClient";
import type { LessonSummaryDto, LessonModel } from "@/features/lessons/types";

function mapLessonSummaryToModel(dto: LessonSummaryDto): LessonModel {
  return {
    courseId: dto.courseId,
    lessonId: dto.lessonId,
    title: dto.title,
    description: dto.description,
    videoUrl: null,
    thumbnailImage: dto.thumbnailUrl,
    isPreview: dto.isPreview,
    order: dto.index,
    duration: dto.duration,
  };
}

function mapCourseDetailsToModel(dto: CourseDetailsDto): CourseModel {
  return {
    id: dto.id,
    title: dto.title,
    description: dto.description,
    imageUrl: dto.imageUrls?.[0] || null,
    instructorName: dto.instructorName,
    isPublished: true,
    price: {
      amount: dto.price,
      currency: dto.currency,
    },
    lessons: dto.lessons.map(mapLessonSummaryToModel),
    updatedAtUtc: dto.updatedAtUtc,
  };
}

function mapCourseSummaryToModel(dto: CourseSummaryDto): CourseModel {
  return {
    id: dto.id,
    title: dto.title,
    description: "",
    imageUrl: dto.thumbnailUrl,
    instructorName: dto.instructorName,
    isPublished: true,
    price: {
      amount: dto.price,
      currency: dto.currency,
    },
  };
}

export async function fetchFeaturedCourses(): Promise<CourseModel[]> {
  const response = await axiosClient.get<
    CourseSummaryDto[] | { items: CourseSummaryDto[] }
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
  request: CreateCourseRequestDto
): Promise<CreateCourseResponse> {
  const response = await axiosClient.post<{ id: string }>("/courses", request);
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
  const response = await axiosClient.get<
    CourseSummaryDto[] | { items: CourseSummaryDto[] }
  >("/courses");
  const data = response.data;
  const dtos = Array.isArray(data) ? data : data.items || [];
  return dtos.map(mapCourseSummaryToModel);
}
