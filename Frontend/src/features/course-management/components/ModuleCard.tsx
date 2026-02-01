import { useState } from "react";
import { useTranslation } from "react-i18next";
import type { ModuleModel } from "@/domain/courses";
import { Card, CardContent, CardHeader, Button } from "@/shared/ui";
import { ChevronDown, ChevronUp, Plus, Clock } from "lucide-react";
import { LessonCard } from "@/features/lesson-viewer";
import { hasLink, formatDuration } from "@/shared/utils";
import { AddLessonDialog } from "@/features/lesson-viewer/components/AddLessonDialog";
import { ModuleRels } from "@/domain/courses";

interface ModuleCardProps {
  module: ModuleModel;
  courseId: string;
  index: number;
}

export function ModuleCard({ module, courseId, index }: ModuleCardProps) {
  const { t } = useTranslation(["courses", "lessons", "translation"]);
  const [isExpanded, setIsExpanded] = useState(true);
  const [isAddLessonOpen, setIsAddLessonOpen] = useState(false);

  const canCreateLesson = hasLink(module.links, ModuleRels.CREATE_LESSON);

  const sortedLessons = [...module.lessons].sort((a, b) => a.order - b.order);
  const durationText = formatDuration(module.duration);

  const ChevronIcon = isExpanded ? ChevronUp : ChevronDown;

  return (
    <>
      <Card className="overflow-hidden border-s-4 border-s-primary/20 rounded-lg">
        <CardHeader
          className="cursor-pointer hover:bg-muted/50 transition-colors py-3 px-4"
          onClick={() => setIsExpanded(!isExpanded)}
        >
          <div className="flex items-center justify-between gap-4">
            <div className="flex items-center gap-3 flex-1 min-w-0 text-start">
              <div className="flex h-7 w-7 shrink-0 items-center justify-center rounded-full bg-primary/10 text-primary font-semibold text-sm">
                {index + 1}
              </div>
              <div className="flex-1 min-w-0">
                <h3 className="font-semibold text-start truncate" dir="auto">
                  {module.title}
                </h3>
                <div className="flex items-center gap-2 text-xs text-muted-foreground mt-0.5 flex-wrap">
                  <span>
                    {module.lessonCount} {t("courses:detail.lessons")}
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
                  <span className="text-xs">{t("courses:detail.addLesson")}</span>
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
              sortedLessons.map((lesson, lessonIndex) => (
                <LessonCard
                  key={lesson.lessonId}
                  lesson={lesson}
                  index={lessonIndex}
                  courseId={courseId}
                />
              ))
            ) : (
              <p className="text-muted-foreground text-sm py-3 text-start">
                {t("courses:detail.noLessons")}
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
    </>
  );
}
