import { useParams } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useCourse, usePatchCourse } from "@/features/courses";
import { Card, CardHeader, CardTitle, CardContent, Skeleton, BreadcrumbNav, InlineEditableTextarea } from "@/components";
import { motion } from "framer-motion";
import { CourseHeader } from "../components/CourseHeader";
import { CourseLessonsSection } from "../components/CourseLessonsSection";
import { toast } from "sonner";
import { Authorized, ActionType, ResourceType, ResourceId } from "@/features/auth";

export default function CoursePage() {
  const { id } = useParams<{ id: string }>();
  const { data: course, isLoading, error } = useCourse(id);
  const patchCourse = usePatchCourse(id!);
  const { t, i18n } = useTranslation(['courses', 'translation']);

  const handleDescriptionUpdate = async (newDescription: string) => {
    try {
      await patchCourse.mutateAsync({ description: newDescription });
      toast.success(t('courses:detail.descriptionUpdated'));
    } catch (error) {
      toast.error(t('courses:detail.descriptionUpdateFailed'));
      throw error;
    }
  };

  if (isLoading) {
    return (
      <div className="space-y-6">
        <div className="bg-background border-b border-border py-3 px-8">
          <div className="max-w-7xl mx-auto">
            <Skeleton className="h-4 w-48" />
          </div>
        </div>
        <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-8">
          <Card className="overflow-hidden">
            <div className="grid md:grid-cols-2 gap-6 p-6">
              <Skeleton className="relative h-64 md:h-full rounded-lg" />
              <div className="space-y-4">
                <div className="space-y-2">
                  <Skeleton className="h-10 w-3/4" />
                  <Skeleton className="h-4 w-1/2" />
                </div>
                <div className="flex gap-3">
                  <Skeleton className="h-10 flex-1" />
                  <Skeleton className="h-10 flex-1" />
                </div>
              </div>
            </div>
          </Card>
          <Card>
            <CardHeader>
              <Skeleton className="h-6 w-32" />
            </CardHeader>
            <CardContent>
              <Skeleton className="h-4 w-full mb-2" />
              <Skeleton className="h-4 w-full mb-2" />
              <Skeleton className="h-4 w-2/3" />
            </CardContent>
          </Card>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="bg-destructive/15 text-destructive px-4 py-3 rounded-md">
          {t("common.error", { message: error.message })}
        </div>
      </div>
    );
  }

  if (!course) {
    return (
      <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="bg-destructive/15 text-destructive px-4 py-3 rounded-md">
          {t('courses:detail.notFound')}
        </div>
      </div>
    );
  }

  const contentDir = i18n.dir();

  const breadcrumbItems = [
    { label: t("breadcrumbs.home"), path: "/" },
    { label: t("breadcrumbs.courses"), path: "/catalog" },
    { label: course.title },
  ];

  const container = {
    hidden: { opacity: 0 },
    show: {
      opacity: 1,
      transition: {
        staggerChildren: 0.1,
      },
    },
  };

  const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0 },
  };

  return (
    <div className="space-y-6">
      <BreadcrumbNav items={breadcrumbItems} />
      <motion.div
        className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-8"
        variants={container}
        initial="hidden"
        animate="show"
      >
        <motion.div variants={item}>
          <CourseHeader course={course} />
        </motion.div>

        {/* Always show description section to allow adding description if empty */}
        {id && (
          <motion.div variants={item}>
            <Card>
              <CardHeader>
                <CardTitle>{t('courses:detail.about')}</CardTitle>
              </CardHeader>
              <CardContent>
                <Authorized
                  action={ActionType.Update}
                  resource={ResourceType.Course}
                  resourceId={ResourceId.create(course.id)}
                  fallback={
                    course.description ? (
                      <p className="text-muted-foreground" dir="auto">
                        {course.description}
                      </p>
                    ) : (
                      <p className="text-muted-foreground italic">
                        {t('courses:detail.noDescription')}
                      </p>
                    )
                  }
                >
                  <InlineEditableTextarea
                    value={course.description || ""}
                    onSave={handleDescriptionUpdate}
                    displayClassName="text-muted-foreground"
                    placeholder={t('courses:detail.enterDescription')}
                    rows={5}
                    maxLength={2000}
                  />
                </Authorized>
              </CardContent>
            </Card>
          </motion.div>
        )}

        <motion.div variants={item}>
          <CourseLessonsSection course={course} contentDir={contentDir} />
        </motion.div>
      </motion.div>
    </div>
  );
}
