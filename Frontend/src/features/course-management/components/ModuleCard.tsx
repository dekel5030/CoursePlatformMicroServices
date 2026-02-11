import { useState } from "react";
import { useTranslation } from "react-i18next";
import type { ModuleModel } from "@/domain/courses";
import { Card, CardContent, CardHeader, Button } from "@/shared/ui";
import { ChevronDown, ChevronUp, Plus, Clock, Trash2, GripVertical } from "lucide-react";
import { SortableContext, verticalListSortingStrategy } from "@dnd-kit/sortable";
import { LessonCard } from "@/features/lesson-viewer";
import { SortableLessonItem, lessonSortableId } from "./SortableLessonItem";
import { hasLink, getLink, formatDuration } from "@/shared/utils";
import { InlineEditableText } from "@/shared/common";
import ConfirmationModal from "@/components/ui/ConfirmationModal";
import { AddLessonDialog } from "@/features/lesson-viewer/components/AddLessonDialog";
import { ModuleRels } from "@/domain/courses";
import { usePatchModule, useDeleteModule } from "@/domain/courses";
import { toast } from "sonner";

interface ModuleCardProps {
  module: ModuleModel;
  courseId: string;
  index: number;
  dragHandleProps?: React.HTMLAttributes<HTMLButtonElement>;
  isDropTarget?: boolean;
}

