import { axiosClient } from "@/app/axios";
import axios from "axios";
import type {
  LessonDetailsDto,
  CreateLessonRequestDto,
  UpdateLessonRequestDto,
  LessonModel,
} from "../types";
import { mapToLessonModel } from "../mappers";

/**
 * Managed lesson page returns { data: ManagedLessonPageData, links: ManagedLessonPageLinks }.
 * Public lesson page returns a flat LessonDetailsDto.
 */
function isManagedLessonPageResponse(
  raw: unknown,
): raw is { data: Record<string, unknown>; links?: Record<string, unknown> } {
  return (
    typeof raw === "object" &&
    raw !== null &&
    "data" in raw &&
    typeof (raw as { data: unknown }).data === "object" &&
    (raw as { data: unknown }).data !== null &&
    "lessonId" in (raw as { data: Record<string, unknown> }).data
  );
}

/**
 * Fetch a lesson by ID
 * When url is provided (e.g. manage lesson), use it as path relative to API base (no leading /api).
 * Backend manage endpoint returns { data, links }; we unwrap and map so videoUrl etc. come from data.
 */
export async function fetchLessonById(
  courseId: string,
  lessonId: string,
  url?: string,
): Promise<LessonModel> {
  const endpoint = url || `/courses/${courseId}/lessons/${lessonId}`;
  const path = endpoint.startsWith("/api") ? endpoint.slice(4) || "/" : endpoint;
  const response = await axiosClient.get<LessonDetailsDto | { data: LessonDetailsDto; links?: LessonModel["links"] }>(path);
  const raw = response.data;
  if (isManagedLessonPageResponse(raw)) {
    const dto = { ...raw.data, links: raw.links } as LessonDetailsDto & { links?: LessonModel["links"] };
    return mapToLessonModel(dto);
  }
  return mapToLessonModel(raw as LessonDetailsDto);
}

/**
 * Create a new lesson
 */
export async function createLesson(
  url: string,
  request: CreateLessonRequestDto,
): Promise<LessonModel> {
  const response = await axiosClient.post<LessonDetailsDto>(url, request);
  return mapToLessonModel(response.data);
}

/**
 * Update a lesson (partial update via HATEOAS link)
 */
export async function patchLesson(
  url: string,
  request: UpdateLessonRequestDto,
): Promise<void> {
  await axiosClient.patch(url, request);
}

/**
 * Delete a lesson (via HATEOAS link)
 */
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

/**
 * Generate an upload URL for lesson video
 */
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

/**
 * Upload video to storage
 */
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

/**
 * Generate lesson content using AI
 */
export async function generateLessonAi(
  url: string,
): Promise<GenerateLessonAiResponse> {
  const response = await axiosClient.post<GenerateLessonAiResponse>(url);
  return response.data;
}

export interface MoveLessonRequest {
  targetModuleId: string;
  targetIndex: number;
}

/**
 * Move a lesson to a different module
 */
export async function moveLesson(
  lessonId: string,
  request: MoveLessonRequest,
): Promise<void> {
  await axiosClient.patch(`/lessons/${lessonId}/move`, request);
}

import type { TranscriptSegment } from "../types/TranscriptSegment";

/**
 * Get transcript segments for a lesson (VTT parsed to JSON).
 * @param transcriptHref - HATEOAS href for the transcript (e.g. from links.manageTranscript.href)
 */
export async function getLessonTranscript(
  transcriptHref: string,
): Promise<TranscriptSegment[]> {
  const response = await axiosClient.get<TranscriptSegment[]>(transcriptHref);
  return response.data;
}

/**
 * Update lesson transcript (JSON segments serialized to VTT and stored).
 * @param transcriptHref - Same HATEOAS href as GET (PUT to same URL)
 */
export async function putLessonTranscript(
  transcriptHref: string,
  segments: TranscriptSegment[],
): Promise<void> {
  await axiosClient.put(transcriptHref, segments);
}
