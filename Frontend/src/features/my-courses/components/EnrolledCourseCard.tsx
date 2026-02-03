import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { Card, CardContent, CardHeader } from "@/shared/ui";
import type { EnrolledCourseDto } from "@/domain/enrollments";
import { BookOpen } from "lucide-react";

interface EnrolledCourseCardProps {
  course: EnrolledCourseDto;
}

function formatLastAccessed(dateStr: string | null): string {
  if (!dateStr) return "";
  const date = new Date(dateStr);
  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(date);
}

export function EnrolledCourseCard({ course }: EnrolledCourseCardProps) {
  const { t } = useTranslation("translation");
  const courseUrl = `/courses/${course.courseId}`;
  const progress = Math.min(100, Math.max(0, course.progressPercentage ?? 0));
  const lastAccessed = formatLastAccessed(course.lastAccessedAt);

  return (
    <Link to={courseUrl} className="block group">
      <Card className="overflow-hidden hover:shadow-lg transition-all duration-300 h-full flex flex-col border hover:border-primary/40">
        <div className="relative h-32 w-full overflow-hidden bg-gradient-to-br from-primary/5 to-primary/10">
          {course.courseImageUrl ? (
            <img
              src={course.courseImageUrl}
              alt={course.courseTitle ?? ""}
              className="h-full w-full object-cover transition-transform duration-300 group-hover:scale-105"
            />
          ) : (
            <div className="flex items-center justify-center h-full">
              <BookOpen className="h-12 w-12 text-primary/30" />
            </div>
          )}
        </div>

        <CardHeader className="pb-2">
          <h3
            className="font-bold text-base line-clamp-2 leading-snug group-hover:text-primary transition-colors"
            title={course.courseTitle ?? undefined}
          >
            {course.courseTitle || t("common.unknown")}
          </h3>
        </CardHeader>

        <CardContent className="flex-grow flex flex-col space-y-3 pt-0">
          <div>
            <p className="text-xs text-muted-foreground mb-1">
              {t("myCourses.progress")}
            </p>
            <div className="h-2 w-full rounded-full bg-muted overflow-hidden">
              <div
                className="h-full bg-primary rounded-full transition-all duration-500"
                style={{ width: `${progress}%` }}
              />
            </div>
            <p className="text-xs font-medium mt-1">{Math.round(progress)}%</p>
          </div>

          {lastAccessed && (
            <p className="text-xs text-muted-foreground">
              <span className="font-medium">{t("myCourses.lastAccessed")}:</span>{" "}
              {lastAccessed}
            </p>
          )}
        </CardContent>
      </Card>
    </Link>
  );
}
