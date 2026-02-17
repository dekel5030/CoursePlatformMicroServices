import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Star } from "lucide-react";
import { Button, Card, CardContent } from "@/shared/ui";
import type { CreateCourseRatingRequest } from "@/domain/courses";

interface AddRatingFormProps {
  onSubmit: (request: CreateCourseRatingRequest) => void;
  isPending: boolean;
}

export function AddRatingForm({ onSubmit, isPending }: AddRatingFormProps) {
  const { t, i18n } = useTranslation(["course-management", "translation"]);
  const [score, setScore] = useState(0);
  const [hoverScore, setHoverScore] = useState(0);
  const [comment, setComment] = useState("");

  const displayScore = hoverScore || score;

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (score < 1 || score > 5) return;
    onSubmit({ score, comment: comment.trim() || undefined });
    setScore(0);
    setHoverScore(0);
    setComment("");
  };

  return (
    <Card dir={i18n.dir()}>
      <CardContent className="pt-6">
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <label className="text-sm font-medium" dir={i18n.dir()}>
              {t("course-management:ratings.scoreLabel")}
              <span className="text-destructive ms-1">*</span>
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
                  aria-label={`${value} / 5`}
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
              htmlFor="rating-comment"
              className="text-sm font-medium"
              dir={i18n.dir()}
            >
              {t("course-management:ratings.commentLabel")}
            </label>
            <textarea
              id="rating-comment"
              value={comment}
              onChange={(e) => setComment(e.target.value)}
              placeholder={t("course-management:ratings.commentPlaceholder")}
              dir={i18n.dir()}
              className="flex min-h-[80px] w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
              maxLength={1000}
            />
          </div>
          <Button type="submit" disabled={score < 1 || isPending}>
            {isPending
              ? t("course-management:ratings.submitting")
              : t("course-management:ratings.submit")}
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
