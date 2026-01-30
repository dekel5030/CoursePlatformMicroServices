import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import type { LessonModel } from "@/domain/lessons";
import {
  Card,
  CardContent,
  Badge,
  Button,
} from "@/shared/ui";
import { InlineEditableText } from "@/shared/common";
import ConfirmationModal from "@/components/ui/ConfirmationModal";
import { Clock, Trash2, Play, Lock } from "lucide-react";
import { usePatchLesson, useDeleteLesson } from "@/domain/lessons";
import { toast } from "sonner";
import { hasLink, getLink } from "@/shared/utils";
import { LessonRels } from "@/domain/lessons";

interface LessonProps {
  lesson: LessonModel;
  index: number;
  courseId: string;
}

export default function LessonCard({ lesson, index, courseId }: LessonProps) {
  const navigate = useNavigate();
  const { t, i18n } = useTranslation(["lessons", "translation"]);

  const patchLesson = usePatchLesson(courseId, lesson.lessonId);
  const deleteLesson = useDeleteLesson(lesson.courseId);

  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);

  const isRTL = i18n.dir() === "rtl";
  const textAlignClass = isRTL ? "text-right" : "text-left";

  // Determine available actions based on HATEOAS links
  const canUpdate = hasLink(lesson.links, LessonRels.PARTIAL_UPDATE);
  const canDelete = hasLink(lesson.links, LessonRels.DELETE);
  const updateLink = getLink(lesson.links, LessonRels.PARTIAL_UPDATE);
  const deleteLink = getLink(lesson.links, LessonRels.DELETE);

  const formatDuration = (duration: string | null | undefined) => {
    if (!duration) return null;
    const parts = duration.split(":");
    return `${parseInt(parts[1])}m ${parseInt(parts[2])}s`;
  };

  const handleLessonClick = () => {
    const selfLink = getLink(lesson.links, LessonRels.SELF);
    navigate(`/courses/${courseId}/lessons/${lesson.lessonId}`, {
      state: { lessonSelfLink: selfLink?.href },
    });
  };

  const handleDeleteClick = (e: React.MouseEvent) => {
    e.stopPropagation();
    setIsDeleteDialogOpen(true);
  };

  const handleConfirmDelete = async () => {
    if (!deleteLink) {
      console.error("No delete link found for this lesson");
      return;
    }

    try {
      await deleteLesson.mutateAsync(deleteLink.href);
    } finally {
      setIsDeleteDialogOpen(false);
    }
  };

  const handleTitleUpdate = async (newTitle: string) => {
    if (!updateLink) {
      console.error("No update link found for this lesson");
      return;
    }

    try {
      await patchLesson.mutateAsync({
        url: updateLink.href,
        request: { title: newTitle },
      });
      toast.success(t("lessons:actions.titleUpdated"));
    } catch (error) {
      toast.error(t("lessons:actions.titleUpdateFailed"));
      throw error;
    }
  };

  const durationText = formatDuration(lesson.duration);

  return (
    <>
      <Card
        className="cursor-pointer hover:bg-accent/50 transition-colors border-l-2 hover:border-l-primary"
        onClick={handleLessonClick}
        role="button"
        tabIndex={0}
        onKeyDown={(e) => {
          const target = e.target as HTMLElement;
          if (target.tagName === "INPUT" || target.tagName === "TEXTAREA") {
            return;
          }
          if (e.key === "Enter" || e.key === " ") {
            handleLessonClick();
          }
        }}
      >
        <CardContent className="py-2.5 px-4">
          <div className="flex items-center gap-3">
            {/* Play Button */}
            <div className="shrink-0">
              <div className="flex h-8 w-8 items-center justify-center rounded-full bg-primary/10 hover:bg-primary/20 transition-colors">
                {lesson.isPreview ? (
                  <Play className="h-4 w-4 text-primary fill-primary" />
                ) : (
                  <Lock className="h-3.5 w-3.5 text-muted-foreground" />
                )}
              </div>
            </div>

            {/* Lesson Info */}
            <div className={`flex-1 min-w-0 ${textAlignClass}`}>
              <div className="flex items-center gap-2">
                <span className="text-xs text-muted-foreground font-medium">
                  {index + 1}.
                </span>
                {canUpdate ? (
                  <div className="flex-1" onClick={(e) => e.stopPropagation()}>
                    <InlineEditableText
                      value={lesson.title}
                      onSave={handleTitleUpdate}
                      displayClassName={`font-medium text-sm ${textAlignClass}`}
                      inputClassName={`font-medium text-sm ${textAlignClass}`}
                      placeholder={t("lessons:actions.enterTitle")}
                      maxLength={200}
                    />
                  </div>
                ) : (
                  <h3
                    className={`font-medium text-sm line-clamp-1 flex-1 ${textAlignClass}`}
                    dir="auto"
                  >
                    {lesson.title}
                  </h3>
                )}
              </div>
            </div>

            {/* Badges & Actions */}
            <div
              className={`flex items-center gap-2 shrink-0 ${isRTL ? "flex-row-reverse" : ""}`}
            >
              {lesson.isPreview && (
                <Badge variant="secondary" className="text-xs h-5">
                  {t("lessons:card.preview")}
                </Badge>
              )}
              {durationText && (
                <div
                  className={`flex items-center gap-1 text-xs text-muted-foreground ${isRTL ? "flex-row-reverse" : ""}`}
                >
                  <Clock className="h-3 w-3" />
                  <span>{durationText}</span>
                </div>
              )}
              {canDelete && (
                <Button
                  variant="ghost"
                  size="sm"
                  className="h-7 w-7 p-0 hover:text-destructive hover:bg-destructive/10"
                  onClick={handleDeleteClick}
                  title={t("common.delete")}
                  disabled={deleteLesson.isPending}
                >
                  <Trash2 className="h-3.5 w-3.5" />
                </Button>
              )}
            </div>
          </div>
        </CardContent>
      </Card>

      <ConfirmationModal
        open={isDeleteDialogOpen}
        onOpenChange={setIsDeleteDialogOpen}
        onConfirm={handleConfirmDelete}
        title={t("lessons:actions.deleteConfirmTitle")}
        message={t("lessons:actions.deleteConfirmMessage")}
        confirmText={t("common.delete")}
        cancelText={t("common.cancel")}
        isLoading={deleteLesson.isPending}
      />
    </>
  );
}
