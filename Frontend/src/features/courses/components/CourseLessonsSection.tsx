import { useState, useMemo } from "react";
import { useTranslation } from "react-i18next";
import { Button, Card, CardHeader, CardTitle, CardContent } from "@/components";
import { Plus } from "lucide-react";
import { LessonCard } from "@/features/lessons";
import { AddLessonDialog } from "@/features/lessons/components/AddLessonDialog";
import { motion } from "framer-motion";
import { hasLink, CourseRels } from "@/utils/linkHelpers";
import type { CourseModel } from "../types";

interface CourseLessonsSectionProps {
  course: CourseModel;
  contentDir: string;
}

export function CourseLessonsSection({
  course,
  contentDir,
}: CourseLessonsSectionProps) {
  const { t, i18n } = useTranslation(["courses", "translation"]);
  const [isAddLessonOpen, setIsAddLessonOpen] = useState(false);

  const sortedLessons = useMemo(() => {
    if (!course.lessons) return [];
    return [...course.lessons].sort((a, b) => a.order - b.order);
  }, [course.lessons]);

  const isRTL = i18n.dir() === "rtl";
  const textAlignClass = isRTL ? "text-right" : "text-left";
  
  // Check if user can create lessons based on HATEOAS links
  const canCreateLesson = hasLink(course.links, CourseRels.CREATE_LESSON);

  return (
    <>
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle className={textAlignClass}>
            {t("courses:detail.lessons")}
          </CardTitle>
          {canCreateLesson && (
            <Button
              size="sm"
              className="gap-2"
              onClick={() => setIsAddLessonOpen(true)}
            >
              <Plus className="h-4 w-4" />
              {t("courses:detail.addLesson")}
            </Button>
          )}
        </CardHeader>
        <CardContent className="space-y-2">
          {sortedLessons.length > 0 ? (
            sortedLessons.map((lesson, index) => (
              <motion.div
                key={lesson.lessonId || `index-${index}`}
                initial={{ opacity: 0, x: contentDir === "rtl" ? 10 : -10 }}
                animate={{ opacity: 1, x: 0 }}
                transition={{ delay: index * 0.05 }}
              >
                <LessonCard
                  lesson={lesson}
                  index={index}
                  courseId={course.id}
                />
              </motion.div>
            ))
          ) : (
            <p className="text-muted-foreground">{t("courses:detail.noLessons")}</p>
          )}
        </CardContent>
      </Card>

      <AddLessonDialog
        courseId={course.id}
        links={course.links}
        open={isAddLessonOpen}
        onOpenChange={setIsAddLessonOpen}
      />
    </>
  );
}
