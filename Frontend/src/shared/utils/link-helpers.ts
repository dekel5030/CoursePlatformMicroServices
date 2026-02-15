import type { LinkDto } from "@/shared/types/LinkDto";
import type { LinkRecord } from "@/shared/types/LinkRecord";

/**
 * Legacy: array-based link helpers for APIs that still return links as LinkDto[].
 * Prefer getLinkFromRecord / hasLinkInRecord when the API returns links as an object
 * keyed by relation (camelCase). Use linkDtoArrayToRecord to normalize array → record
 * when supporting both shapes.
 */

/**
 * Check if a specific link relation exists in the links array (legacy array shape).
 * @param links - Array of HATEOAS links
 * @param rel - The relation name to check for (e.g., "partial-update", "delete", "create")
 * @returns true if the link exists, false otherwise
 */
export function hasLink(links: LinkDto[] | undefined, rel: string): boolean {
  if (!links || links.length === 0) {
    return false;
  }
  return links.some((link) => link.rel === rel);
}

/**
 * Get a specific link by relation name (legacy array shape).
 * @param links - Array of HATEOAS links
 * @param rel - The relation name to find
 * @returns The link if found, undefined otherwise
 */
export function getLink(
  links: LinkDto[] | undefined,
  rel: string,
): LinkDto | undefined {
  if (!links || links.length === 0) {
    return undefined;
  }
  return links.find((link) => link.rel === rel);
}

/**
 * Check if a link exists in a strongly-typed links record (rel is the key, camelCase).
 */
export function hasLinkInRecord(
  links: Record<string, LinkRecord | undefined> | undefined,
  key: string,
): boolean {
  if (!links || typeof links !== "object") return false;
  const link = links[key];
  return !!link?.href;
}

/**
 * Get a link from a strongly-typed links record (rel is the key, camelCase).
 */
export function getLinkFromRecord(
  links: Record<string, LinkRecord | undefined> | undefined,
  key: string,
): LinkRecord | undefined {
  if (!links || typeof links !== "object") return undefined;
  return links[key];
}

/** Convert kebab-case rel to camelCase (e.g. "partial-update" -> "partialUpdate") */
function relToCamel(rel: string): string {
  return rel.replace(/-([a-z])/g, (_, c) => c.toUpperCase());
}

/**
 * Convert legacy LinkDto[] to LinksRecord (for backward compatibility).
 */
export function linkDtoArrayToRecord(
  links: LinkDto[] | undefined,
): Record<string, LinkRecord | undefined> {
  if (!links?.length) return {};
  return links.reduce<Record<string, LinkRecord>>((acc, link) => {
    const key = relToCamel(link.rel);
    acc[key] = { href: link.href, method: link.method };
    return acc;
  }, {});
}

function getPathFromHref(href: string): string {
  if (href.startsWith("http://") || href.startsWith("https://")) {
    try {
      return new URL(href).pathname;
    } catch {
      return href;
    }
  }
  return href.split("?")[0];
}

export interface ApiHrefContext {
  courseId?: string;
}

export function apiHrefToAppRoute(
  href: string,
  context?: ApiHrefContext,
): string | null {
  const path = getPathFromHref(href);
  const coursesIdMatch = path.match(/^\/api\/courses\/([^/]+)$/);
  if (coursesIdMatch) return `/courses/${coursesIdMatch[1]}`;
  const manageCoursesIdMatch = path.match(/^\/api\/manage\/courses\/([^/]+)$/);
  if (manageCoursesIdMatch)
    return `/manage/courses/${manageCoursesIdMatch[1]}`;
  const manageCoursesAnalyticsMatch = path.match(
    /^\/api\/manage\/courses\/([^/]+)\/analytics$/,
  );
  if (manageCoursesAnalyticsMatch)
    return `/manage/courses/${manageCoursesAnalyticsMatch[1]}/analytics`;
  const lessonsIdMatch = path.match(/^\/api\/lessons\/([^/]+)$/);
  if (lessonsIdMatch && context?.courseId)
    return `/courses/${context.courseId}/lessons/${lessonsIdMatch[1]}`;
  const manageLessonsIdMatch = path.match(/^\/api\/manage\/lessons\/([^/]+)$/);
  if (manageLessonsIdMatch && context?.courseId)
    return `/manage/courses/${context.courseId}/lessons/${manageLessonsIdMatch[1]}`;
  const manageCourseLessonsMatch = path.match(
    /^\/api\/manage\/courses\/([^/]+)\/lessons\/([^/]+)$/
  );
  if (manageCourseLessonsMatch)
    return `/manage/courses/${manageCourseLessonsMatch[1]}/lessons/${manageCourseLessonsMatch[2]}`;
  const catalogMatch = path.match(/^\/api\/courses$/);
  if (catalogMatch) return "/catalog";
  const enrolledMatch = path.match(/^\/api\/users\/me\/courses\/enrolled/);
  if (enrolledMatch) return "/users/me/courses/enrolled";
  const courseRatingsMatch = path.match(/^\/api\/courses\/([^/]+)\/ratings/);
  if (courseRatingsMatch) return `/courses/${courseRatingsMatch[1]}#ratings`;
  return null;
}

export const LINK_LABELS: Record<string, string> = {
  self: "Self",
  coursePage: "View course",
  analytics: "Analytics",
  partialUpdate: "Edit",
  delete: "Delete",
  publish: "Publish",
  generateImageUploadUrl: "Upload image",
  createModule: "Add module",
  changePosition: "Reorder",
  manage: "Manage",
  ratings: "Reviews",
  viewCourse: "View course",
  continueLearning: "Continue",
  create: "Add rating",
  update: "Edit",
  managedCourse: "Back to course",
  publicPreview: "Preview course",
  generateVideoUploadUrl: "Upload video",
  aiGenerate: "Generate with AI",
  nextLesson: "Next lesson",
  previousLesson: "Previous lesson",
  course: "Back to course",
  markAsComplete: "Mark complete",
  unmarkAsComplete: "Unmark complete",
  createLesson: "Add lesson",
  browseCatalog: "Browse catalog",
  next: "Next",
  prev: "Previous",
};
