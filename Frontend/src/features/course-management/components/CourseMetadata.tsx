import { useTranslation } from "react-i18next";
import { Card, CardHeader, CardTitle, CardContent, Badge } from "@/shared/ui";
import { Avatar } from "@/shared/ui";
import { Users, BookOpen, Clock, Tag, FolderTree } from "lucide-react";
import type { CourseModel } from "@/domain/courses";

interface CourseMetadataProps {
  course: CourseModel;
}

/**
 * CourseMetadata: Displays course information like instructor, stats, category, and tags
 */
export function CourseMetadata({ course }: CourseMetadataProps) {
  const { t, i18n } = useTranslation(["courses", "translation"]);
  const isRTL = i18n.dir() === "rtl";
  const textAlignClass = isRTL ? "text-right" : "text-left";

  const formatDuration = (duration: string | undefined) => {
    if (!duration) return "00:00:00";
    return duration;
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle className={textAlignClass}>
          {t("courses:detail.courseInfo")}
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-6">
        {/* Instructor Info */}
        <div
          className={`flex items-center gap-3 ${isRTL ? "flex-row-reverse" : ""}`}
        >
          <Avatar className="h-12 w-12">
            {course.instructorAvatarUrl ? (
              <img
                src={course.instructorAvatarUrl}
                alt={course.instructorName || "Instructor"}
                className="h-full w-full object-cover"
              />
            ) : (
              <div className="flex h-full w-full items-center justify-center bg-primary/10 text-primary font-semibold">
                {course.instructorName?.charAt(0) || "I"}
              </div>
            )}
          </Avatar>
          <div className={textAlignClass}>
            <p className="text-sm text-muted-foreground">
              {t("courses:detail.instructor")}
            </p>
            <p className="font-semibold">{course.instructorName}</p>
          </div>
        </div>

        {/* Course Stats */}
        <div className="grid grid-cols-2 gap-4">
          <div
            className={`flex items-center gap-2 ${isRTL ? "flex-row-reverse" : ""}`}
          >
            <Users className="h-4 w-4 text-muted-foreground" />
            <div className={textAlignClass}>
              <p className="text-sm text-muted-foreground">
                {t("courses:detail.enrolled")}
              </p>
              <p className="font-semibold">{course.enrollmentCount || 0}</p>
            </div>
          </div>

          <div
            className={`flex items-center gap-2 ${isRTL ? "flex-row-reverse" : ""}`}
          >
            <BookOpen className="h-4 w-4 text-muted-foreground" />
            <div className={textAlignClass}>
              <p className="text-sm text-muted-foreground">
                {t("courses:detail.lessons")}
              </p>
              <p className="font-semibold">{course.lessonCount || 0}</p>
            </div>
          </div>

          {course.totalDuration && (
            <div
              className={`flex items-center gap-2 ${isRTL ? "flex-row-reverse" : ""}`}
            >
              <Clock className="h-4 w-4 text-muted-foreground" />
              <div className={textAlignClass}>
                <p className="text-sm text-muted-foreground">
                  {t("courses:detail.duration")}
                </p>
                <p className="font-semibold">
                  {formatDuration(course.totalDuration)}
                </p>
              </div>
            </div>
          )}

          {course.categoryName && (
            <div
              className={`flex items-center gap-2 ${isRTL ? "flex-row-reverse" : ""}`}
            >
              <FolderTree className="h-4 w-4 text-muted-foreground" />
              <div className={textAlignClass}>
                <p className="text-sm text-muted-foreground">
                  {t("courses:detail.category")}
                </p>
                <p className="font-semibold">{course.categoryName}</p>
              </div>
            </div>
          )}
        </div>

        {/* Tags */}
        {course.tags && course.tags.length > 0 && (
          <div className={textAlignClass}>
            <div
              className={`flex items-center gap-2 mb-2 ${isRTL ? "flex-row-reverse" : ""}`}
            >
              <Tag className="h-4 w-4 text-muted-foreground" />
              <p className="text-sm text-muted-foreground">
                {t("courses:detail.tags")}
              </p>
            </div>
            <div
              className={`flex flex-wrap gap-2 ${isRTL ? "justify-end" : ""}`}
            >
              {course.tags.map((tag, index) => (
                <Badge key={index} variant="secondary">
                  {tag}
                </Badge>
              ))}
            </div>
          </div>
        )}
      </CardContent>
    </Card>
  );
}