export function ModuleCard({
  module,
  courseId,
  index,
  dragHandleProps,
  isDropTarget = false,
}: ModuleCardProps) {
  const { t } = useTranslation(["course-management", "translation"]);
  const [isExpanded, setIsExpanded] = useState(true);
  const [isAddLessonOpen, setIsAddLessonOpen] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);

  const patchModule = usePatchModule(courseId);
  const deleteModule = useDeleteModule(courseId);

  const canCreateLesson = hasLink(module.links, ModuleRels.CREATE_LESSON);
  const canUpdate = hasLink(module.links, ModuleRels.PARTIAL_UPDATE);
  const canDelete = hasLink(module.links, ModuleRels.DELETE);
  const canReorderLessons = hasLink(module.links, ModuleRels.REORDER_LESSONS);
  const updateLink = getLink(module.links, ModuleRels.PARTIAL_UPDATE);
  const deleteLink = getLink(module.links, ModuleRels.DELETE);

  const sortedLessons = [...module.lessons].sort((a, b) => a.order - b.order);
  const durationText = formatDuration(module.duration);

  const ChevronIcon = isExpanded ? ChevronUp : ChevronDown;

  const handleTitleUpdate = async (newTitle: string) => {
    if (!updateLink) return;
    try {
      await patchModule.mutateAsync({
        url: updateLink.href,
        request: { title: newTitle },
      });
      toast.success(t("course-management:detail.titleUpdated"));
    } catch (error) {
      toast.error(t("course-management:detail.titleUpdateFailed"));
      throw error;
    }
  };

  const handleDeleteClick = (e: React.MouseEvent) => {
    e.stopPropagation();
    setIsDeleteDialogOpen(true);
  };

  const handleConfirmDelete = async () => {
    if (!deleteLink) return;
    try {
      await deleteModule.mutateAsync(deleteLink.href);
    } finally {
      setIsDeleteDialogOpen(false);
    }
  };

  return (
    <>
      <Card
        className={`overflow-hidden border-s-4 rounded-lg transition-colors ${
          isDropTarget
            ? "border-s-primary bg-primary/5 ring-2 ring-primary/40"
            : "border-s-primary/20"
        }`}
      >
        <CardHeader
          className="cursor-pointer hover:bg-muted/50 transition-colors py-3 px-4"
          onClick={() => setIsExpanded(!isExpanded)}
        >
          <div className="flex items-center justify-between gap-4">
            <div className="flex items-center gap-3 flex-1 min-w-0 text-start">
              {dragHandleProps && (
                <button
                  type="button"
                  {...dragHandleProps}
                  className="shrink-0 flex h-7 w-7 items-center justify-center rounded cursor-grab active:cursor-grabbing text-muted-foreground hover:bg-muted/80 touch-none"
                  onClick={(e) => e.stopPropagation()}
                  aria-label={t("course-management:detail.dragModule")}
                >
                  <GripVertical className="h-4 w-4" />
                </button>
              )}
              <div className="flex h-7 w-7 shrink-0 items-center justify-center rounded-full bg-primary/10 text-primary font-semibold text-sm">
                {index + 1}
              </div>
              <div className="flex-1 min-w-0">
                {canUpdate ? (
                  <div onClick={(e) => e.stopPropagation()}>
                    <InlineEditableText
                      value={module.title}
                      onSave={handleTitleUpdate}
                      displayClassName="font-semibold text-start truncate"
                      inputClassName="font-semibold text-start"
                      placeholder={t("course-management:detail.enterTitle")}
                      maxLength={200}
                    />
                  </div>
                ) : (
                  <h3 className="font-semibold text-start truncate" dir="auto">
                    {module.title}
                  </h3>
                )}
                <div className="flex items-center gap-2 text-xs text-muted-foreground mt-0.5 flex-wrap">
                  <span>
                    {module.lessonCount} {t("course-management:detail.lessons")}
                  </span>
                  {durationText && (
                    <>
                      <span aria-hidden>·</span>
                      <span className="flex items-center gap-1">
                        <Clock className="h-3 w-3 shrink-0" />
                        {durationText}
                      </span>
                    </>
                  )}
                </div>
              </div>
            </div>

            <div className="flex items-center gap-1 shrink-0">
              {canCreateLesson && (
                <Button
                  size="sm"
                  variant="ghost"
                  className="gap-1.5 h-8"
                  onClick={(e) => {
                    e.stopPropagation();
                    setIsAddLessonOpen(true);
                  }}
                >
                  <Plus className="h-3.5 w-3.5" />
                  <span className="text-xs">{t("course-management:detail.addLesson")}</span>
                </Button>
              )}
              {canDelete && (
                <Button
                  variant="ghost"
                  size="sm"
                  className="h-8 w-8 p-0 shrink-0 hover:text-destructive hover:bg-destructive/10"
                  onClick={handleDeleteClick}
                  title={t("common.delete")}
                  disabled={deleteModule.isPending}
                >
                  <Trash2 className="h-3.5 w-3.5" />
                </Button>
              )}
              <Button variant="ghost" size="sm" className="h-8 w-8 p-0 shrink-0">
                <ChevronIcon className="h-4 w-4" />
              </Button>
            </div>
          </div>
        </CardHeader>

        {isExpanded && (
          <CardContent className="ps-4 pe-4 pb-3 pt-0 space-y-1">
            {sortedLessons.length > 0 ? (
              canReorderLessons ? (
                <SortableContext
                  items={sortedLessons.map((l) => lessonSortableId(l.lessonId))}
                  strategy={verticalListSortingStrategy}
                >
                  {sortedLessons.map((lesson, lessonIndex) => (
                    <SortableLessonItem
                      key={lesson.lessonId}
                      lesson={lesson}
                      index={lessonIndex}
                      courseId={courseId}
                      moduleId={module.id}
                    />
                  ))}
                </SortableContext>
              ) : (
              sortedLessons.map((lesson, lessonIndex) => (
                  <LessonCard
                    key={lesson.lessonId}
                    lesson={lesson}
                    index={lessonIndex}
                    courseId={courseId}
                  />
                ))
              )
            ) : (
              <p className="text-muted-foreground text-sm py-3 text-start">
                {t("course-management:detail.noLessons")}
              </p>
            )}
          </CardContent>
        )}
      </Card>

      <AddLessonDialog
        courseId={courseId}
        links={module.links}
        open={isAddLessonOpen}
        onOpenChange={setIsAddLessonOpen}
      />

      <ConfirmationModal
        open={isDeleteDialogOpen}
        onOpenChange={setIsDeleteDialogOpen}
        onConfirm={handleConfirmDelete}
        title={t("course-management:detail.moduleDeleteConfirmTitle")}
        message={t("course-management:detail.moduleDeleteConfirmMessage")}
        confirmText={t("common.delete")}
        cancelText={t("common.cancel")}
        isLoading={deleteModule.isPending}
      />
    </>
  );
}
