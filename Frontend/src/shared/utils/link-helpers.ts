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
