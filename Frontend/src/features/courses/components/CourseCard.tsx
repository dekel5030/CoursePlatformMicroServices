import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import type { Course } from "../types";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  CardFooter,
  Badge,
} from "@/components";
import { BookOpen, Clock, DollarSign } from "lucide-react";

interface Props {
  course: Course;
}

export default function CourseCard({ course }: Props) {
  const { t } = useTranslation(['courses', 'translation']);

  return (
    <Link
      to={`/courses/${course.id}`}
      className="block hover:opacity-80 transition-opacity"
    >
      <Card className="overflow-hidden hover:shadow-lg transition-shadow">
        {course.imageUrl && (
          <div className="relative h-48 w-full overflow-hidden">
            <img
              src={course.imageUrl}
              alt={course.title}
              className="h-full w-full object-cover"
            />
          </div>
        )}

        <CardHeader>
          <CardTitle className="line-clamp-2">{course.title}</CardTitle>
        </CardHeader>

        <CardContent className="space-y-3">
          {course.description && (
            <p className="text-sm text-muted-foreground line-clamp-3">
              {course.description}
            </p>
          )}

          <div className="flex items-center justify-between">
            <div className="flex items-center gap-1 text-sm font-semibold">
              <DollarSign className="h-4 w-4" />
              {course.price.amount} {course.price.currency}
            </div>
            <Badge variant={course.isPublished ? "default" : "secondary"}>
              {course.isPublished ? t('courses:card.published') : t('courses:card.draft')}
            </Badge>
          </div>
        </CardContent>

        <CardFooter className="flex items-center justify-between text-xs text-muted-foreground">
          <div className="flex items-center gap-1">
            <BookOpen className="h-3 w-3" />
            {t('courses:card.lessonsCount', { count: course.lessons?.length || 0 })}
          </div>
          <div className="flex items-center gap-1">
            <Clock className="h-3 w-3" />
            {new Date(course.updatedAtUtc).toLocaleDateString()}
          </div>
        </CardFooter>
      </Card>
    </Link>
  );
}
