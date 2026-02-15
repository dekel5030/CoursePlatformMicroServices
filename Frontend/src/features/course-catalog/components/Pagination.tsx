import { Button } from "@/shared/ui";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { getLink, getLinkFromRecord } from "@/shared/utils";
import { CourseRels } from "@/domain/courses";
import type { LinkDto } from "@/shared/types";
import type { LinkRecord } from "@/shared/types/LinkRecord";
import { useTranslation } from "react-i18next";
import i18n from "@/i18n";

interface PaginationProps {
  /** Legacy: array of links (next-page, previous-page) */
  links?: LinkDto[];
  /** Strongly-typed collection links with prev, next (camelCase) */
  collectionLinks?: Record<string, LinkRecord | undefined>;
  onNavigate: (url: string) => void;
  isLoading?: boolean;
}

export function Pagination({
  links,
  collectionLinks,
  onNavigate,
  isLoading,
}: PaginationProps) {
  const { t } = useTranslation();
  const previousLink = collectionLinks
    ? getLinkFromRecord(collectionLinks, "prev")
    : getLink(links, CourseRels.PREVIOUS_PAGE);
  const nextLink = collectionLinks
    ? getLinkFromRecord(collectionLinks, "next")
    : getLink(links, CourseRels.NEXT_PAGE);

  const isRtl = i18n.dir() === "rtl";
  const PreviousIcon = isRtl ? ChevronRight : ChevronLeft;
  const NextIcon = isRtl ? ChevronLeft : ChevronRight;

  if (!previousLink && !nextLink) {
    return null;
  }

  return (
    <div className="flex items-center justify-center gap-2 mt-8">
      <Button
        variant="outline"
        size="sm"
        onClick={() => previousLink?.href && onNavigate(previousLink.href)}
        disabled={!previousLink?.href || isLoading}
        className="gap-1"
      >
        <PreviousIcon className="h-4 w-4" />
        {t("common.previous")}
      </Button>
      <Button
        variant="outline"
        size="sm"
        onClick={() => nextLink?.href && onNavigate(nextLink.href)}
        disabled={!nextLink?.href || isLoading}
        className="gap-1"
      >
        {t("common.next")}
        <NextIcon className="h-4 w-4" />
      </Button>
    </div>
  );
}
