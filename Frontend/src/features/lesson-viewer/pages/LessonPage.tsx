import { useState, useCallback } from "react";
import { useParams, useLocation, useNavigate } from "react-router-dom";
import {
  useLesson,
  usePatchLesson,
  useGenerateLessonAi,
  useDeleteLesson,
} from "@/domain/lessons";
import { useCourse } from "@/domain/courses";
import {
  useEnrollmentIdByCourseId,
  updateLessonProgress,
  markLessonCompleted,
} from "@/domain/enrollments";
import { useQueryClient } from "@tanstack/react-query";
import { enrollmentsQueryKeys } from "@/domain/enrollments/query-keys";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  Skeleton,
  Button,
  Switch,
} from "@/shared/ui";
import { BreadcrumbNav } from "@/components/layout";
import { LinkButtons } from "@/shared/components";
import { InlineEditableText, InlineEditableTextarea, RichTextViewer } from "@/shared/common";
import { Clock, Sparkles } from "lucide-react";
import { useTranslation } from "react-i18next";
import { motion } from "framer-motion";
import { toast } from "sonner";
import { getLinkFromRecord, formatDuration, apiHrefToAppRoute, LINK_LABELS } from "@/shared/utils";
import { LessonVideoUpload } from "../components/LessonVideoUpload";
import { AiSuggestionField } from "../components/AiSuggestionField";
import { HlsVideoPlayer } from "@/components/HlsVideoPlayer";
import ConfirmationModal from "@/components/ui/ConfirmationModal";
import { axiosClient } from "@/app/axios";

