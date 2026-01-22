import { useMemo } from "react";
import { useTranslation } from "react-i18next";
import { Card, CardHeader, CardTitle, CardContent, Button } from "@/components";
import { ModuleCard } from "./ModuleCard";
import { motion } from "framer-motion";
import { Plus, FolderPlus } from "lucide-react";
import { hasLink, getLink, CourseRels } from "@/utils/linkHelpers";
import { useCreateModule } from "../hooks/use-courses";
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
  const createModule = useCreateModule(course.id);

  const sortedModules = useMemo(() => {
    if (!course.modules) return [];
    return [...course.modules].sort((a, b) => a.order - b.order);
  }, [course.modules]);

  const isRTL = i18n.dir() === "rtl";
  const textAlignClass = isRTL ? "text-right" : "text-left";

  // Check permissions based on HATEOAS links
  const canCreateModule = hasLink(course.links, CourseRels.CREATE_MODULE);
  const createModuleLink = getLink(course.links, CourseRels.CREATE_MODULE);

  const handleCreateModule = async () => {
    if (!createModuleLink) return;

    const moduleNumber = sortedModules.length + 1;
    const title = `${t("courses:detail.module")} ${moduleNumber}`;

    await createModule.mutateAsync({
      url: createModuleLink.href,
      request: { title },
    });
  };

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <CardTitle className={textAlignClass}>
          {t("courses:detail.courseContent")}
        </CardTitle>
        {canCreateModule && (
          <Button
            size="sm"
            variant="outline"
            className="gap-2"
            onClick={handleCreateModule}
            disabled={createModule.isPending}
          >
            <FolderPlus className="h-4 w-4" />
            {t("courses:detail.addModule")}
          </Button>
        )}
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
          <div className="text-center py-8 space-y-4">
            <p className={`text-muted-foreground ${textAlignClass}`}>
              {t("courses:detail.noModules")}
            </p>
            {canCreateModule && (
              <Button
                variant="default"
                className="gap-2"
                onClick={handleCreateModule}
                disabled={createModule.isPending}
              >
                <Plus className="h-4 w-4" />
                {t("courses:detail.createFirstModule")}
              </Button>
            )}
          </div>
        )}
      </CardContent>
    </Card>
  );
}
