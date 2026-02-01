import { useMemo } from "react";
import { useTranslation } from "react-i18next";
import { Card, CardHeader, CardTitle, CardContent, Button } from "@/shared/ui";
import { ModuleCard } from "./ModuleCard";
import { motion } from "framer-motion";
import { Plus, FolderPlus } from "lucide-react";
import { hasLink, getLink } from "@/shared/utils";
import { CourseRels } from "@/domain/courses";
import { useCreateModule } from "@/domain/courses";
import type { CourseModel } from "@/domain/courses";

interface CourseLessonsSectionProps {
  course: CourseModel;
}

export function CourseLessonsSection({ course }: CourseLessonsSectionProps) {
  const { t } = useTranslation(["courses", "translation"]);
  const createModule = useCreateModule(course.id);

  const sortedModules = useMemo(() => {
    if (!course.modules) return [];
    return [...course.modules].sort((a, b) => a.order - b.order);
  }, [course.modules]);

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
      <CardHeader className="flex flex-row items-center justify-between gap-4 py-4">
        <CardTitle className="text-start text-lg">
          {t("courses:detail.courseContent")}
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
            {t("courses:detail.addModule")}
          </Button>
        )}
      </CardHeader>
      <CardContent className="space-y-2 pb-6 pt-0">
        {sortedModules.length > 0 ? (
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
        ) : (
          <div className="py-10 text-center space-y-4">
            <p className="text-muted-foreground text-start">
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
