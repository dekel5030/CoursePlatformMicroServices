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
      <Card className="overflow-hidden border-l-4 border-l-primary/20">
        <CardHeader
          className="cursor-pointer hover:bg-accent/30 transition-colors py-3 px-4"
          onClick={() => setIsExpanded(!isExpanded)}
        >
          <div className="flex items-center justify-between gap-4">
            <div className={`flex items-center gap-3 flex-1 ${textAlignClass}`}>
              <div className="flex h-7 w-7 shrink-0 items-center justify-center rounded-full bg-primary/10 text-primary font-semibold text-sm">
                {index + 1}
              </div>
              <div className="flex-1 min-w-0">
                <h3
                  className={`font-semibold ${textAlignClass}`}
                  dir="auto"
                >
                  {module.title}
                </h3>
                <div className={`flex items-center gap-3 text-xs text-muted-foreground mt-0.5 ${isRTL ? "flex-row-reverse" : ""}`}>
                  <span>
                    {module.lessonCount} {t("courses:detail.lessons")}
                  </span>
                  {durationText && (
                    <>
                      <span>•</span>
                      <div className={`flex items-center gap-1 ${isRTL ? "flex-row-reverse" : ""}`}>
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
              <Button variant="ghost" size="sm" className="h-8 w-8 p-0">
                {isExpanded ? (
                  <ChevronUp className="h-4 w-4" />
                ) : (
                  <ChevronDown className="h-4 w-4" />
                )}
              </Button>
            </div>
          </div>
        </CardHeader>

        {isExpanded && (
          <CardContent className="px-4 pb-3 pt-0 space-y-1">
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
                className={`text-muted-foreground text-sm py-3 ${textAlignClass}`}
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
