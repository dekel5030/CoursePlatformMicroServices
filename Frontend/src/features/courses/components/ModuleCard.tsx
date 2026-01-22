import { useState } from "react";
import { useTranslation } from "react-i18next";
import type { ModuleModel } from "../types";
import { Card, CardContent, CardHeader, Button } from "@/components";
import { ChevronDown, ChevronUp, Plus, Clock } from "lucide-react";
import { LessonCard } from "@/features/lessons";
import { hasLink } from "@/utils/linkHelpers";
import { AddLessonDialog } from "@/features/lessons/components/AddLessonDialog";

interface ModuleCardProps {
  module: ModuleModel;
  courseId: string;
  index: number;
}

export function ModuleCard({ module, courseId, index }: ModuleCardProps) {
  const { t, i18n } = useTranslation(["courses", "lessons", "translation"]);
  const [isExpanded, setIsExpanded] = useState(true);
  const [isAddLessonOpen, setIsAddLessonOpen] = useState(false);

  const isRTL = i18n.dir() === "rtl";
  const textAlignClass = isRTL ? "text-right" : "text-left";

  // Check available actions based on HATEOAS links
  const canCreateLesson = hasLink(module.links, "create-lesson");

  const formatDuration = (duration: string) => {
    if (!duration || duration === "00:00:00") return null;

    const parts = duration.split(":");
    if (parts.length >= 2) {
      const hours = parseInt(parts[0]);
      const minutes = parseInt(parts[1]);

      if (hours > 0) {
        return `${hours}${t("translation:time.hour")} ${minutes}${t("translation:time.minute")}`;
      }
      if (minutes > 0) {
        return `${minutes}${t("translation:time.minute")}`;
      }
    }
    return null;
  };

  const sortedLessons = [...module.lessons].sort((a, b) => a.order - b.order);
  const durationText = formatDuration(module.duration);

  return (
    <>
      <Card className="overflow-hidden">
        <CardHeader
          className="cursor-pointer hover:bg-accent/50 transition-colors p-4"
          onClick={() => setIsExpanded(!isExpanded)}
        >
          <div className="flex items-center justify-between gap-4">
            <div className={`flex items-center gap-3 flex-1 ${textAlignClass}`}>
              <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-md bg-primary/10 text-primary font-semibold text-sm">
                {index + 1}
              </div>
              <div className="flex-1 min-w-0">
                <h3
                  className={`font-semibold text-lg ${textAlignClass}`}
                  dir="auto"
                >
                  {module.title}
                </h3>
                <div className="flex items-center gap-4 text-sm text-muted-foreground mt-1">
                  <span>
                    {module.lessonCount} {t("courses:detail.lessons")}
                  </span>
                  {durationText && (
                    <>
                      <span>•</span>
                      <div className="flex items-center gap-1">
                        <Clock className="h-3 w-3" />
                        <span>{durationText}</span>
                      </div>
                    </>
                  )}
                </div>
              </div>
            </div>

            <div className="flex items-center gap-2">
              {canCreateLesson && (
                <Button
                  size="sm"
                  variant="ghost"
                  className="gap-2"
                  onClick={(e) => {
                    e.stopPropagation();
                    setIsAddLessonOpen(true);
                  }}
                >
                  <Plus className="h-4 w-4" />
                  {t("courses:detail.addLesson")}
                </Button>
              )}
              <Button variant="ghost" size="sm">
                {isExpanded ? (
                  <ChevronUp className="h-5 w-5" />
                ) : (
                  <ChevronDown className="h-5 w-5" />
                )}
              </Button>
            </div>
          </div>
        </CardHeader>

        {isExpanded && (
          <CardContent className="p-4 pt-0 space-y-2">
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
              <p
                className={`text-muted-foreground text-sm py-4 ${textAlignClass}`}
              >
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
