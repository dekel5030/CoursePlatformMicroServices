import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Button, Card, CardHeader, CardTitle, CardContent } from "@/components";
import { Plus } from "lucide-react";
import { Authorized, ActionType, ResourceType, ResourceId } from "@/features/auth";
import { Lesson } from "@/features/lessons";
import { AddLessonDialog } from "@/features/lessons/components/AddLessonDialog";
import { motion } from "framer-motion";
import type { Course } from "../types";

interface CourseLessonsSectionProps {
  course: Course;
  contentDir: string;
}

export function CourseLessonsSection({ course, contentDir }: CourseLessonsSectionProps) {
  const { t } = useTranslation(['courses', 'translation']);
  const [isAddLessonOpen, setIsAddLessonOpen] = useState(false);

  return (
    <>
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle>{t('courses:detail.lessons')}</CardTitle>
          <Authorized 
            action={ActionType.Create} 
            resource={ResourceType.Lesson}
            resourceId={ResourceId.Wildcard}
          >
            <Button 
              size="sm" 
              className="gap-2"
              onClick={() => setIsAddLessonOpen(true)}
            >
              <Plus className="h-4 w-4" />
              {t('courses:detail.addLesson')}
            </Button>
          </Authorized>
        </CardHeader>
        <CardContent className="space-y-2">
          {course.lessons && course.lessons.length > 0 ? (
            course.lessons
              .sort((a, b) => a.order - b.order)
              .map((lesson, index) => (
                <motion.div
                  key={lesson.id}
                  initial={{
                    opacity: 0,
                    x: contentDir === "rtl" ? 10 : -10,
                  }}
                  animate={{ opacity: 1, x: 0 }}
                  transition={{ delay: index * 0.05 }}
                >
                  <Lesson lesson={lesson} index={index} />
                </motion.div>
              ))
          ) : (
            <p className="text-muted-foreground text-center py-8">
              {t('courses:detail.noLessons')}
            </p>
          )}
        </CardContent>
      </Card>

      <AddLessonDialog 
        courseId={course.id}
        open={isAddLessonOpen} 
        onOpenChange={setIsAddLessonOpen} 
      />
    </>
  );
}
