import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Star, Pencil, Trash2 } from "lucide-react";
import { Card, CardContent, Avatar, Button } from "@/shared/ui";
import ConfirmationModal from "@/components/ui/ConfirmationModal";
import { getLinkFromRecord, linkDtoArrayToRecord } from "@/shared/utils";
import type { CourseRatingDto } from "@/domain/courses";
import { EditRatingDialog } from "./EditRatingDialog";

interface RatingCardProps {
  rating: CourseRatingDto;
  onPatch: (url: string, request: { score?: number; comment?: string }) => Promise<void>;
  onDelete: (url: string) => Promise<void>;
  isPatchPending?: boolean;
  isDeletePending?: boolean;
}

function formatDate(dateStr: string, locale: string): string {
  try {
    const date = new Date(dateStr);
    if (isNaN(date.getTime())) return "";
    return new Intl.DateTimeFormat(locale, {
      dateStyle: "medium",
      timeStyle: "short",
    }).format(date);
  } catch {
    return "";
  }
}

export function RatingCard({
  rating,
  onPatch,
  onDelete,
  isPatchPending = false,
  isDeletePending = false,
}: RatingCardProps) {
  const { t, i18n } = useTranslation(["course-management", "translation"]);
  const [isEditOpen, setIsEditOpen] = useState(false);
  const [isDeleteOpen, setIsDeleteOpen] = useState(false);

  const ratingLinks = Array.isArray(rating.links)
    ? linkDtoArrayToRecord(rating.links)
    : (rating as unknown as { links?: Record<string, { href?: string; method?: string }> }).links;
  const updateLink = ratingLinks
    ? getLinkFromRecord(ratingLinks, "update") ?? getLinkFromRecord(ratingLinks, "partialUpdate")
    : undefined;
  const deleteLink = ratingLinks ? getLinkFromRecord(ratingLinks, "delete") : undefined;
  const canUpdate = !!updateLink?.href;
  const canDelete = !!deleteLink?.href;

  const userName =
    [rating.user?.firstName, rating.user?.lastName].filter(Boolean).join(" ") ||
    t("common.unknown");

  const formattedDate = formatDate(
    rating.updatedAt || rating.createdAt,
    i18n.language || "he"
  );

  const handleSave = async (request: { score?: number; comment?: string }) => {
    if (!updateLink?.href) return;
    await onPatch(updateLink.href, request);
    setIsEditOpen(false);
  };

  const handleConfirmDelete = async () => {
    if (!deleteLink?.href) return;
    await onDelete(deleteLink.href);
    setIsDeleteOpen(false);
  };

  return (
    <>
      <Card dir={i18n.dir()}>
        <CardContent className="pt-6">
          <div className="flex gap-4">
            <Avatar className="h-10 w-10 shrink-0">
              {rating.user?.avatarUrl ? (
                <img
                  src={rating.user?.avatarUrl ?? ""}
                  alt={userName}
                  className="h-full w-full object-cover"
                />
              ) : (
                <div className="flex h-full w-full items-center justify-center bg-primary/10 text-primary text-sm font-medium">
                  {userName.charAt(0) || "?"}
                </div>
              )}
            </Avatar>
            <div className="min-w-0 flex-1 space-y-2">
              <div className="flex flex-wrap items-center justify-between gap-2">
                <div className="flex items-center gap-2">
                  <span className="font-medium">{userName}</span>
                  <div className="flex items-center gap-1">
                    {[1, 2, 3, 4, 5].map((i) => (
                      <Star
                        key={i}
                        className={`h-4 w-4 ${
                          i <= rating.rating
                            ? "fill-yellow-400 text-yellow-400"
                            : "text-muted-foreground"
                        }`}
                      />
                    ))}
                  </div>
                </div>
                <div className="flex items-center gap-2">
                  {canUpdate && (
                    <Button
                      variant="ghost"
                      size="sm"
                      className="h-8 w-8 p-0"
                      onClick={() => setIsEditOpen(true)}
                    >
                      <Pencil className="h-4 w-4" />
                    </Button>
                  )}
                  {canDelete && (
                    <Button
                      variant="ghost"
                      size="sm"
                      className="h-8 w-8 p-0 text-destructive hover:text-destructive"
                      onClick={() => setIsDeleteOpen(true)}
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  )}
                </div>
              </div>
              {rating.comment && (
                <p className="text-sm text-muted-foreground" dir="auto">
                  {rating.comment}
                </p>
              )}
              {formattedDate && (
                <p className="text-xs text-muted-foreground">{formattedDate}</p>
              )}
            </div>
          </div>
        </CardContent>
      </Card>

      <EditRatingDialog
        rating={rating}
        open={isEditOpen}
        onOpenChange={setIsEditOpen}
        onSave={handleSave}
        isPending={isPatchPending}
      />

      <ConfirmationModal
        open={isDeleteOpen}
        onOpenChange={setIsDeleteOpen}
        onConfirm={handleConfirmDelete}
        title={t("course-management:ratings.deleteConfirmTitle")}
        message={t("course-management:ratings.deleteConfirmMessage")}
        confirmText={t("common.delete")}
        cancelText={t("common.cancel")}
        isLoading={isDeletePending}
      />
    </>
  );
}
