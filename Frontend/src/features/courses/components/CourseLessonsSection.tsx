import { useMemo } from "react";
import { useTranslation } from "react-i18next";
import { Card, CardHeader, CardTitle, CardContent } from "@/components";
import { ModuleCard } from "./ModuleCard";
import { motion } from "framer-motion";
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

  const sortedModules = useMemo(() => {
    if (!course.modules) return [];
    return [...course.modules].sort((a, b) => a.order - b.order);
  }, [course.modules]);

  const isRTL = i18n.dir() === "rtl";
  const textAlignClass = isRTL ? "text-right" : "text-left";

  return (
    <Card>
      <CardHeader>
        <CardTitle className={textAlignClass}>
          {t("courses:detail.courseContent")}
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-3">
        {sortedModules.length > 0 ? (
          sortedModules.map((module, index) => (
            <motion.div
              key={module.id || `index-${index}`}
              initial={{ opacity: 0, x: contentDir === "rtl" ? 10 : -10 }}
              animate={{ opacity: 1, x: 0 }}
              transition={{ delay: index * 0.05 }}
            >
              <ModuleCard module={module} courseId={course.id} index={index} />
            </motion.div>
          ))
        ) : (
          <p className={`text-muted-foreground ${textAlignClass}`}>
            {t("courses:detail.noModules")}
          </p>
        )}
      </CardContent>
    </Card>
  );
}
