/**
 * HATEOAS relation names for course operations
 */
export const CourseRels = {
  SELF: "self",
  PARTIAL_UPDATE: "partial-update",
  DELETE: "delete",
  CREATE: "create",
  CREATE_MODULE: "create-module",
  CREATE_LESSON: "create-lesson",
  GENERATE_IMAGE_UPLOAD_URL: "generate-image-upload-url",
  RATINGS: "ratings",
  CREATE_RATING: "create-rating",
  NEXT_PAGE: "next-page",
  PREVIOUS_PAGE: "previous-page",
  REORDER_MODULES: "reorder-modules",
} as const;
