import { useTranslation } from "react-i18next";
import {
  Card,
  CardHeader,
  CardTitle,
  CardContent,
  Skeleton,
} from "@/shared/ui";
import { BreadcrumbNav } from "@/components/layout";
import { InlineEditableTextarea, RichTextViewer } from "@/shared/common";
import { motion } from "framer-motion";
import { CourseHeader } from "./CourseHeader";
import { CourseLessonsSection } from "./CourseLessonsSection";
import { CourseRatingsSection } from "./CourseRatingsSection";
import type { CourseModel } from "@/domain/courses";

export interface BreadcrumbItem {
  label: string;
  path?: string;
}

interface CourseDetailContentProps {
  course: CourseModel | undefined;
  isLoading: boolean;
  error: Error | null;
  onDescriptionUpdate: (newDescription: string) => Promise<void>;
  onPriceUpdate?: (amount: number, currency: string) => Promise<void>;
  breadcrumbItems: BreadcrumbItem[];
}

function LoadingSkeleton() {
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

function ErrorState({ message }: { message: string }) {
  return (
    <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="bg-destructive/15 text-destructive px-4 py-3 rounded-md">
        {message}
      </div>
    </div>
  );
}

export function CourseDetailContent({
  course,
  isLoading,
  error,
  onDescriptionUpdate,
  onPriceUpdate,
  breadcrumbItems,
}: CourseDetailContentProps) {
  const { t, i18n } = useTranslation(["course-management", "translation"]);
  const dir = i18n.dir();

  if (isLoading) {
    return <LoadingSkeleton />;
  }

  if (error) {
    return (
      <div className="space-y-6">
        <BreadcrumbNav items={breadcrumbItems} />
        <ErrorState message={t("common.error", { message: error.message })} />
      </div>
    );
  }

  if (!course) {
    return (
      <div className="space-y-6">
        <BreadcrumbNav items={breadcrumbItems} />
        <ErrorState message={t("course-management:detail.notFound")} />
      </div>
    );
  }

  const canUpdate = !!course.links?.partialUpdate?.href;

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
        className="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8 py-6 space-y-6"
        variants={container}
        initial="hidden"
        animate="show"
      >
        <motion.div variants={item}>
          <CourseHeader course={course} onPriceUpdate={onPriceUpdate} />
        </motion.div>

        {/* Course Description */}
        {(course.description || canUpdate) && course.id && (
          <motion.div variants={item}>
            <Card>
              <CardHeader className="pb-3">
                <CardTitle className="text-start">
                  {t("course-management:detail.about")}
                </CardTitle>
              </CardHeader>
              <CardContent className="text-start" dir={dir}>
                {canUpdate ? (
                  <InlineEditableTextarea
                    value={course.description || ""}
                    onSave={onDescriptionUpdate}
                    displayClassName="text-muted-foreground text-start"
                    placeholder={t("course-management:detail.enterDescription")}
                    rows={4}
                    maxLength={2000}
                    renderAsMarkdown={true}
                  />
                ) : course.description ? (
                  <RichTextViewer content={course.description} />
                ) : null}
              </CardContent>
            </Card>
          </motion.div>
        )}

        {/* Course Content - Modules & Lessons */}
        <motion.div variants={item}>
          <CourseLessonsSection course={course} />
        </motion.div>

        {/* Reviews & Ratings */}
        <motion.div variants={item}>
          <CourseRatingsSection course={course} />
        </motion.div>
      </motion.div>
    </div>
  );
}
