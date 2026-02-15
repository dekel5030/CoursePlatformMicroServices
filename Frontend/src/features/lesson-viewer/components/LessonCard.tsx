import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import type { LessonModel } from "@/domain/lessons";
import { Card, CardContent, Badge, Button } from "@/shared/ui";
import { InlineEditableText } from "@/shared/common";
import ConfirmationModal from "@/components/ui/ConfirmationModal";
import { Clock, Trash2, Play, Lock, GripVertical } from "lucide-react";
import { usePatchLesson, useDeleteLesson } from "@/domain/lessons";
import { toast } from "sonner";
import { getLinkFromRecord, formatDuration, apiHrefToAppRoute } from "@/shared/utils";

interface LessonProps {
  lesson: LessonModel;
  index: number;
  courseId: string;
  dragHandleProps?: React.HTMLAttributes<HTMLButtonElement>;
}

export default function LessonCard({
  lesson,
  index,
  courseId,
  dragHandleProps,
}: LessonProps) {
  const navigate = useNavigate();
  const { t } = useTranslation(["lesson-viewer", "translation"]);

  const patchLesson = usePatchLesson(courseId, lesson.lessonId);
  const deleteLesson = useDeleteLesson(lesson.courseId);

  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);

  const updateLink = getLinkFromRecord(lesson.links, "partialUpdate");
  const deleteLink = getLinkFromRecord(lesson.links, "delete");
  const selfLink = getLinkFromRecord(lesson.links, "self");
  const manageLink = getLinkFromRecord(lesson.links, "manage");
  const canUpdate = !!updateLink?.href;
  const canDelete = !!deleteLink?.href;

  const durationText = formatDuration(lesson.duration);

  const lessonRoute = `/courses/${courseId}/lessons/${lesson.lessonId}`;
  const manageRoute = manageLink?.href
    ? apiHrefToAppRoute(manageLink.href, { courseId }) ?? null
    : null;

  const handleLessonClick = () => {
    if (manageRoute) {
      navigate(manageRoute, { state: { lessonSelfLink: manageLink?.href } });
      return;
    }
    navigate(lessonRoute, {
      state: { lessonSelfLink: selfLink?.href },
    });
  };

  const handleManageClick = (e: React.MouseEvent) => {
    e.stopPropagation();
    if (manageRoute) {
      navigate(manageRoute, { state: { lessonSelfLink: manageLink?.href } });
    } else {
      handleLessonClick();
    }
  };

  const handleDeleteClick = (e: React.MouseEvent) => {
    e.stopPropagation();
    setIsDeleteDialogOpen(true);
  };

  const handleConfirmDelete = async () => {
    if (!deleteLink?.href) return;
    try {
      await deleteLesson.mutateAsync(deleteLink.href);
    } finally {
      setIsDeleteDialogOpen(false);
    }
  };

  const handleTitleUpdate = async (newTitle: string) => {
    if (!updateLink?.href) return;
    try {
      await patchLesson.mutateAsync({
        url: updateLink.href,
        request: { title: newTitle },
      });
      toast.success(t("lesson-viewer:actions.titleUpdated"));
    } catch (error) {
      toast.error(t("lesson-viewer:actions.titleUpdateFailed"));
      throw error;
    }
  };

  return (
    <>
      <Card
        className="cursor-pointer hover:bg-muted/50 transition-colors border-s-2 border-s-transparent hover:border-s-primary/30 rounded-md"
        onClick={handleLessonClick}
        role="button"
        tabIndex={0}
        onKeyDown={(e) => {
          const target = e.target as HTMLElement;
          if (target.tagName === "INPUT" || target.tagName === "TEXTAREA") return;
          if (e.key === "Enter" || e.key === " ") handleLessonClick();
        }}
      >
        <CardContent className="py-2 px-4">
          <div className="flex items-center gap-3">
            {/* Drag handle */}
            {dragHandleProps && (
              <button
                type="button"
                {...dragHandleProps}
                className="shrink-0 flex h-8 w-8 items-center justify-center rounded cursor-grab active:cursor-grabbing text-muted-foreground hover:bg-muted/80 touch-none"
                onClick={(e) => e.stopPropagation()}
                aria-label={t("lesson-viewer:card.dragHandle")}
              >
                <GripVertical className="h-4 w-4" />
              </button>
            )}
            {/* Play / Lock icon */}
            <div className="shrink-0 flex h-8 w-8 items-center justify-center rounded-full bg-muted">
              {lesson.isPreview ? (
                <Play className="h-4 w-4 text-primary fill-primary" />
              ) : (
                <Lock className="h-3.5 w-3.5 text-muted-foreground" />
              )}
            </div>

            {/* Title */}
            <div className="flex-1 min-w-0 text-start">
              <div className="flex items-center gap-2">
                <span className="text-xs text-muted-foreground shrink-0">{index + 1}.</span>
                {canUpdate ? (
                  <div className="flex-1 min-w-0" onClick={(e) => e.stopPropagation()}>
                    <InlineEditableText
                      value={lesson.title}
                      onSave={handleTitleUpdate}
                      displayClassName="font-medium text-sm text-start truncate"
                      inputClassName="font-medium text-sm text-start"
                      placeholder={t("lesson-viewer:actions.enterTitle")}
                      maxLength={200}
                    />
                  </div>
                ) : (
                  <h3 className="font-medium text-sm line-clamp-1 text-start truncate" dir="auto">
                    {lesson.title}
                  </h3>
                )}
              </div>
            </div>

            <div className="flex items-center gap-2 shrink-0">
              {lesson.isPreview && (
                <Badge variant="secondary" className="text-xs h-5 font-normal">
                  {t("lesson-viewer:card.preview")}
                </Badge>
              )}
              {durationText && (
                <span className="flex items-center gap-1 text-xs text-muted-foreground">
                  <Clock className="h-3 w-3 shrink-0" />
                  {durationText}
                </span>
              )}
              {(manageLink?.href || selfLink?.href) && (
                <Button
                  variant="outline"
                  size="sm"
                  className="h-7 text-xs shrink-0"
                  onClick={handleManageClick}
                >
                  {manageLink?.href
                    ? t("lesson-viewer:card.manage", { defaultValue: "Open" })
                    : t("lesson-viewer:card.view", { defaultValue: "View" })}
                </Button>
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
        title={t("lesson-viewer:actions.deleteConfirmTitle")}
        message={t("lesson-viewer:actions.deleteConfirmMessage")}
        confirmText={t("common.delete")}
        cancelText={t("common.cancel")}
        isLoading={deleteLesson.isPending}
      />
    </>
  );
}
