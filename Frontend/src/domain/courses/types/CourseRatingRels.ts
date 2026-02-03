/**
 * HATEOAS relation names for course rating operations
 */
export const CourseRatingRels = {
  SELF: "self",
  PARTIAL_UPDATE: "partial-update",
  DELETE: "delete",
  CREATE_RATING: "create-rating",
  NEXT_PAGE: "next-page",
  PREVIOUS_PAGE: "previous-page",
} as const;
