import { useMemo } from "react";
import {
  getLinkFromRecord,
  apiHrefToAppRoute,
  type ApiHrefContext,
} from "@/shared/utils";
import type { LinksRecord } from "@/shared/types/LinkRecord";

/**
 * Hook to resolve a HATEOAS link by relation name.
 * Returns the link record, whether it exists, and the app route (if resolvable).
 */
export function useHateoasLink(
  links: LinksRecord | undefined,
  rel: string,
  context?: ApiHrefContext
) {
  return useMemo(() => {
    const link = getLinkFromRecord(links, rel);
    const hasLink = !!link?.href;
    const route = link?.href ? apiHrefToAppRoute(link.href, context) : null;
    return { link, hasLink, route };
  }, [links, rel, context?.courseId]);
}