export default function LessonPage() {
  const { courseId, lessonId } = useParams<{
    courseId: string;
    lessonId: string;
  }>();
  const location = useLocation();
  const isManageRoute = location.pathname.startsWith("/manage/");
  const lessonSelfLink =
    (location.state as { lessonSelfLink?: string })?.lessonSelfLink ??
    (isManageRoute ? `/manage/lessons/${lessonId}` : undefined);

  const {
    data: lesson,
    isLoading,
    error,
  } = useLesson(courseId!, lessonId, lessonSelfLink);
  const { data: course } = useCourse(courseId!);

  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const patchLesson = usePatchLesson(courseId!, lessonId!);
  const generateAi = useGenerateLessonAi();
  const deleteLesson = useDeleteLesson(courseId!);
  const enrollmentId = useEnrollmentIdByCourseId(courseId ?? undefined);

  const { t } = useTranslation(["lesson-viewer", "translation"]);

  const handleVideoTimeUpdate = useCallback(
    (seconds: number) => {
      if (!enrollmentId || !lessonId) return;
      updateLessonProgress(enrollmentId, lessonId, seconds).catch(() => {
        // Fire-and-forget; avoid spamming user on network errors
      });
    },
    [enrollmentId, lessonId]
  );

  const handleVideoEnded = useCallback(() => {
    if (!enrollmentId || !lessonId) return;
    markLessonCompleted(enrollmentId, lessonId)
      .then(() => {
        toast.success(t("lesson-viewer:toolbar.markedComplete"));
        queryClient.invalidateQueries({ queryKey: enrollmentsQueryKeys.all });
      })
      .catch(() => {
        toast.error(t("common.error", { message: "Failed to mark lesson complete" }));
      });
  }, [enrollmentId, lessonId, queryClient, t]);

  const [aiTitle, setAiTitle] = useState<string | null>(null);
  const [aiDescription, setAiDescription] = useState<string | null>(null);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [deleteLinkHref, setDeleteLinkHref] = useState<string | null>(null);

  const handleAcceptTitle = async (newTitle: string) => {
    const updateLink = getLinkFromRecord(lesson?.links, "partialUpdate");
    if (!updateLink?.href) {
      console.error("No update link found for this lesson");
      return;
    }

    try {
      await patchLesson.mutateAsync({
        url: updateLink.href,
        request: { title: newTitle },
      });
      toast.success(t("lesson-viewer:actions.titleUpdated"));
      setAiTitle(null);
    } catch (error) {
      toast.error(t("lesson-viewer:actions.titleUpdateFailed"));
      throw error;
    }
  };

  const handleAcceptDescription = async (newDescription: string) => {
    const updateLink = getLinkFromRecord(lesson?.links, "partialUpdate");
    if (!updateLink?.href) {
      console.error("No update link found for this lesson");
      return;
    }

    try {
      await patchLesson.mutateAsync({
        url: updateLink.href,
        request: { description: newDescription },
      });
      toast.success(t("lesson-viewer:actions.descriptionUpdated"));
      setAiDescription(null);
    } catch (error) {
      toast.error(t("lesson-viewer:actions.descriptionUpdateFailed"));
      throw error;
    }
  };

  const handleAccessChange = async (checked: boolean) => {
    const updateLink = getLinkFromRecord(lesson?.links, "partialUpdate");
    if (!updateLink?.href) return;
    const newAccess = checked ? "Public" : "Private";
    try {
      await patchLesson.mutateAsync({
        url: updateLink.href,
        request: { access: newAccess },
      });
      toast.success(t("lesson-viewer:accessLevel.changeSuccess"));
    } catch (error) {
      toast.error(t("lesson-viewer:accessLevel.changeFailed"));
      throw error;
    }
  };

  const handleGenerateWithAi = async () => {
    const aiGenerateLink = getLinkFromRecord(lesson?.links, "aiGenerate");
    if (!aiGenerateLink?.href) {
      console.error("No AI generate link found for this lesson");
      return;
    }

    try {
      const result = await generateAi.mutateAsync(aiGenerateLink.href);

      setAiTitle(result.title);
      setAiDescription(result.description);

      toast.success(t("lesson-viewer:actions.aiGenerateSuccess"));
    } catch (error: unknown) {
      const axiosError = error as { response?: { data?: { title?: string } } };
      if (axiosError?.response?.data?.title === "Lesson.NoTranscript") {
        toast.error(t("lesson-viewer:actions.aiGenerateNoTranscript"));
      } else {
        toast.error(t("lesson-viewer:actions.aiGenerateFailed"));
      }
      console.error("Failed to generate with AI:", error);
    }
  };

  const getRouteForHref = useCallback(
    (href: string) => apiHrefToAppRoute(href, { courseId: courseId ?? undefined }),
    [courseId]
  );

  const getStateForHref = useCallback((href: string, rel: string) => {
    if (rel === "manage" || rel === "self") {
      return { lessonSelfLink: href };
    }
    return undefined;
  }, []);

  const lessonLabelByRel: Record<string, string> = {
    managedCourse: t("lesson-viewer:toolbar.backToCourse", { defaultValue: LINK_LABELS.managedCourse }),
    publicPreview: t("lesson-viewer:toolbar.previewCourse", { defaultValue: LINK_LABELS.publicPreview }),
    partialUpdate: t("common.edit"),
    delete: t("common.delete"),
    generateVideoUploadUrl: t("lesson-viewer:toolbar.uploadVideo", { defaultValue: LINK_LABELS.generateVideoUploadUrl }),
    aiGenerate: t("lesson-viewer:actions.generateWithAi"),
    manageTranscript: t("lesson-viewer:toolbar.editTranscript", { defaultValue: LINK_LABELS.manageTranscript }),
    nextLesson: t("lesson-viewer:toolbar.nextLesson", { defaultValue: LINK_LABELS.nextLesson }),
    previousLesson: t("lesson-viewer:toolbar.previousLesson", { defaultValue: LINK_LABELS.previousLesson }),
    course: t("lesson-viewer:toolbar.backToCourse", { defaultValue: LINK_LABELS.course }),
    markAsComplete: t("lesson-viewer:toolbar.markComplete", { defaultValue: LINK_LABELS.markAsComplete }),
    unmarkAsComplete: t("lesson-viewer:toolbar.unmarkComplete", { defaultValue: LINK_LABELS.unmarkAsComplete }),
    manage: t("lesson-viewer:toolbar.manage", { defaultValue: LINK_LABELS.manage }),
  };

  const handleLessonAction = useCallback(
    async (rel: string, link: { href?: string | null; method?: string | null }) => {
      if (rel === "delete" && link?.href) {
        setDeleteLinkHref(link.href);
        setIsDeleteDialogOpen(true);
        return;
      }
      if ((rel === "markAsComplete" || rel === "unmarkAsComplete") && link?.href) {
        try {
          if (rel === "markAsComplete") {
            await axiosClient.post(link.href);
            toast.success(t("lesson-viewer:toolbar.markedComplete"));
          } else {
            await axiosClient.delete(link.href);
            toast.success(t("lesson-viewer:toolbar.unmarkedComplete"));
          }
        } catch (err) {
          toast.error(t("common.error", { message: String(err) }));
        }
      }
    },
    [t]
  );

  const handleConfirmDeleteLesson = useCallback(async () => {
    if (!deleteLinkHref) return;
    try {
      await deleteLesson.mutateAsync(deleteLinkHref);
      toast.success(t("lesson-viewer:actions.deleteSuccess"));
      navigate(
        courseId
          ? isManageRoute
            ? `/manage/courses/${courseId}`
            : `/courses/${courseId}`
          : "/catalog"
      );
    } catch {
      toast.error(t("lesson-viewer:actions.deleteFailed"));
    } finally {
      setIsDeleteDialogOpen(false);
      setDeleteLinkHref(null);
    }
  }, [deleteLinkHref, deleteLesson, navigate, courseId, isManageRoute, t]);

  if (isLoading) {
    return (
      <div className="space-y-6">
        <div className="bg-background border-b border-border py-3 px-8">
          <div className="max-w-5xl mx-auto">
            <Skeleton className="h-4 w-64" />
          </div>
        </div>
        <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-6">
          <Skeleton className="w-full aspect-video rounded-xl shadow-sm" />
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

  const formattedDuration = formatDuration(lesson.duration);

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

  const hasLessonLinks = lesson.links && Object.keys(lesson.links).length > 0;

  return (
    <div className="space-y-8">
      <BreadcrumbNav items={breadcrumbItems} />
      {hasLessonLinks && (
        <div className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex flex-wrap items-center justify-end gap-2 py-2 border-b border-border">
            <LinkButtons
              links={lesson.links}
              labelByRel={lessonLabelByRel}
              onAction={handleLessonAction}
              excludeRels={["self", "partialUpdate", "generateVideoUploadUrl", "aiGenerate"]}
              getRouteForHref={getRouteForHref}
              getStateForHref={getStateForHref}
              variant="outline"
              size="sm"
              className="flex-wrap"
            />
          </div>
        </div>
      )}
      <ConfirmationModal
        open={isDeleteDialogOpen}
        onOpenChange={(open) => {
          if (!open) setDeleteLinkHref(null);
          setIsDeleteDialogOpen(open);
        }}
        onConfirm={handleConfirmDeleteLesson}
        title={t("lesson-viewer:actions.deleteConfirmTitle")}
        message={t("lesson-viewer:actions.deleteConfirmMessage")}
        confirmText={t("common.delete")}
        cancelText={t("common.cancel")}
        isLoading={deleteLesson.isPending}
      />
      <motion.div
        className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-8"
        variants={container}
        initial="hidden"
        animate="show"
      >
        {lesson.videoUrl ? (
          <motion.div variants={item}>
            <Card className="overflow-hidden border-0 shadow-sm rounded-xl bg-black">
              <CardContent className="p-0">
                <HlsVideoPlayer
                  src={lesson.videoUrl}
                  poster={lesson.thumbnailImage ?? undefined}
                  onTimeUpdate={
                    !isManageRoute && enrollmentId
                      ? handleVideoTimeUpdate
                      : undefined
                  }
                  onEnded={
                    !isManageRoute && enrollmentId
                      ? handleVideoEnded
                      : undefined
                  }
                  transcripts={
                    lesson.transcriptUrl
                      ? [
                          {
                            src: lesson.transcriptUrl,
                            label: "עברית",
                            lang: "he-IL",
                            isDefault: true,
                          },
                        ]
                      : []
                  }
                />
              </CardContent>
            </Card>
          </motion.div>
        ) : (
          <motion.div variants={item}>
            <Card className="overflow-hidden border-0 shadow-sm rounded-xl">
              <CardContent className="p-12 text-center space-y-4">
                <p className="text-muted-foreground">
                  {t("lesson-viewer:pages.lesson.noVideo")}
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
          <Card className="shadow-sm">
            <CardHeader className="space-y-4 p-6">
              {/* AI Generation Button */}
              {!!getLinkFromRecord(lesson.links, "aiGenerate")?.href &&
                !!lesson.links?.partialUpdate?.href && (
                  <div className="flex flex-wrap items-center justify-between gap-3 p-3 ps-4 bg-muted/60 rounded-lg border-s-4 border-primary">
                    <div className="flex items-center gap-3">
                      <Sparkles className="h-4 w-4 text-primary shrink-0" />
                      <div>
                        <p className="text-sm font-medium">
                          {t("lesson-viewer:actions.aiContentGenerator", { defaultValue: "AI Content Generator" })}
                        </p>
                        <p className="text-xs text-muted-foreground">
                          {t("lesson-viewer:actions.aiGenerateHint", { defaultValue: "Generate title and description from transcript" })}
                        </p>
                      </div>
                    </div>
                    <Button
                      onClick={handleGenerateWithAi}
                      disabled={generateAi.isPending}
                      variant="outline"
                      size="sm"
                      className="gap-2 shrink-0"
                    >
                      {generateAi.isPending ? (
                        <>
                          <div className="h-4 w-4 border-2 border-white border-t-transparent rounded-full animate-spin" />
                          {t("lesson-viewer:actions.generatingWithAi")}
                        </>
                      ) : (
                        <>
                          <Sparkles className="h-4 w-4" />
                          {t("lesson-viewer:actions.generateWithAi")}
                        </>
                      )}
                    </Button>
                  </div>
                )}

              {/* AI Suggestions Display */}
              {aiTitle && (
                <AiSuggestionField
                  label="Title"
                  originalValue={lesson.title}
                  suggestedValue={aiTitle}
                  onAccept={handleAcceptTitle}
                  onReject={() => setAiTitle(null)}
                  type="text"
                  placeholder={t("lesson-viewer:actions.enterTitle")}
                  maxLength={200}
                />
              )}

              {aiDescription && (
                <AiSuggestionField
                  label="Description"
                  originalValue={lesson.description || ""}
                  suggestedValue={aiDescription}
                  onAccept={handleAcceptDescription}
                  onReject={() => setAiDescription(null)}
                  type="textarea"
                  placeholder={t("lesson-viewer:actions.enterDescription")}
                  maxLength={2000}
                  rows={5}
                />
              )}

              {/* Title Section */}
              {!aiTitle && (
                <div className="flex items-start justify-between gap-4 flex-wrap">
                  {!!lesson.links?.partialUpdate?.href ? (
                    <div className="flex-1">
                      <InlineEditableText
                        value={lesson.title}
                        onSave={handleAcceptTitle}
                        displayClassName="text-2xl font-semibold"
                        inputClassName="text-2xl font-semibold"
                        placeholder={t("lesson-viewer:actions.enterTitle")}
                        maxLength={200}
                      />
                    </div>
                  ) : (
                    <CardTitle className="text-2xl" dir="auto">
                      {lesson.title}
                    </CardTitle>
                  )}
                  <div className="flex flex-wrap items-center gap-2">
                    {!!lesson.links?.partialUpdate?.href && (
                      <div className="flex items-center gap-1.5 shrink-0">
                        <Switch
                          checked={lesson.isPreview}
                          onCheckedChange={handleAccessChange}
                          disabled={patchLesson.isPending}
                          aria-label={t("lesson-viewer:accessLevel.label")}
                        />
                        <span className="text-sm text-muted-foreground">
                          {lesson.isPreview
                            ? t("lesson-viewer:accessLevel.freePreview")
                            : t("lesson-viewer:accessLevel.private")}
                        </span>
                      </div>
                    )}
                    {formattedDuration && (
                      <div className="flex items-center gap-1 text-sm text-muted-foreground bg-secondary/50 px-3 py-1 rounded-full">
                        <Clock className="h-4 w-4" />
                        {formattedDuration}
                      </div>
                    )}
                    <LessonVideoUpload
                      courseId={lesson.courseId}
                      lessonId={lesson.lessonId}
                      links={lesson.links}
                    />
                  </div>
                </div>
              )}
            </CardHeader>

            {/* Description Section */}
            {!aiDescription && (
              <CardContent className="pt-0 px-6 pb-6">
                <div className="space-y-2">
                  <h2 className="text-lg font-semibold">
                    {t("lesson-viewer:pages.lesson.description")}
                  </h2>
                  {!!lesson.links?.partialUpdate?.href ? (
                    <InlineEditableTextarea
                      value={lesson.description || ""}
                      onSave={handleAcceptDescription}
                      displayClassName="text-muted-foreground leading-relaxed"
                      placeholder={t("lesson-viewer:actions.enterDescription")}
                      rows={5}
                      maxLength={2000}
                      renderAsMarkdown={true}
                    />
                  ) : lesson.description ? (
                    <RichTextViewer content={lesson.description} />
                  ) : (
                    <p className="text-muted-foreground italic">
                      {t("lesson-viewer:actions.noDescription")}
                    </p>
                  )}
                </div>
              </CardContent>
            )}
          </Card>
        </motion.div>
      </motion.div>
    </div>
  );
}
