import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import type { LessonModel } from "@/domain/lessons";
import { Card, CardContent, Badge, Button, Switch } from "@/shared/ui";
import { InlineEditableText } from "@/shared/common";
import ConfirmationModal from "@/components/ui/ConfirmationModal";
import { Clock, Trash2, Play, Lock, GripVertical, LogIn } from "lucide-react";
import { usePatchLesson, useDeleteLesson } from "@/domain/lessons";
import { useAuth } from "@/features/auth/hooks";
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
  const auth = useAuth();
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

  const manageRoute = manageLink?.href
    ? apiHrefToAppRoute(manageLink.href, { courseId }) ?? null
    : null;
  const selfRoute = selfLink?.href
    ? apiHrefToAppRoute(selfLink.href, { courseId }) ?? null
    : null;
  const lessonRoute = manageRoute ?? selfRoute;
  const hasAccessRoute = !!lessonRoute;

  const handleLessonClick = () => {
    if (!hasAccessRoute) {
      if (!auth.isAuthenticated) {
        void auth.signinRedirect();
      }
      return;
    }
    navigate(lessonRoute!, {
      state: {
        lessonSelfLink:
          manageLink?.href ?? selfLink?.href,
      },
    });
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

  const handleAccessChange = async (checked: boolean) => {
    if (!updateLink?.href) return;
    const newAccess = checked ? "Public" : "Private";
    try {
      await patchLesson.mutateAsync({
        url: updateLink.href,
        request: { access: newAccess },
      });
      toast.success(t("lesson-viewer:accessLevel.changeSuccess"));
    } catch (error) {
      toast.error(t("lesson-viewer:accessLevel.changeFailed"));
      throw error;
    }
  };

  return (
    <>
      <Card
        className={`transition-colors border-s-2 rounded-md ${
          hasAccessRoute
            ? "cursor-pointer hover:bg-muted/50 border-s-transparent hover:border-s-primary/30"
            : "border-s-muted opacity-90"
        }`}
        onClick={hasAccessRoute ? handleLessonClick : undefined}
        role={hasAccessRoute ? "button" : undefined}
        tabIndex={hasAccessRoute ? 0 : undefined}
        onKeyDown={
          hasAccessRoute
            ? (e) => {
                const target = e.target as HTMLElement;
                if (target.tagName === "INPUT" || target.tagName === "TEXTAREA")
                  return;
                if (e.key === "Enter" || e.key === " ") handleLessonClick();
              }
            : undefined
        }
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
              {canUpdate ? (
                <div
                  className="flex items-center gap-1.5"
                  onClick={(e) => e.stopPropagation()}
                >
                  <Switch
                    checked={lesson.isPreview}
                    onCheckedChange={handleAccessChange}
                    disabled={patchLesson.isPending}
                    aria-label={t("lesson-viewer:accessLevel.label")}
                  />
                  <span className="text-xs text-muted-foreground whitespace-nowrap">
                    {lesson.isPreview
                      ? t("lesson-viewer:accessLevel.freePreview")
                      : t("lesson-viewer:accessLevel.private")}
                  </span>
                </div>
              ) : lesson.isPreview ? (
                <Badge variant="secondary" className="text-xs h-5 font-normal">
                  {t("lesson-viewer:card.preview")}
                </Badge>
              ) : null}
              {durationText && (
                <span className="flex items-center gap-1 text-xs text-muted-foreground">
                  <Clock className="h-3 w-3 shrink-0" />
                  {durationText}
                </span>
              )}
              {hasAccessRoute ? (
                <Button
                  variant="outline"
                  size="sm"
                  className="h-7 text-xs shrink-0"
                  asChild
                >
                  <Link
                    to={lessonRoute!}
                    state={{
                      lessonSelfLink: manageLink?.href ?? selfLink?.href,
                    }}
                    onClick={(e) => e.stopPropagation()}
                  >
                    {manageLink?.href
                      ? t("lesson-viewer:card.manage", { defaultValue: "Open" })
                      : t("lesson-viewer:card.view", { defaultValue: "View" })}
                  </Link>
                </Button>
              ) : !auth.isAuthenticated ? (
                <Button
                  variant="outline"
                  size="sm"
                  className="h-7 text-xs shrink-0 gap-1"
                  onClick={(e) => {
                    e.stopPropagation();
                    void auth.signinRedirect();
                  }}
                >
                  <LogIn className="h-3 w-3" />
                  {t("lesson-viewer:card.loginToAccess", {
                    defaultValue: "Log in to access",
                  })}
                </Button>
              ) : (
                <Button
                  variant="ghost"
                  size="sm"
                  className="h-7 text-xs shrink-0 text-muted-foreground"
                  disabled
                >
                  {t("lesson-viewer:card.purchaseToAccess", {
                    defaultValue: "Purchase to access",
                  })}
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
