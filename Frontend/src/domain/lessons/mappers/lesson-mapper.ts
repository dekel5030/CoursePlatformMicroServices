import type { LessonDetailsDto } from "../types/LessonDetailsDto";
import type { LessonModel } from "../types/LessonModel";

/**
 * Maps a lesson details DTO to a LessonModel
 */
export function mapToLessonModel(dto: LessonDetailsDto): LessonModel {
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
