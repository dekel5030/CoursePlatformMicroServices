import type { LinkRecord } from "@/shared/types/LinkRecord";

/** Course page (public): course.links */
export interface CoursePageCourseLinks {
  self?: LinkRecord;
  manage?: LinkRecord;
  ratings?: LinkRecord;
}

/** Managed course page: course.links */
export interface ManagedCourseLinks {
  self?: LinkRecord;
  coursePage?: LinkRecord;
  analytics?: LinkRecord;
  partialUpdate?: LinkRecord;
  delete?: LinkRecord;
  publish?: LinkRecord;
  generateImageUploadUrl?: LinkRecord;
  createModule?: LinkRecord;
  changePosition?: LinkRecord;
}

/** Managed courses list: item.links */
export interface ManagedCourseSummaryLinks {
  self?: LinkRecord;
  coursePage?: LinkRecord;
  analytics?: LinkRecord;
  editMetadata?: LinkRecord;
  delete?: LinkRecord;
}

/** Managed courses list: collection.links */
export interface GetManagedCoursesCollectionLinks {
  self?: LinkRecord;
  create?: LinkRecord;
  next?: LinkRecord;
  prev?: LinkRecord;
}

/** Course catalog: item.links */
export interface CourseCatalogItemLinks {
  self?: LinkRecord;
  watch?: LinkRecord;
}

/** Course catalog: collection.links */
export interface CourseCatalogCollectionLinks {
  self?: LinkRecord;
  next?: LinkRecord;
  prev?: LinkRecord;
}

/** Course ratings: collection.links */
export interface GetCourseRatingsCollectionLinks {
  self?: LinkRecord;
  next?: LinkRecord;
  prev?: LinkRecord;
  create?: LinkRecord;
}

/** Course ratings: item.links */
export interface CourseRatingItemLinks {
  update?: LinkRecord;
  delete?: LinkRecord;
}

/** Managed module: module.links */
export interface ManagedModuleLinks {
  createLesson?: LinkRecord;
  partialUpdate?: LinkRecord;
  delete?: LinkRecord;
  changePosition?: LinkRecord;
}

/** Course page lesson: lesson.links */
export interface CoursePageLessonLinks {
  self?: LinkRecord;
}

/** Managed course page lesson: lesson.links */
export interface ManagedLessonLinks {
  self?: LinkRecord;
  manage?: LinkRecord;
  partialUpdate?: LinkRecord;
  changePosition?: LinkRecord;
}

/** Managed lesson page: page.links */
export interface ManagedLessonPageLinks {
  self?: LinkRecord;
  managedCourse?: LinkRecord;
  publicPreview?: LinkRecord;
  partialUpdate?: LinkRecord;
  delete?: LinkRecord;
  generateVideoUploadUrl?: LinkRecord;
  aiGenerate?: LinkRecord;
  nextLesson?: LinkRecord;
  previousLesson?: LinkRecord;
}

/** Public lesson page (GET /lessons/{id}): page.links */
export interface LessonPageLinks {
  self?: LinkRecord;
  course?: LinkRecord;
  nextLesson?: LinkRecord;
  previousLesson?: LinkRecord;
  markAsComplete?: LinkRecord;
  unmarkAsComplete?: LinkRecord;
  manage?: LinkRecord;
}

/** Enrolled course: item.links */
export interface EnrolledCourseLinks {
  viewCourse?: LinkRecord;
  continueLearning?: LinkRecord;
}

/** Enrolled courses: collection.links */
export interface GetEnrolledCoursesCollectionLinks {
  self?: LinkRecord;
  browseCatalog?: LinkRecord;
  next?: LinkRecord;
  prev?: LinkRecord;
}

/** Course analytics page: page.links */
export interface GetCourseAnalyticsLinks {
  self?: LinkRecord;
  course?: LinkRecord;
  managedCourse?: LinkRecord;
}
