import { axiosClient } from "@/app/axios";
import type { EnrolledCourseCollectionDto, EnrolledCourseDto } from "../types";
import { mapEnrolledCourseWithAnalyticsToDto } from "../mappers/enrolled-course-mapper";

export interface EnrolledCourseCollectionViewDto {
  items: EnrolledCourseDto[] | null;
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages?: number;
  links: import("@/shared/types/LinkDto").LinkDto[] | null;
}

function normalizeEnrolledItem(item: unknown): EnrolledCourseDto | null {
  if (item == null || typeof item !== "object") return null;
  const obj = item as Record<string, unknown>;
  if ("data" in obj && obj.data != null && typeof obj.data === "object") {
    const data = obj.data as Record<string, unknown>;
    const links = "links" in obj ? obj.links : undefined;
    const progressPercentage =
      typeof data.progressPercentage === "number" ? data.progressPercentage : 0;
    return {
      enrollmentId: String(data.enrollmentId ?? ""),
      courseId: String(data.courseId ?? ""),
      courseTitle: data.courseTitle != null ? String(data.courseTitle) : null,
      courseImageUrl: data.courseImageUrl != null ? String(data.courseImageUrl) : null,
      courseSlug: data.courseSlug != null ? String(data.courseSlug) : null,
      progressPercentage,
      lastAccessedAt: data.lastAccessedAt != null ? String(data.lastAccessedAt) : null,
      enrolledAt: String(data.enrolledAt ?? ""),
      status: data.status != null ? String(data.status) : null,
      links: (links as EnrolledCourseDto["links"]) ?? null,
    };
  }
  if ("enrolledCourse" in obj && "analytics" in obj) {
    return mapEnrolledCourseWithAnalyticsToDto(
      obj as unknown as Parameters<typeof mapEnrolledCourseWithAnalyticsToDto>[0]
    );
  }
  return null;
}

export async function fetchMyEnrollments(
  pageNumber = 1,
  pageSize = 10
): Promise<EnrolledCourseCollectionViewDto> {
  const response = await axiosClient.get<EnrolledCourseCollectionDto>(
    "/users/me/courses/enrolled",
    { params: { pageNumber, pageSize } }
  );
  const data = response.data;
  const rawItems = data.items ?? [];
  const items = rawItems
    .map(normalizeEnrolledItem)
    .filter((d): d is EnrolledCourseDto => d != null);
  return {
    ...data,
    items,
  };
}

/**
 * Update the video offset (seconds) for the current lesson on an enrollment.
 * Call periodically while the user is watching (e.g. every 15 seconds).
 */
export async function updateLessonProgress(
  enrollmentId: string,
  lessonId: string,
  seconds: number
): Promise<void> {
  await axiosClient.patch(`/enrollments/${enrollmentId}/progress`, {
    lessonId,
    seconds,
  });
}

/**
 * Mark a lesson as completed for an enrollment.
 * Call when the user finishes the video or explicitly marks the lesson complete.
 */
export async function markLessonCompleted(
  enrollmentId: string,
  lessonId: string
): Promise<void> {
  await axiosClient.post(
    `/enrollments/${enrollmentId}/lessons/${lessonId}/completed`
  );
}
