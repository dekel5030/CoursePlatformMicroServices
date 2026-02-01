import { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import { Star } from "lucide-react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from "@/shared/ui";
import { Button } from "@/shared/ui";
import { hasLink, getLink } from "@/shared/utils";
import { CourseRatingRels } from "@/domain/courses";
import type { CourseRatingDto, UpdateCourseRatingRequest } from "@/domain/courses";

interface EditRatingDialogProps {
  rating: CourseRatingDto | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSave: (request: UpdateCourseRatingRequest) => Promise<void>;
  isPending: boolean;
}

export function EditRatingDialog({
  rating,
  open,
  onOpenChange,
  onSave,
  isPending,
}: EditRatingDialogProps) {
  const { t, i18n } = useTranslation(["course-management", "translation"]);
  const [score, setScore] = useState(0);
  const [hoverScore, setHoverScore] = useState(0);
  const [comment, setComment] = useState("");

  const canUpdate = rating && hasLink(rating.links, CourseRatingRels.PARTIAL_UPDATE);
  const updateLink = rating ? getLink(rating.links, CourseRatingRels.PARTIAL_UPDATE) : null;

  useEffect(() => {
    if (rating) {
      setScore(rating.rating);
      setComment(rating.comment || "");
    }
  }, [rating]);

  const displayScore = hoverScore || score;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!canUpdate || !updateLink) return;
    await onSave({ score, comment: comment.trim() || undefined });
    onOpenChange(false);
  };

  const handleClose = () => {
    setHoverScore(0);
    onOpenChange(false);
  };

  if (!rating) return null;

  return (
    <Dialog open={open} onOpenChange={handleClose}>
      <DialogContent className="sm:max-w-[425px]" dir={i18n.dir()}>
        <DialogHeader dir={i18n.dir()}>
          <DialogTitle dir={i18n.dir()}>
            {t("course-management:ratings.editDialog.title")}
          </DialogTitle>
          <DialogDescription>
            {t("course-management:ratings.editDialog.description")}
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} dir={i18n.dir()}>
          <div className="grid gap-4 py-4">
            <div className="space-y-2">
              <label className="text-sm font-medium" dir={i18n.dir()}>
                {t("course-management:ratings.scoreLabel")}
              </label>
              <div className="flex gap-1">
                {[1, 2, 3, 4, 5].map((value) => (
                  <button
                    key={value}
                    type="button"
                    className="p-1 transition-transform hover:scale-110 focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2 rounded"
                    onClick={() => setScore(value)}
                    onMouseEnter={() => setHoverScore(value)}
                    onMouseLeave={() => setHoverScore(0)}
                  >
                    <Star
                      className={`h-8 w-8 transition-colors ${
                        value <= displayScore
                          ? "fill-yellow-400 text-yellow-400"
                          : "text-muted-foreground"
                      }`}
                    />
                  </button>
                ))}
              </div>
            </div>
            <div className="space-y-2">
              <label
                htmlFor="edit-rating-comment"
                className="text-sm font-medium"
                dir={i18n.dir()}
              >
                {t("course-management:ratings.commentLabel")}
              </label>
              <textarea
                id="edit-rating-comment"
                value={comment}
                onChange={(e) => setComment(e.target.value)}
                placeholder={t("course-management:ratings.commentPlaceholder")}
                dir={i18n.dir()}
                className="flex min-h-[80px] w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
                maxLength={1000}
              />
            </div>
          </div>
          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={handleClose}
              disabled={isPending}
            >
              {t("common.cancel")}
            </Button>
            <Button type="submit" disabled={isPending}>
              {isPending
                ? t("course-management:ratings.editDialog.saving")
                : t("common.save")}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
