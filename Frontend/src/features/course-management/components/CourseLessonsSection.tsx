import { useMemo, useCallback } from "react";
import { useTranslation } from "react-i18next";
import {
  DndContext,
  type DragEndEvent,
  closestCenter,
  PointerSensor,
  useSensor,
  useSensors,
} from "@dnd-kit/core";
import { SortableContext, verticalListSortingStrategy, arrayMove } from "@dnd-kit/sortable";
import { Card, CardHeader, CardTitle, CardContent, Button } from "@/shared/ui";
import { ModuleCard } from "./ModuleCard";
import { SortableModuleItem, moduleSortableId } from "./SortableModuleItem";
import { getLessonId } from "./SortableLessonItem";
import { motion } from "framer-motion";
import { Plus, FolderPlus } from "lucide-react";
import { hasLink, getLink } from "@/shared/utils";
import { CourseRels } from "@/domain/courses";
import { useCreateModule, useReorderModules, useReorderLessons } from "@/domain/courses";
import { useMoveLesson } from "@/domain/lessons";
import type { CourseModel } from "@/domain/courses";

interface CourseLessonsSectionProps {
  course: CourseModel;
}

const MODULE_PREFIX = "module-";
const LESSON_PREFIX = "lesson-";

export function CourseLessonsSection({ course }: CourseLessonsSectionProps) {
  const { t } = useTranslation(["course-management", "translation"]);
  const createModule = useCreateModule(course.id);
  const reorderModules = useReorderModules(course.id);
  const reorderLessons = useReorderLessons(course.id);
  const moveLesson = useMoveLesson(course.id);

  const sortedModules = useMemo(() => {
    if (!course.modules) return [];
    return [...course.modules].sort((a, b) => a.order - b.order);
  }, [course.modules]);

  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: {
        distance: 8,
      },
    })
  );

  const canCreateModule = hasLink(course.links, CourseRels.CREATE_MODULE);
  const canReorderModules = hasLink(course.links, CourseRels.REORDER_MODULES);
  const createModuleLink = getLink(course.links, CourseRels.CREATE_MODULE);

  const handleDragEnd = useCallback(
    async (event: DragEndEvent) => {
      const { active, over } = event;
      if (!over) return;

      const activeId = String(active.id);
      const overId = String(over.id);

      if (activeId.startsWith(MODULE_PREFIX)) {
        if (!canReorderModules || !overId.startsWith(MODULE_PREFIX)) return;
        const oldIndex = sortedModules.findIndex(
          (m) => moduleSortableId(m.id) === activeId
        );
        const newIndex = sortedModules.findIndex(
          (m) => moduleSortableId(m.id) === overId
        );
        if (oldIndex === -1 || newIndex === -1 || oldIndex === newIndex) return;
        const newOrder = arrayMove(sortedModules, oldIndex, newIndex);
        const moduleIds = newOrder.map((m) => m.id);
        await reorderModules.mutateAsync({ moduleIds });
        return;
      }

      if (activeId.startsWith(LESSON_PREFIX)) {
        const overData = over.data.current as
          | { type: string; lessonId: string; moduleId: string }
          | undefined;

        const sourceLessonId = getLessonId(activeId);
        const sourceModule = sortedModules.find((m) =>
          m.lessons.some((l) => l.lessonId === sourceLessonId)
        );
        if (!sourceModule) return;

        const sourceLessons = [...sourceModule.lessons].sort((a, b) => a.order - b.order);
        const sourceIndex = sourceLessons.findIndex((l) => l.lessonId === sourceLessonId);
        if (sourceIndex === -1) return;

        let targetModuleId: string;
        let targetIndex: number;

        if (overId.startsWith(LESSON_PREFIX) && overData?.moduleId) {
          targetModuleId = overData.moduleId;
          const targetModule = sortedModules.find((m) => m.id === targetModuleId);
          if (!targetModule) return;
          const targetLessons = [...targetModule.lessons].sort((a, b) => a.order - b.order);
          const overLessonId = getLessonId(overId);
          targetIndex = targetLessons.findIndex((l) => l.lessonId === overLessonId);
          if (targetIndex === -1) return;
        } else {
          return;
        }

        if (sourceModule.id === targetModuleId) {
          if (sourceIndex === targetIndex) return;
          const newOrder = arrayMove(
            sourceLessons.map((l) => l.lessonId),
            sourceIndex,
            targetIndex
          );
          await reorderLessons.mutateAsync({
            moduleId: sourceModule.id,
            lessonIds: newOrder,
          });
        } else {
          await moveLesson.mutateAsync({
            lessonId: sourceLessonId,
            request: { targetModuleId, targetIndex },
          });
        }
      }
    },
    [
      course.id,
      canReorderModules,
      sortedModules,
      reorderModules,
      reorderLessons,
      moveLesson,
    ]
  );

  const handleCreateModule = async () => {
    if (!createModuleLink) return;
    const moduleNumber = sortedModules.length + 1;
    const title = `${t("course-management:detail.module")} ${moduleNumber}`;
    await createModule.mutateAsync({
      url: createModuleLink.href,
      request: { title },
    });
  };

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between gap-4 py-4">
        <CardTitle className="text-start text-lg">
          {t("course-management:detail.courseContent")}
        </CardTitle>
        {canCreateModule && (
          <Button
            size="sm"
            variant="outline"
            className="gap-2 shrink-0"
            onClick={handleCreateModule}
            disabled={createModule.isPending}
          >
            <FolderPlus className="h-4 w-4" />
            {t("course-management:detail.addModule")}
          </Button>
        )}
      </CardHeader>
      <CardContent className="space-y-2 pb-6 pt-0">
        {sortedModules.length > 0 ? (
          <DndContext
            sensors={sensors}
            collisionDetection={closestCenter}
            onDragEnd={handleDragEnd}
          >
            {canReorderModules ? (
              <SortableContext
                items={sortedModules.map((m) => moduleSortableId(m.id))}
                strategy={verticalListSortingStrategy}
              >
                {sortedModules.map((module, index) => (
                  <motion.div
                    key={module.id || `index-${index}`}
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    transition={{ delay: index * 0.04 }}
                  >
                    <SortableModuleItem
                      module={module}
                      courseId={course.id}
                      index={index}
                    />
                  </motion.div>
                ))}
              </SortableContext>
            ) : (
              sortedModules.map((module, index) => (
                <motion.div
                  key={module.id || `index-${index}`}
                  initial={{ opacity: 0 }}
                  animate={{ opacity: 1 }}
                  transition={{ delay: index * 0.04 }}
                >
                  <ModuleCard module={module} courseId={course.id} index={index} />
                </motion.div>
              ))
            )}
          </DndContext>
        ) : (
          <div className="py-10 text-center space-y-4">
            <p className="text-muted-foreground text-start">
              {t("course-management:detail.noModules")}
            </p>
            {canCreateModule && (
              <Button
                variant="default"
                className="gap-2"
                onClick={handleCreateModule}
                disabled={createModule.isPending}
              >
                <Plus className="h-4 w-4" />
                {t("course-management:detail.createFirstModule")}
              </Button>
            )}
          </div>
        )}
      </CardContent>
    </Card>
  );
}
