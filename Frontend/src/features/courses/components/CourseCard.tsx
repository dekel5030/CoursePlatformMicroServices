import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import type { CourseModel } from "../types";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  CardFooter,
  Badge,
} from "@/components";
import { BookOpen, Clock } from "lucide-react";

interface Props {
  course: CourseModel;
}

export default function CourseCard({ course }: Props) {
  const { t, i18n } = useTranslation(["courses", "translation"]);

  // --- תיקון הגנה (Defensive Coding) ---
  // 1. שימוש ב-Optional Chaining (?.) כדי לא לקרוס אם price הוא null
  // 2. שימוש ב-Default Value (||) כדי לספק ערך ברירת מחדל
  const safeCurrency = course.price?.currency || "ILS";
  const safeAmount = course.price?.amount || 0;

  // לוג דיבאג - יעזור לך לזהות אם יש ב-DB רשומות פגומות
  if (!course.price?.currency) {
    console.warn(
      `[CourseCard] Missing currency for course: ${course.title} (${course.id})`,
      course,
    );
  }

  // יצירת הפורמטר עם הערכים הבטוחים
  const formattedPrice = new Intl.NumberFormat(i18n.language || "he-IL", {
    style: "currency",
    currency: safeCurrency,
    maximumFractionDigits: 0,
  }).format(safeAmount);

  return (
    <Link
      to={`/courses/${course.id}`}
      className="block hover:opacity-80 transition-opacity"
    >
      <Card className="overflow-hidden hover:shadow-lg transition-shadow h-full flex flex-col">
        {/* טיפול בתמונה */}
        <div className="relative h-48 w-full overflow-hidden bg-gray-100">
          {course.imageUrl ? (
            <img
              src={course.imageUrl}
              alt={course.title}
              className="h-full w-full object-cover transition-transform duration-300 hover:scale-105"
            />
          ) : (
            <div className="flex items-center justify-center h-full text-muted-foreground">
              <BookOpen className="h-10 w-10 opacity-20" />
            </div>
          )}
        </div>

        <CardHeader>
          <CardTitle className="line-clamp-2" title={course.title}>
            {course.title}
          </CardTitle>
        </CardHeader>

        <CardContent className="space-y-3 flex-grow">
          {course.description && (
            <p className="text-sm text-muted-foreground line-clamp-3">
              {course.description}
            </p>
          )}

          <div className="flex items-center justify-between mt-auto">
            <div className="flex items-center gap-1 text-sm font-bold text-primary">
              {/* כאן אנחנו משתמשים במחיר המפורמט הבטוח */}
              {formattedPrice}
            </div>
            <Badge variant={course.isPublished ? "default" : "secondary"}>
              {course.isPublished
                ? t("courses:card.published")
                : t("courses:card.draft")}
            </Badge>
          </div>
        </CardContent>

        <CardFooter className="flex items-center justify-between text-xs text-muted-foreground border-t pt-4">
          <div className="flex items-center gap-1">
            <BookOpen className="h-3 w-3" />
            {t("courses:card.lessonsCount", {
              count: course.lessonCount || 0,
            })}
          </div>
          {course.updatedAtUtc && (
            <div className="flex items-center gap-1">
              <Clock className="h-3 w-3" />
              {new Date(course.updatedAtUtc).toLocaleDateString(i18n.language)}
            </div>
          )}
        </CardFooter>
      </Card>
    </Link>
  );
}
