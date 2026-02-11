/**
 * HATEOAS relation names for module operations
 * Note: Modules are part of the courses domain
 */
export const ModuleRels = {
  SELF: "self",
  CREATE_LESSON: "create-lesson",
  PARTIAL_UPDATE: "partial-update",
  DELETE: "delete",
  REORDER_LESSONS: "reorder-lessons",
} as const;
