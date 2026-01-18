import { useParams } from "react-router-dom";
import { useLesson, usePatchLesson } from "@/features/lessons";
import { useCourse } from "@/features/courses";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  Skeleton,
  BreadcrumbNav,
  InlineEditableText,
  InlineEditableTextarea,
} from "@/components";
import { Clock } from "lucide-react";
import { useTranslation } from "react-i18next";
import { motion } from "framer-motion";
import { toast } from "sonner";
import { getLink, hasLink, LessonRels } from "@/utils/linkHelpers";
import { LessonVideoUpload } from "../components/LessonVideoUpload";

export default function LessonPage() {
  const { courseId, lessonId } = useParams<{
    courseId: string;
    lessonId: string;
  }>();

  const { data: lesson, isLoading, error } = useLesson(courseId!, lessonId);
  const { data: course } = useCourse(courseId!);

  const patchLesson = usePatchLesson(courseId!, lessonId!);

  const { t } = useTranslation(["lessons", "translation"]);

  const handleTitleUpdate = async (newTitle: string) => {
    const updateLink = getLink(lesson?.links, LessonRels.PARTIAL_UPDATE);
    if (!updateLink) {
      console.error("No update link found for this lesson");
      return;
    }
    
    try {
      await patchLesson.mutateAsync({ url: updateLink.href, request: { title: newTitle } });
      toast.success(t("lessons:actions.titleUpdated"));
    } catch (error) {
      toast.error(t("lessons:actions.titleUpdateFailed"));
      throw error;
    }
  };

  const handleDescriptionUpdate = async (newDescription: string) => {
    const updateLink = getLink(lesson?.links, LessonRels.PARTIAL_UPDATE);
    if (!updateLink) {
      console.error("No update link found for this lesson");
      return;
    }
    
    try {
      await patchLesson.mutateAsync({ url: updateLink.href, request: { description: newDescription } });
      toast.success(t("lessons:actions.descriptionUpdated"));
    } catch (error) {
      toast.error(t("lessons:actions.descriptionUpdateFailed"));
      throw error;
    }
  };

  if (isLoading) {
    return (
      <div className="space-y-6">
        <div className="bg-background border-b border-border py-3 px-8">
          <div className="max-w-7xl mx-auto">
            <Skeleton className="h-4 w-64" />
          </div>
        </div>
        <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-6">
          <Skeleton className="w-full aspect-video rounded-xl shadow-lg" />
          <Card>
            <CardHeader className="space-y-3">
              <div className="flex items-start justify-between">
                <Skeleton className="h-10 w-1/2" />
                <Skeleton className="h-6 w-20 rounded-full" />
              </div>
            </CardHeader>
            <CardContent>
              <div className="space-y-2">
                <Skeleton className="h-6 w-32" />
                <Skeleton className="h-4 w-full" />
                <Skeleton className="h-4 w-full" />
                <Skeleton className="h-4 w-2/3" />
              </div>
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

  if (!lesson) {
    return (
      <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="bg-destructive/15 text-destructive px-4 py-3 rounded-md">
          Lesson not found
        </div>
      </div>
    );
  }

  const formatDuration = (duration: string | null | undefined) => {
    if (!duration) return null;
    const parts = duration.split(":");
    return `${parseInt(parts[0])}h ${parseInt(parts[1])}m`;
  };

  const breadcrumbItems = [
    { label: t("breadcrumbs.home"), path: "/" },
    { label: t("breadcrumbs.courses"), path: "/catalog" },
    {
      label: course?.title || t("breadcrumbs.course"),
      path: course ? `/courses/${course.id}` : undefined,
    },
    { label: lesson.title },
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
        className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-6"
        variants={container}
        initial="hidden"
        animate="show"
      >
        {lesson.videoUrl ? (
          <motion.div variants={item}>
            <Card className="overflow-hidden border-0 shadow-lg bg-black">
              <CardContent className="p-0">
                <video
                  className="w-full aspect-video"
                  controls
                  controlsList="nodownload"
                  poster={lesson.thumbnailImage || undefined}
                >
                  <source src={lesson.videoUrl} type="video/mp4" />
                </video>
              </CardContent>
            </Card>
          </motion.div>
        ) : (
          <motion.div variants={item}>
            <Card className="overflow-hidden border-0 shadow-lg">
              <CardContent className="p-12 text-center space-y-4">
                <p className="text-muted-foreground">
                  {t("lessons:pages.lesson.noVideo")}
                </p>
                <LessonVideoUpload
                  courseId={lesson.courseId}
                  lessonId={lesson.lessonId}
                  links={lesson.links}
                />
              </CardContent>
            </Card>
          </motion.div>
        )}

        <motion.div variants={item}>
          <Card>
            <CardHeader className="space-y-3">
              <div className="flex items-start justify-between gap-4 flex-wrap">
                {hasLink(lesson.links, LessonRels.PARTIAL_UPDATE) ? (
                  <InlineEditableText
                    value={lesson.title}
                    onSave={handleTitleUpdate}
                    displayClassName="text-3xl font-semibold"
                    inputClassName="text-3xl font-semibold"
                    placeholder={t("lessons:actions.enterTitle")}
                    maxLength={200}
                  />
                ) : (
                  <CardTitle className="text-3xl" dir="auto">
                    {lesson.title}
                  </CardTitle>
                )}
                <div className="flex items-center gap-2">
                  {lesson.duration && (
                    <div className="flex items-center gap-1 text-sm text-muted-foreground bg-secondary/50 px-3 py-1 rounded-full">
                      <Clock className="h-4 w-4" />
                      {formatDuration(lesson.duration)}
                    </div>
                  )}
                  <LessonVideoUpload
                    courseId={lesson.courseId}
                    lessonId={lesson.lessonId}
                    links={lesson.links}
                  />
                </div>
              </div>
            </CardHeader>

            <CardContent>
              <div className="space-y-2">
                <h2 className="text-lg font-semibold">
                  {t("lessons:pages.lesson.description")}
                </h2>
                {hasLink(lesson.links, LessonRels.PARTIAL_UPDATE) ? (
                  <InlineEditableTextarea
                    value={lesson.description || ""}
                    onSave={handleDescriptionUpdate}
                    displayClassName="text-muted-foreground leading-relaxed"
                    placeholder={t("lessons:actions.enterDescription")}
                    rows={5}
                    maxLength={2000}
                  />
                ) : lesson.description ? (
                  <p
                    className="text-muted-foreground leading-relaxed"
                    dir="auto"
                  >
                    {lesson.description}
                  </p>
                ) : (
                  <p className="text-muted-foreground italic">
                    {t("lessons:actions.noDescription")}
                  </p>
                )}
              </div>
            </CardContent>
          </Card>
        </motion.div>
      </motion.div>
    </div>
  );
}
