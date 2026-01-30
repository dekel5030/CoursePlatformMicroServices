import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import type { CourseModel } from "@/domain/courses";
import { DifficultyLevel, CourseStatus } from "@/domain/courses";
import { Card, CardContent, CardHeader, Badge, Avatar } from "@/shared/ui";
import { BookOpen, Clock, Users, Eye, Star, TrendingUp } from "lucide-react";

interface Props {
  course: CourseModel;
}

export default function CourseCard({ course }: Props) {
  const { t, i18n } = useTranslation(["courses", "translation"]);

  const safeCurrency = course.price?.currency || "ILS";
  const safeAmount = course.price?.amount || 0;

  const formattedPrice = new Intl.NumberFormat(i18n.language || "he-IL", {
    style: "currency",
    currency: safeCurrency,
    maximumFractionDigits: 0,
  }).format(safeAmount);

  const formattedOriginalPrice = course.originalPrice
    ? new Intl.NumberFormat(i18n.language || "he-IL", {
        style: "currency",
        currency: course.originalPrice.currency,
        maximumFractionDigits: 0,
      }).format(course.originalPrice.amount)
    : null;

  const hasDiscount =
    course.originalPrice && course.originalPrice.amount > safeAmount;

  const formatDuration = (duration?: string) => {
    if (!duration || duration === "00:00:00") return null;
    const parts = duration.split(":");
    const hours = parseInt(parts[0]);
    const minutes = parseInt(parts[1]);
    if (hours > 0) return `${hours}h ${minutes}m`;
    return `${minutes}m`;
  };

  const durationText = formatDuration(course.totalDuration);

  const getDifficultyLabel = (difficulty: number) => {
    const difficultyKey = Object.keys(DifficultyLevel).find(
      key => DifficultyLevel[key as keyof typeof DifficultyLevel] === difficulty
    );
    const key = difficultyKey?.toLowerCase() || "";
    if (!key) return "";
    return t(`courses:difficulty.${key}`, { defaultValue: difficultyKey || "" });
  };

  // Don't show "Empty" category
  const showCategory = course.categoryName && course.categoryName !== "Empty";

  return (
    <Link to={`/courses/${course.id}`} className="block group">
      <Card className="overflow-hidden hover:shadow-2xl transition-all duration-300 h-full flex flex-col border hover:border-primary/40 bg-white">
        {/* Image Section */}
        <div className="relative h-44 w-full overflow-hidden bg-gradient-to-br from-gray-50 to-gray-100">
          {course.imageUrl ? (
            <img
              src={course.imageUrl}
              alt={course.title}
              className="h-full w-full object-cover transition-transform duration-700 group-hover:scale-105"
            />
          ) : (
            <div className="flex items-center justify-center h-full text-muted-foreground">
              <BookOpen className="h-16 w-16 opacity-20" />
            </div>
          )}

          {/* Top-left badges stack */}
          <div className="absolute top-2 left-2 flex flex-col gap-2">
            {/* Difficulty Badge */}
            {course.difficulty !== undefined && (
              <Badge className="bg-primary text-white font-semibold shadow-lg border-0">
                {t("courses:difficulty.label")}{" "}
                {getDifficultyLabel(course.difficulty)}
              </Badge>
            )}

            {/* Discount badge */}
            {hasDiscount && (
              <Badge className="bg-red-500 text-white font-semibold shadow-lg border-0">
                <TrendingUp className="h-3 w-3 mr-1" />
                {t("courses:card.sale")}
              </Badge>
            )}
          </div>

          {/* Status Badge - Only show if Draft */}
          {course.status === CourseStatus.Draft && (
            <div className="absolute top-2 right-2">
              <Badge variant="secondary" className="shadow-md font-medium">
                {t("courses:status.draft")}
              </Badge>
            </div>
          )}

          {/* Bottom Overlay with Category */}
          {showCategory && (
            <div className="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black/60 to-transparent p-3">
              <span className="text-white text-xs font-medium bg-black/30 px-2 py-1 rounded backdrop-blur-sm">
                {course.categoryName}
              </span>
            </div>
          )}
        </div>

        {/* Content */}
        <CardHeader className="pb-3">
          <h3
            className="font-bold text-base line-clamp-2 leading-snug group-hover:text-primary transition-colors"
            title={course.title}
          >
            {course.title}
          </h3>
        </CardHeader>

        <CardContent className="flex-grow flex flex-col p-4 space-y-3">
          {/* Short description */}
          {course.shortDescription && (
            <p className="text-sm text-muted-foreground line-clamp-2">
              {course.shortDescription}
            </p>
          )}

          {/* Instructor */}
          {course.instructorName && (
            <div className="flex items-center gap-2">
              <Avatar className="h-7 w-7 border">
                {course.instructorAvatarUrl ? (
                  <img
                    src={course.instructorAvatarUrl}
                    alt={course.instructorName}
                    className="object-cover"
                  />
                ) : (
                  <div className="bg-primary text-white flex items-center justify-center h-full w-full text-xs font-bold">
                    {course.instructorName.charAt(0).toUpperCase()}
                  </div>
                )}
              </Avatar>
              <span className="text-xs text-muted-foreground font-medium truncate">
                {course.instructorName}
              </span>
            </div>
          )}

          {/* Rating */}
          {course.averageRating !== undefined && course.averageRating > 0 && (
            <div className="flex items-center gap-2">
              <div className="flex items-center">
                {[...Array(5)].map((_, i) => (
                  <Star
                    key={i}
                    className={`h-3.5 w-3.5 ${
                      i < Math.round(course.averageRating!)
                        ? "fill-yellow-400 text-yellow-400"
                        : "fill-gray-200 text-gray-200"
                    }`}
                  />
                ))}
              </div>
              <span className="text-xs font-bold text-gray-700">
                {course.averageRating.toFixed(1)}
              </span>
              {course.reviewsCount !== undefined && course.reviewsCount > 0 && (
                <span className="text-xs text-muted-foreground">
                  ({course.reviewsCount.toLocaleString()})
                </span>
              )}
            </div>
          )}

          {/* Course Stats */}
          <div className="flex items-center gap-3 text-xs text-muted-foreground flex-wrap">
            {course.lessonCount !== undefined && course.lessonCount > 0 && (
              <div className="flex items-center gap-1">
                <BookOpen className="h-3.5 w-3.5" />
                <span>{course.lessonCount}</span>
              </div>
            )}

            {durationText && (
              <div className="flex items-center gap-1">
                <Clock className="h-3.5 w-3.5" />
                <span>{durationText}</span>
              </div>
            )}

            {course.enrollmentCount !== undefined &&
              course.enrollmentCount > 0 && (
                <div className="flex items-center gap-1">
                  <Users className="h-3.5 w-3.5" />
                  <span>{course.enrollmentCount.toLocaleString()}</span>
                </div>
              )}

            {course.courseViews !== undefined && course.courseViews > 0 && (
              <div className="flex items-center gap-1">
                <Eye className="h-3.5 w-3.5" />
                <span>{course.courseViews.toLocaleString()}</span>
              </div>
            )}
          </div>

          {/* Spacer */}
          <div className="flex-grow"></div>

          {/* Price Section */}
          <div className="pt-3 border-t mt-auto">
            <div className="flex items-center justify-between">
              <div className="flex flex-col">
                {formattedOriginalPrice && hasDiscount && (
                  <span className="text-xs text-muted-foreground line-through">
                    {formattedOriginalPrice}
                  </span>
                )}
                <span className="text-xl font-bold text-primary">
                  {formattedPrice}
                </span>
              </div>

              {/* Show badges if available */}
              {course.badges && course.badges.length > 0 && (
                <div className="flex gap-1">
                  {course.badges.slice(0, 2).map((badge, index) => (
                    <Badge
                      key={index}
                      variant="outline"
                      className="text-xs font-medium"
                    >
                      {badge}
                    </Badge>
                  ))}
                </div>
              )}
            </div>
          </div>
        </CardContent>
      </Card>
    </Link>
  );
}
