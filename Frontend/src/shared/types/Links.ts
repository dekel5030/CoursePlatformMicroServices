/**
 * Base link record - matches backend LinkRecord
 */
export interface LinkRecord {
  href: string;
  method: string;
}

/**
 * Typed links for course (management view)
 */
export interface ManagedCourseLinks {
  self: LinkRecord;
  ratings: LinkRecord;
  coursePage?: LinkRecord | null;
  analytics?: LinkRecord | null;
  partialUpdate?: LinkRecord | null;
  delete?: LinkRecord | null;
  publish?: LinkRecord | null;
  generateImageUploadUrl?: LinkRecord | null;
  createModule?: LinkRecord | null;
  reorderModules?: LinkRecord | null;
}

/**
 * Typed links for module (management view)
 */
export interface ManagedModuleLinks {
  createLesson?: LinkRecord | null;
  partialUpdate?: LinkRecord | null;
  delete?: LinkRecord | null;
  reorderLessons?: LinkRecord | null;
}

/**
 * Typed links for lesson (management view)
 */
export interface ManagedLessonLinks {
  self: LinkRecord;
  partialUpdate?: LinkRecord | null;
  uploadVideoUrl?: LinkRecord | null;
  aiGenerate?: LinkRecord | null;
  move?: LinkRecord | null;
  delete?: LinkRecord | null;
}

/**
 * Typed links for enrolled course (student view)
 */
export interface EnrolledCourseLinks {
  viewCourse: LinkRecord;
  continueLearning?: LinkRecord | null;
}

/**
 * Pagination links for collection responses
 */
export interface PaginationLinks {
  self: LinkRecord;
  previousPage?: LinkRecord | null;
  nextPage?: LinkRecord | null;
  create?: LinkRecord | null;
}
