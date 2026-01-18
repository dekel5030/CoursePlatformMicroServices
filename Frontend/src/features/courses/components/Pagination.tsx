import { Button } from "@/components/ui/button";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { getLink, CourseRels } from "@/utils/linkHelpers";
import type { LinkDto } from "@/types/LinkDto";
import { useTranslation } from "react-i18next";

interface PaginationProps {
  links?: LinkDto[];
  onNavigate: (url: string) => void;
  isLoading?: boolean;
}

export function Pagination({ links, onNavigate, isLoading }: PaginationProps) {
  const { t } = useTranslation();
  const previousLink = getLink(links, CourseRels.PREVIOUS_PAGE);
  const nextLink = getLink(links, CourseRels.NEXT_PAGE);

  // Don't render anything if there are no pagination links
  if (!previousLink && !nextLink) {
    return null;
  }

  return (
    <div className="flex items-center justify-center gap-2 mt-8">
      <Button
        variant="outline"
        size="sm"
        onClick={() => previousLink && onNavigate(previousLink.href)}
        disabled={!previousLink || isLoading}
        className="gap-1"
      >
        <ChevronLeft className="h-4 w-4" />
        {t("common.previous")}
      </Button>
      <Button
        variant="outline"
        size="sm"
        onClick={() => nextLink && onNavigate(nextLink.href)}
        disabled={!nextLink || isLoading}
        className="gap-1"
      >
        {t("common.next")}
        <ChevronRight className="h-4 w-4" />
      </Button>
    </div>
  );
}
