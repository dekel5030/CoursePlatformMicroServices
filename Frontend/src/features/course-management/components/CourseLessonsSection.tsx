import { useMemo, useCallback, useState } from "react";
import { useTranslation } from "react-i18next";
import {
  DndContext,
  DragOverlay,
  type DragEndEvent,
  type DragOverEvent,
  type DragStartEvent,
  pointerWithin,
  PointerSensor,
  useSensor,
  useSensors,
} from "@dnd-kit/core";
import { restrictToVerticalAxis } from "@dnd-kit/modifiers";
import { SortableContext, verticalListSortingStrategy, arrayMove } from "@dnd-kit/sortable";
import { Card, CardHeader, CardTitle, CardContent, Button } from "@/shared/ui";
import { ModuleCard } from "./ModuleCard";
import { SortableModuleItem, moduleSortableId, getModuleId } from "./SortableModuleItem";
import { getLessonId } from "./SortableLessonItem";
import { motion } from "framer-motion";
import { Plus, FolderPlus } from "lucide-react";
import { hasLink, getLink } from "@/shared/utils";
import { CourseRels } from "@/domain/courses";
import { useCreateModule, useReorderModules, useReorderLessons } from "@/domain/courses";
import { useMoveLesson } from "@/domain/lessons";
import { LessonCard } from "@/features/lesson-viewer";
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

  const [activeDragId, setActiveDragId] = useState<string | null>(null);
  const [targetedModuleId, setTargetedModuleId] = useState<string | null>(null);

  const sortedModules = useMemo(() => {
    if (!course.modules) return [];
    return [...course.modules].sort((a, b) => a.order - b.order);
  }, [course.modules]);

  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: {
        distance: 10,
      },
    })
  );

  const canCreateModule = hasLink(course.links, CourseRels.CREATE_MODULE);
  const canReorderModules = hasLink(course.links, CourseRels.REORDER_MODULES);
  const createModuleLink = getLink(course.links, CourseRels.CREATE_MODULE);

  const handleDragStart = useCallback((event: DragStartEvent) => {
    setActiveDragId(String(event.active.id));
    setTargetedModuleId(null);
  }, []);

  const handleDragOver = useCallback((event: DragOverEvent) => {
    const { active, over } = event;
    if (!over) {
      setTargetedModuleId(null);
      return;
    }
    const activeId = String(active.id);
    const overId = String(over.id);
    const overData = over.data.current as
      | { type: string; lessonId?: string; moduleId?: string }
      | undefined;
    if (activeId.startsWith(MODULE_PREFIX)) {
      setTargetedModuleId(
        overId.startsWith(MODULE_PREFIX) ? getModuleId(overId) : null
      );
    } else if (activeId.startsWith(LESSON_PREFIX)) {
      if (overId.startsWith(MODULE_PREFIX)) {
        setTargetedModuleId(getModuleId(overId));
      } else if (overId.startsWith(LESSON_PREFIX) && overData?.moduleId) {
        setTargetedModuleId(overData.moduleId);
      } else {
        setTargetedModuleId(null);
      }
    } else {
      setTargetedModuleId(null);
    }
  }, []);

  const handleDragEnd = useCallback(
    async (event: DragEndEvent) => {
      setActiveDragId(null);
      setTargetedModuleId(null);
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
        } else if (overId.startsWith(MODULE_PREFIX)) {
          targetModuleId = getModuleId(overId);
          const targetModule = sortedModules.find((m) => m.id === targetModuleId);
          if (!targetModule) return;
          targetIndex = 0;
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
            collisionDetection={pointerWithin}
            modifiers={[restrictToVerticalAxis]}
            onDragStart={handleDragStart}
            onDragOver={handleDragOver}
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
                      isDropTarget={targetedModuleId === module.id}
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
                  <ModuleCard
                    module={module}
                    courseId={course.id}
                    index={index}
                    isDropTarget={targetedModuleId === module.id}
                  />
                </motion.div>
              ))
            )}
            <DragOverlay dropAnimation={null}>
              {activeDragId ? (
                activeDragId.startsWith(LESSON_PREFIX) ? (
                  (() => {
                    const lessonId = getLessonId(activeDragId);
                    const mod = sortedModules.find((m) =>
                      m.lessons.some((l) => l.lessonId === lessonId)
                    );
                    const lesson = mod?.lessons.find((l) => l.lessonId === lessonId);
                    const lessonIndex = mod
                      ? [...mod.lessons].sort((a, b) => a.order - b.order).findIndex((l) => l.lessonId === lessonId)
                      : 0;
                    return lesson && mod ? (
                      <div className="shadow-lg rounded-md opacity-95 pointer-events-none w-full max-w-[var(--radix-popper-available-width)]">
                        <LessonCard
                          lesson={lesson}
                          index={lessonIndex >= 0 ? lessonIndex : 0}
                          courseId={course.id}
                        />
                      </div>
                    ) : null;
                  })()
                ) : activeDragId.startsWith(MODULE_PREFIX) ? (
                  (() => {
                    const moduleId = getModuleId(activeDragId);
                    const mod = sortedModules.find((m) => m.id === moduleId);
                    const moduleIndex = mod ? sortedModules.findIndex((m) => m.id === moduleId) : 0;
                    return mod ? (
                      <div className="shadow-lg rounded-lg opacity-95 pointer-events-none w-full max-w-[var(--radix-popper-available-width)]">
                        <ModuleCard
                          module={mod}
                          courseId={course.id}
                          index={moduleIndex}
                        />
                      </div>
                    ) : null;
                  })()
                ) : null
              ) : null}
            </DragOverlay>
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
