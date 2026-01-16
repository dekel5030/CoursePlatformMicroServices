import type { LinkDto } from "@/types/LinkDto";

/**
 * Check if a specific link relation exists in the links array
 * @param links - Array of HATEOAS links
 * @param rel - The relation name to check for (e.g., "partial-update", "delete", "create")
 * @returns true if the link exists, false otherwise
 */
export function hasLink(links: LinkDto[] | undefined, rel: string): boolean {
  if (!links || links.length === 0) {
    return false;
  }
  return links.some(link => link.rel === rel);
}

/**
 * Get a specific link by relation name
 * @param links - Array of HATEOAS links
 * @param rel - The relation name to find
 * @returns The link if found, undefined otherwise
 */
export function getLink(links: LinkDto[] | undefined, rel: string): LinkDto | undefined {
  if (!links || links.length === 0) {
    return undefined;
  }
  return links.find(link => link.rel === rel);
}

/**
 * Relation names used in the API for course operations
 */
export const CourseRels = {
  SELF: "self",
  PARTIAL_UPDATE: "partial-update",
  DELETE: "delete",
  CREATE: "create",
  CREATE_LESSON: "create-lesson",
  NEXT_PAGE: "next-page",
  PREVIOUS_PAGE: "previous-page",
} as const;

/**
 * Relation names used in the API for lesson operations
 */
export const LessonRels = {
  SELF: "self",
  PARTIAL_UPDATE: "partial-update",
  DELETE: "delete",
} as const;
