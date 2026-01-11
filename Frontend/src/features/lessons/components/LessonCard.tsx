import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import type { LessonModel } from "../types/LessonModel";
import {
  Card,
  CardContent,
  Badge,
  Button,
  InlineEditableText,
} from "@/components";
import ConfirmationModal from "@/components/ui/ConfirmationModal";
import { Clock, Trash2 } from "lucide-react";
import {
  Authorized,
  ActionType,
  ResourceType,
  ResourceId,
} from "@/features/auth";
import { usePatchLesson, useDeleteLesson } from "../hooks/use-lessons";
import { toast } from "sonner";

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
  const formatDuration = (duration: string | null | undefined) => {
    if (!duration) return null;

    const parts = duration.split(":");
    if (parts.length >= 2) {
      const hours = parseInt(parts[0]);
      const minutes = parseInt(parts[1]);

      if (hours > 0) {
        return `${hours}h ${minutes}m`;
      }
      return `${minutes}m`;
    }
    return duration;
  };

  const handleLessonClick = () => {
    navigate(`/courses/${courseId}/lessons/${lesson.lessonId}`);
  };

  const handleDeleteClick = (e: React.MouseEvent) => {
    e.stopPropagation();
    setIsDeleteDialogOpen(true);
  };

  const handleConfirmDelete = async () => {
    try {
      await deleteLesson.mutateAsync(lesson.lessonId);
      toast.success(t("lessons:actions.deleteSuccess"));
    } catch (error) {
      toast.error(t("lessons:actions.deleteFailed"));
      console.error("Failed to delete lesson:", error);
    } finally {
      setIsDeleteDialogOpen(false);
    }
  };

  const handleTitleUpdate = async (newTitle: string) => {
    try {
      await patchLesson.mutateAsync({ title: newTitle });
      toast.success(t("lessons:actions.titleUpdated"));
    } catch (error) {
      toast.error(t("lessons:actions.titleUpdateFailed"));
      throw error;
    }
  };

  const handleDescriptionUpdate = async (newDescription: string) => {
    try {
      await patchLesson.mutateAsync({ description: newDescription });
      toast.success(t("lessons:actions.descriptionUpdated"));
    } catch (error) {
      toast.error(t("lessons:actions.descriptionUpdateFailed"));
      throw error;
    }
  };

  return (
    <>
      <Card
        className="cursor-pointer hover:shadow-md transition-shadow"
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
        <CardContent className="p-4">
          <div className="flex items-start gap-4">
            <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-primary/10 text-primary font-semibold">
              {index + 1}
            </div>

            <div className={`flex-1 space-y-1 min-w-0 ${textAlignClass}`}>
              <div className="flex items-center gap-2 flex-wrap">
                <Authorized
                  action={ActionType.Update}
                  resource={ResourceType.Lesson}
                  resourceId={ResourceId.create(lesson.lessonId)}
                  fallback={
                    <h3
                      className={`font-semibold text-base line-clamp-1 ${textAlignClass}`}
                      dir="auto"
                    >
                      {lesson.title}
                    </h3>
                  }
                >
                  <div className="flex-1" onClick={(e) => e.stopPropagation()}>
                    <InlineEditableText
                      value={lesson.title}
                      onSave={handleTitleUpdate}
                      displayClassName={`font-semibold text-base ${textAlignClass}`}
                      inputClassName={`font-semibold text-base ${textAlignClass}`}
                      placeholder={t("lessons:actions.enterTitle")}
                      maxLength={200}
                    />
                  </div>
                </Authorized>
                {lesson.isPreview && (
                  <Badge variant="secondary" className="text-xs">
                    {t("lessons:card.preview")}
                  </Badge>
                )}
              </div>

              {lesson.description !== null &&
                lesson.description !== undefined && (
                  <Authorized
                    action={ActionType.Update}
                    resource={ResourceType.Lesson}
                    resourceId={ResourceId.create(lesson.lessonId)}
                    fallback={
                      lesson.description ? (
                        <p
                          className={`text-sm text-muted-foreground line-clamp-2 ${textAlignClass}`}
                          dir="auto"
                        >
                          {lesson.description}
                        </p>
                      ) : null
                    }
                  >
                    <div onClick={(e) => e.stopPropagation()}>
                      <InlineEditableText
                        value={lesson.description || ""}
                        onSave={handleDescriptionUpdate}
                        displayClassName={`text-sm text-muted-foreground line-clamp-2 ${textAlignClass}`}
                        inputClassName={`text-sm ${textAlignClass}`}
                        placeholder={t("lessons:actions.enterDescription")}
                        maxLength={500}
                      />
                    </div>
                  </Authorized>
                )}
            </div>

            <div className="flex items-center gap-2 text-sm text-muted-foreground shrink-0">
              {lesson.duration && (
                <div className="flex items-center gap-1">
                  <Clock className="h-4 w-4" />
                  <span>{formatDuration(lesson.duration)}</span>
                </div>
              )}
              <div className="flex gap-1">
                <Authorized
                  action={ActionType.Delete}
                  resource={ResourceType.Lesson}
                  resourceId={ResourceId.create(lesson.lessonId)}
                >
                  <Button
                    variant="ghost"
                    size="sm"
                    className="h-8 w-8 p-0 hover:text-destructive"
                    onClick={handleDeleteClick}
                    title={t("common.delete")}
                    disabled={deleteLesson.isPending}
                  >
                    <Trash2 className="h-4 w-4" />
                  </Button>
                </Authorized>
              </div>
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
