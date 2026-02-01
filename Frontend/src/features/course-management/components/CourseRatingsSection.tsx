import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Card, CardHeader, CardTitle, CardContent, Skeleton } from "@/shared/ui";
import { hasLink, getLink } from "@/shared/utils";
import { CourseRels, CourseRatingRels } from "@/domain/courses";
import {
  useCourseRatings,
  useCreateRating,
  usePatchRating,
  useDeleteRating,
} from "@/domain/courses";
import { Pagination } from "@/features/course-catalog/components/Pagination";
import { AddRatingForm } from "./AddRatingForm";
import { RatingCard } from "./RatingCard";
import { toast } from "sonner";
import type { CourseModel } from "@/domain/courses";

interface CourseRatingsSectionProps {
  course: CourseModel;
}

export function CourseRatingsSection({ course }: CourseRatingsSectionProps) {
  const { t, i18n } = useTranslation(["course-management", "translation"]);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageUrl, setPageUrl] = useState<string | undefined>(undefined);
  const pageSize = 10;

  const ratingsLink = getLink(course.links, CourseRels.RATINGS);
  const createRatingLink = getLink(course.links, CourseRels.CREATE_RATING);

  const ratingsUrl = ratingsLink?.href;
  const canAddRating = hasLink(course.links, CourseRels.CREATE_RATING);

  const {
    data: ratingsData,
    isLoading,
    isFetching,
  } = useCourseRatings(course.id, ratingsUrl, {
    pageNumber,
    pageSize,
    pageUrl,
  });

  const createRating = useCreateRating(course.id);
  const patchRating = usePatchRating(course.id);
  const deleteRating = useDeleteRating(course.id);

  const handleNavigate = (url: string) => {
    setPageUrl(url);
  };

  const handleCreateRating = async (request: { score: number; comment?: string }) => {
    if (!createRatingLink) return;
    try {
      await createRating.mutateAsync({
        url: createRatingLink.href,
        request: { score: request.score, comment: request.comment },
      });
      toast.success(t("course-management:ratings.createSuccess"));
    } catch (error) {
      toast.error(
        t("course-management:ratings.error", {
          message: error instanceof Error ? error.message : String(error),
        })
      );
    }
  };

  const handlePatchRating = async (
    url: string,
    request: { score?: number; comment?: string }
  ) => {
    try {
      await patchRating.mutateAsync({ url, request });
      toast.success(t("course-management:ratings.updateSuccess"));
    } catch (error) {
      toast.error(
        t("course-management:ratings.error", {
          message: error instanceof Error ? error.message : String(error),
        })
      );
      throw error;
    }
  };

  const handleDeleteRating = async (url: string) => {
    try {
      await deleteRating.mutateAsync(url);
      toast.success(t("course-management:ratings.deleteSuccess"));
    } catch (error) {
      toast.error(
        t("course-management:ratings.error", {
          message: error instanceof Error ? error.message : String(error),
        })
      );
      throw error;
    }
  };

  if (!ratingsLink) {
    return null;
  }

  const useLinkPagination =
    ratingsData?.links &&
    (hasLink(ratingsData.links, CourseRatingRels.NEXT_PAGE) ||
      hasLink(ratingsData.links, CourseRatingRels.PREVIOUS_PAGE));

  return (
    <Card dir={i18n.dir()}>
      <CardHeader>
        <CardTitle className="text-start">
          {t("course-management:ratings.sectionTitle")}
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-6">
        {canAddRating && (
          <AddRatingForm
            onSubmit={handleCreateRating}
            isPending={createRating.isPending}
          />
        )}

        {isLoading ? (
          <div className="space-y-4">
            <Skeleton className="h-24 w-full" />
            <Skeleton className="h-24 w-full" />
            <Skeleton className="h-24 w-full" />
          </div>
        ) : !ratingsData?.items?.length ? (
          <p className="text-sm text-muted-foreground text-center py-8">
            {t("course-management:ratings.noRatings")}
          </p>
        ) : (
          <div className="space-y-4">
            {ratingsData.items.map((rating) => (
              <RatingCard
                key={rating.id}
                rating={rating}
                onPatch={handlePatchRating}
                onDelete={handleDeleteRating}
                isPatchPending={patchRating.isPending}
                isDeletePending={deleteRating.isPending}
              />
            ))}

            {useLinkPagination ? (
              <Pagination
                links={ratingsData.links}
                onNavigate={handleNavigate}
                isLoading={isFetching}
              />
            ) : (
              (() => {
                const totalPages =
                  ratingsData.totalPages ??
                  Math.ceil(
                    (ratingsData.totalItems || 0) / (ratingsData.pageSize || pageSize)
                  );
                return totalPages > 1 ? (
                  <div className="flex items-center justify-center gap-2 mt-8">
                    <button
                      type="button"
                      className="px-4 py-2 text-sm font-medium rounded-md border border-input bg-background hover:bg-accent disabled:opacity-50"
                      disabled={pageNumber <= 1 || isFetching}
                      onClick={() => {
                        setPageUrl(undefined);
                        setPageNumber((p) => Math.max(1, p - 1));
                      }}
                    >
                      {t("common.previous")}
                    </button>
                    <span className="text-sm text-muted-foreground px-2">
                      {pageNumber} / {totalPages}
                    </span>
                    <button
                      type="button"
                      className="px-4 py-2 text-sm font-medium rounded-md border border-input bg-background hover:bg-accent disabled:opacity-50"
                      disabled={pageNumber >= totalPages || isFetching}
                      onClick={() => {
                        setPageUrl(undefined);
                        setPageNumber((p) => p + 1);
                      }}
                    >
                      {t("common.next")}
                    </button>
                  </div>
                ) : null;
              })()
            )}
          </div>
        )}
      </CardContent>
    </Card>
  );
}
