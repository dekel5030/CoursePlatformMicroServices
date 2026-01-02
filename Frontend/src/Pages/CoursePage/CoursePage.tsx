import { useParams } from "react-router-dom";
import { Lesson } from "@/features/lessons";
import { useCourse } from "@/features/courses";
import {
  Button,
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  Skeleton,
} from "@/components/ui";
import Breadcrumb from "@/components/layout/Breadcrumb/Breadcrumb";
import { ShoppingCart, CreditCard } from "lucide-react";
import { useTranslation } from "react-i18next";
import { usePermissions } from "@/hooks/usePermissions";
import { hasPermission } from "@/utils/permissionEvaluation";
import { Plus, Trash2 } from "lucide-react";
import { motion } from "framer-motion";

export default function CoursePage() {
  const { id } = useParams<{ id: string }>();
  const { data: course, isLoading, error } = useCourse(id);
  const { t, i18n } = useTranslation();
  const permissions = usePermissions();

  const canDeleteCourse = course
    ? hasPermission(permissions, "Delete", "Course", course.id)
    : false;
  const canAddLesson = hasPermission(permissions, "Create", "Lesson", "*");

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
          Course not found
        </div>
      </div>
    );
  }

  // Determine content direction based on the current language or explicit field if available
  // For now, we will use auto detection for text content
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
      <Breadcrumb items={breadcrumbItems} />
      <motion.div
        className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-8"
        variants={container}
        initial="hidden"
        animate="show"
      >
        <motion.div variants={item}>
          <Card className="overflow-hidden">
            <div className="grid md:grid-cols-2 gap-6 p-6">
              {course.imageUrl && (
                <div className="relative h-64 md:h-full overflow-hidden rounded-lg">
                  <motion.img
                    initial={{ scale: 1.1 }}
                    animate={{ scale: 1 }}
                    transition={{ duration: 0.5 }}
                    src={course.imageUrl}
                    alt={course.title}
                    className="h-full w-full object-cover"
                  />
                </div>
              )}
              <div className="space-y-4">
                <div className="space-y-2">
                  <div className="flex justify-between items-start">
                    <h1 className="text-3xl font-bold" dir="auto">
                      {course.title}
                    </h1>
                    {canDeleteCourse && (
                      <Button variant="destructive" size="sm" className="gap-2">
                        <Trash2 className="h-4 w-4" />
                        {t("pages.course.deleteCourse")}
                      </Button>
                    )}
                  </div>
                  <p className="text-muted-foreground">
                    {t("pages.course.instructor")}:{" "}
                    <span dir="auto">
                      {course.instructorUserId ?? "Unknown"}
                    </span>
                  </p>
                </div>
                <div className="flex gap-3">
                  <motion.div
                    className="flex-1"
                    whileHover={{ scale: 1.02 }}
                    whileTap={{ scale: 0.98 }}
                  >
                    <Button className="w-full gap-2">
                      <CreditCard className="h-4 w-4" />
                      {t("pages.course.buyNow")}
                    </Button>
                  </motion.div>
                  <motion.div
                    className="flex-1"
                    whileHover={{ scale: 1.02 }}
                    whileTap={{ scale: 0.98 }}
                  >
                    <Button variant="outline" className="w-full gap-2">
                      <ShoppingCart className="h-4 w-4" />
                      {t("pages.course.addToCart")}
                    </Button>
                  </motion.div>
                </div>
              </div>
            </div>
          </Card>
        </motion.div>

        {course.description && (
          <motion.div variants={item}>
            <Card>
              <CardHeader>
                <CardTitle>{t("pages.course.about")}</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-muted-foreground" dir="auto">
                  {course.description}
                </p>
              </CardContent>
            </Card>
          </motion.div>
        )}

        <motion.div variants={item}>
          <Card>
            <CardHeader className="flex flex-row items-center justify-between">
              <CardTitle>{t("pages.course.lessons")}</CardTitle>
              {canAddLesson && (
                <Button size="sm" className="gap-2">
                  <Plus className="h-4 w-4" />
                  {t("pages.course.addLesson")}
                </Button>
              )}
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
                  {t("pages.course.noLessons")}
                </p>
              )}
            </CardContent>
          </Card>
        </motion.div>
      </motion.div>
    </div>
  );
}
