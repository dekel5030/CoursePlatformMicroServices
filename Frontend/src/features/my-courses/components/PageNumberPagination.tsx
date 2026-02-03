import { Button } from "@/shared/ui";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { useTranslation } from "react-i18next";
import i18n from "@/i18n";

interface PageNumberPaginationProps {
  pageNumber: number;
  totalPages: number;
  totalItems: number;
  onPageChange: (page: number) => void;
  isLoading?: boolean;
}

export function PageNumberPagination({
  pageNumber,
  totalPages,
  totalItems,
  onPageChange,
  isLoading = false,
}: PageNumberPaginationProps) {
  const { t } = useTranslation("translation");
  const isRtl = i18n.dir() === "rtl";
  const PreviousIcon = isRtl ? ChevronRight : ChevronLeft;
  const NextIcon = isRtl ? ChevronLeft : ChevronRight;

  if (totalPages <= 1 && totalItems <= 0) {
    return null;
  }

  return (
    <div className="flex items-center justify-center gap-4 mt-8">
      <Button
        variant="outline"
        size="sm"
        onClick={() => onPageChange(pageNumber - 1)}
        disabled={pageNumber <= 1 || isLoading}
        className="gap-1"
      >
        <PreviousIcon className="h-4 w-4" />
        {t("common.previous")}
      </Button>

      <span className="text-sm text-muted-foreground">
        {t("myCourses.pageOf", {
          current: pageNumber,
          total: totalPages || 1,
        })}
      </span>

      <Button
        variant="outline"
        size="sm"
        onClick={() => onPageChange(pageNumber + 1)}
        disabled={pageNumber >= totalPages || isLoading}
        className="gap-1"
      >
        {t("common.next")}
        <NextIcon className="h-4 w-4" />
      </Button>
    </div>
  );
}
