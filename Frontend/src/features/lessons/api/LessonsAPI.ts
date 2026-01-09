import { axiosClient } from "@/axios";
import type {
  LessonDetailsDto,
  CreateLessonRequestDto,
  UpdateLessonRequestDto,
  LessonModel,
} from "../types";

/**
 * Adapter/Mapper: Converts backend DTO to UI Model
 * This is the single place where backend schema changes need to be handled
 */
function mapToLessonModel(dto: LessonDetailsDto): LessonModel {
  return {
    courseId: dto.courseId,
    lessonId: dto.lessonId,
    title: dto.title,
    description: dto.description || null,
    videoUrl: dto.videoUrl,
    thumbnailImage: dto.thumbnailUrl,
    isPreview: dto.isPreview,
    order: dto.index,
    duration: dto.duration,
  };
}

export async function fetchLessonById(
  courseId: string,
  lessonId: string
): Promise<LessonModel> {
  const response = await axiosClient.get<LessonDetailsDto>(
    `/courses/${courseId}/lessons/${lessonId}`
  );
  return mapToLessonModel(response.data);
}

export async function createLesson(
  courseId: string,
  request: CreateLessonRequestDto
): Promise<LessonModel> {
  const response = await axiosClient.post<LessonDetailsDto>(
    `/courses/${courseId}/lessons`,
    request
  );
  return mapToLessonModel(response.data);
}

export async function patchLesson(
  courseId: string,
  lessonId: string,
  request: UpdateLessonRequestDto
): Promise<void> {
  await axiosClient.patch(`/courses/${courseId}/lessons/${lessonId}`, request);
}

export async function deleteLesson(
  courseId: string,
  lessonId: string
): Promise<void> {
  await axiosClient.delete(`/courses/${courseId}/lessons/${lessonId}`);
}
