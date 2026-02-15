import { useTranslation } from "react-i18next";
import { Card, Badge } from "@/shared/ui";
import { Avatar } from "@/shared/ui";
import { InlineEditableText } from "@/shared/common";
import { Users, BookOpen, Clock, Tag, Eye } from "lucide-react";
import { motion } from "framer-motion";
import { CourseActions } from "./CourseActions";
import { CourseImageUpload } from "./CourseImageUpload";
import { usePatchCourse } from "@/domain/courses";
import { toast } from "sonner";
import { getLinkFromRecord, formatDuration } from "@/shared/utils";
import type { CourseModel } from "@/domain/courses";

interface CourseHeaderProps {
  course: CourseModel;
}

export function CourseHeader({ course }: CourseHeaderProps) {
  const { t } = useTranslation(["course-management", "translation"]);
  const patchCourse = usePatchCourse(course.id);

  const updateLink = getLinkFromRecord(course.links, "partialUpdate");
  const canUpdate = !!updateLink?.href;

  const handleTitleUpdate = async (newTitle: string) => {
    if (!updateLink?.href) {
      console.error("No update link found for this course");
      return;
    }
    try {
      await patchCourse.mutateAsync({ url: updateLink.href!, request: { title: newTitle } });
      toast.success(t("course-management:detail.titleUpdated"));
    } catch (error) {
      toast.error(t("course-management:detail.titleUpdateFailed"));
      throw error;
    }
  };

  const formattedDuration = formatDuration(course.totalDuration);

  const showCategory = course.categoryName && course.categoryName !== "Empty";
  const showTags = course.tags && course.tags.length > 0;

  return (
    <Card className="overflow-hidden">
      <div className="grid lg:grid-cols-[minmax(0,1.2fr)_minmax(0,2fr)] gap-6 p-6">
        {/* Image */}
        {course.imageUrl && (
          <div className="relative aspect-video lg:aspect-[4/3] overflow-hidden rounded-lg bg-muted">
            <motion.img
              initial={{ scale: 1.05 }}
              animate={{ scale: 1 }}
              transition={{ duration: 0.4 }}
              src={course.imageUrl}
              alt={course.title}
              className="h-full w-full object-cover"
            />
          </div>
        )}

        {/* Content */}
        <div className="flex flex-col min-w-0 gap-4">
          {/* Title */}
          <div className="text-start" dir="auto">
            {canUpdate ? (
              <InlineEditableText
                value={course.title}
                onSave={handleTitleUpdate}
                displayClassName="text-2xl font-bold break-words text-start"
                inputClassName="text-2xl font-bold text-start"
                placeholder={t("course-management:detail.enterTitle")}
                maxLength={200}
              />
            ) : (
              <h1 className="text-2xl font-bold break-words text-start" dir="auto">
                {course.title}
              </h1>
            )}
          </div>

          {/* Instructor */}
          <div className="flex items-center gap-3">
            <Avatar className="h-9 w-9 shrink-0">
              {course.instructorAvatarUrl ? (
                <img
                  src={course.instructorAvatarUrl}
                  alt={course.instructorName || ""}
                  className="h-full w-full object-cover"
                />
              ) : (
                <div className="flex h-full w-full items-center justify-center bg-primary/10 text-primary text-sm font-medium">
                  {course.instructorName?.charAt(0) || "—"}
                </div>
              )}
            </Avatar>
            <div className="text-start min-w-0">
              <p className="text-xs text-muted-foreground">{t("course-management:detail.instructor")}</p>
              <p className="font-medium truncate">{course.instructorName}</p>
            </div>
          </div>

          {/* Stats: enrolled, lessons, duration */}
          <div className="flex flex-wrap items-center gap-x-5 gap-y-1 text-sm">
            <div className="flex items-center gap-1.5 text-muted-foreground">
              <Users className="h-3.5 w-3.5 shrink-0" />
              <span>{course.enrollmentCount ?? 0}</span>
              <span>{t("course-management:detail.enrolled")}</span>
            </div>
            <div className="flex items-center gap-1.5 text-muted-foreground">
              <BookOpen className="h-3.5 w-3.5 shrink-0" />
              <span>{course.lessonCount ?? 0}</span>
              <span>{t("course-management:detail.lessons")}</span>
            </div>
            {formattedDuration && (
              <div className="flex items-center gap-1.5 text-muted-foreground">
                <Clock className="h-3.5 w-3.5 shrink-0" />
                <span>{formattedDuration}</span>
              </div>
            )}
            <div className="flex items-center gap-1.5 text-muted-foreground">
              <Eye className="h-3.5 w-3.5 shrink-0" />
              <span>{course.courseViews ?? 0}</span>
              <span>{t("course-management:detail.views")}</span>
            </div>
          </div>

          {/* Category & Tags */}
          {(showCategory || showTags) && (
            <div className="flex flex-wrap items-center gap-2 text-start">
              {showCategory && (
                <div className="flex items-center gap-1.5 text-muted-foreground">
                  <Tag className="h-3.5 w-3.5 shrink-0" />
                  <span className="text-sm">{course.categoryName}</span>
                </div>
              )}
              {showTags && (
                <div className="flex flex-wrap gap-1.5">
                  {course.tags!.map((tag, index) => (
                    <Badge key={index} variant="secondary" className="text-xs font-normal">
                      {tag}
                    </Badge>
                  ))}
                </div>
              )}
            </div>
          )}

          {/* Price & actions row */}
          <div className="flex flex-wrap items-center justify-between gap-3 pt-2 border-t border-border">
            <p className="text-xl font-semibold text-start">
              {course.price.amount > 0
                ? `${course.price.amount} ${course.price.currency}`
                : t("course-management:detail.free")}
            </p>
            <div className="flex items-center gap-2">
              <CourseImageUpload courseId={course.id} links={course.links} />
              <CourseActions courseId={course.id} links={course.links} />
            </div>
          </div>

        </div>
      </div>
    </Card>
  );
}
