import { axiosClient } from "@/axios";
import axios from "axios";
import type {
  LessonDetailsDto,
  CreateLessonRequestDto,
  UpdateLessonRequestDto,
  LessonModel,
} from "../types";

function mapToLessonModel(dto: LessonDetailsDto): LessonModel {
  return {
    courseId: dto.courseId,
    lessonId: dto.lessonId,
    title: dto.title,
    description: dto.description,
    videoUrl: dto.videoUrl,
    transcriptUrl: dto.transcriptUrl ?? null,
    thumbnailImage: dto.thumbnailUrl,
    isPreview: dto.isPreview,
    order: dto.index,
    duration: dto.duration,
    links: dto.links,
  };
}

export async function fetchLessonById(
  courseId: string,
  lessonId: string,
  url?: string,
): Promise<LessonModel> {
  // Use the provided HATEOAS self link if available
  // Otherwise fall back to the constructed course-based URL
  const endpoint = url || `/courses/${courseId}/lessons/${lessonId}`;
  const response = await axiosClient.get<LessonDetailsDto>(endpoint);
  return mapToLessonModel(response.data);
}

export async function createLesson(
  url: string,
  request: CreateLessonRequestDto,
): Promise<LessonModel> {
  const response = await axiosClient.post<LessonDetailsDto>(url, request);
  return mapToLessonModel(response.data);
}

export async function patchLesson(
  url: string,
  request: UpdateLessonRequestDto,
): Promise<void> {
  await axiosClient.patch(url, request);
}

export async function deleteLesson(url: string): Promise<void> {
  await axiosClient.delete(url);
}

export interface GenerateVideoUploadUrlRequest {
  fileName: string;
  contentType: string;
}

export interface GenerateVideoUploadUrlResponse {
  uploadUrl: string;
  fileKey: string;
  expiresAt: string;
}

export async function generateVideoUploadUrl(
  uploadUrl: string,
  request: GenerateVideoUploadUrlRequest,
): Promise<GenerateVideoUploadUrlResponse> {
  const response = await axiosClient.post<GenerateVideoUploadUrlResponse>(
    uploadUrl,
    request,
  );
  return response.data;
}

export async function uploadVideoToStorage(
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

export interface GenerateLessonAiResponse {
  title: string;
  description: string;
}

export async function generateLessonAi(
  url: string,
): Promise<GenerateLessonAiResponse> {
  const response = await axiosClient.post<GenerateLessonAiResponse>(url);
  return response.data;
}
