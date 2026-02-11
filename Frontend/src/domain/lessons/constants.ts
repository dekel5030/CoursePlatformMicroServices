/**
 * HATEOAS relation names for lesson operations
 */
export const LessonRels = {
  SELF: "self",
  PARTIAL_UPDATE: "partial-update",
  DELETE: "delete",
  UPLOAD_VIDEO_URL: "upload-video-url",
  GENERATE_VIDEO_UPLOAD_URL: "generate-video-upload-url",
  AI_GENERATE: "ai-generate",
  MOVE: "move",
} as const;
