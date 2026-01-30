import { useTranslation } from "react-i18next";
import { Button, Card, Badge } from "@/shared/ui";
import { Avatar } from "@/shared/ui";
import { InlineEditableText } from "@/shared/common";
import { ShoppingCart, CreditCard, Users, BookOpen, Clock, Tag } from "lucide-react";
import { motion } from "framer-motion";
import { CourseActions } from "./CourseActions";
import { CourseImageUpload } from "./CourseImageUpload";
import { usePatchCourse } from "@/domain/courses";
import { toast } from "sonner";
import { hasLink, getLink } from "@/shared/utils";
import { CourseRels } from "@/domain/courses";
import type { CourseModel } from "@/domain/courses";

interface CourseHeaderProps {
  course: CourseModel;
}

export function CourseHeader({ course }: CourseHeaderProps) {
  const { t, i18n } = useTranslation(["courses", "translation"]);
  const patchCourse = usePatchCourse(course.id);

  const isRTL = i18n.dir() === "rtl";
  const textAlignClass = isRTL ? "text-right" : "text-left";
  
  // Check if user can update course based on HATEOAS links
  const canUpdate = hasLink(course.links, CourseRels.PARTIAL_UPDATE);
  const updateLink = getLink(course.links, CourseRels.PARTIAL_UPDATE);

  const handleTitleUpdate = async (newTitle: string) => {
    if (!updateLink) {
      console.error("No update link found for this course");
      return;
    }
    
    try {
      await patchCourse.mutateAsync({ url: updateLink.href, request: { title: newTitle } });
      toast.success(t("courses:detail.titleUpdated"));
    } catch (error) {
      toast.error(t("courses:detail.titleUpdateFailed"));
      throw error;
    }
  };

  const formatDuration = (duration: string | undefined) => {
    if (!duration || duration === "00:00:00") return null;
    return duration;
  };

  const formattedDuration = formatDuration(course.totalDuration);

  return (
    <Card className="overflow-hidden">
      <div className="grid lg:grid-cols-[2fr_3fr] gap-6 p-6">
        {/* Image Section */}
        {course.imageUrl && (
          <div className="relative h-64 lg:h-80 overflow-hidden rounded-lg">
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
        
        {/* Content Section */}
        <div className="space-y-4">
          {/* Title */}
          <div className={textAlignClass}>
            {canUpdate ? (
              <InlineEditableText
                value={course.title}
                onSave={handleTitleUpdate}
                displayClassName={`text-3xl font-bold break-words ${textAlignClass}`}
                inputClassName={`text-3xl font-bold ${textAlignClass}`}
                placeholder={t("courses:detail.enterTitle")}
                maxLength={200}
              />
            ) : (
              <h1
                dir="auto"
                className={`text-3xl font-bold break-words ${textAlignClass}`}
              >
                {course.title}
              </h1>
            )}
          </div>

          {/* Instructor Info */}
          <div className={`flex items-center gap-3 ${isRTL ? "flex-row-reverse" : ""}`}>
            <Avatar className="h-10 w-10">
              {course.instructorAvatarUrl ? (
                <img
                  src={course.instructorAvatarUrl}
                  alt={course.instructorName || "Instructor"}
                  className="h-full w-full object-cover"
                />
              ) : (
                <div className="flex h-full w-full items-center justify-center bg-primary/10 text-primary font-semibold text-sm">
                  {course.instructorName?.charAt(0) || "I"}
                </div>
              )}
            </Avatar>
            <div className={textAlignClass}>
              <p className="text-sm text-muted-foreground">
                {t("courses:detail.instructor")}
              </p>
              <p className="font-medium">{course.instructorName}</p>
            </div>
          </div>

          {/* Stats Grid */}
          <div className="grid grid-cols-3 gap-4 py-2">
            <div className={`flex flex-col ${textAlignClass}`}>
              <div className={`flex items-center gap-1.5 text-muted-foreground mb-1 ${isRTL ? "flex-row-reverse" : ""}`}>
                <Users className="h-4 w-4" />
                <span className="text-xs">{t("courses:detail.enrolled")}</span>
              </div>
              <span className="text-lg font-semibold">{course.enrollmentCount || 0}</span>
            </div>

            <div className={`flex flex-col ${textAlignClass}`}>
              <div className={`flex items-center gap-1.5 text-muted-foreground mb-1 ${isRTL ? "flex-row-reverse" : ""}`}>
                <BookOpen className="h-4 w-4" />
                <span className="text-xs">{t("courses:detail.lessons")}</span>
              </div>
              <span className="text-lg font-semibold">{course.lessonCount || 0}</span>
            </div>

            {formattedDuration && (
              <div className={`flex flex-col ${textAlignClass}`}>
                <div className={`flex items-center gap-1.5 text-muted-foreground mb-1 ${isRTL ? "flex-row-reverse" : ""}`}>
                  <Clock className="h-4 w-4" />
                  <span className="text-xs">{t("courses:detail.duration")}</span>
                </div>
                <span className="text-lg font-semibold">{formattedDuration}</span>
              </div>
            )}
          </div>

          {/* Category & Tags */}
          {(course.categoryName || (course.tags && course.tags.length > 0)) && (
            <div className={`space-y-2 ${textAlignClass}`}>
              {course.categoryName && course.categoryName !== "Empty" && (
                <div className={`flex items-center gap-2 ${isRTL ? "flex-row-reverse" : ""}`}>
                  <Tag className="h-3.5 w-3.5 text-muted-foreground" />
                  <span className="text-sm text-muted-foreground">{course.categoryName}</span>
                </div>
              )}
              {course.tags && course.tags.length > 0 && (
                <div className={`flex flex-wrap gap-1.5 ${isRTL ? "justify-end" : ""}`}>
                  {course.tags.map((tag, index) => (
                    <Badge key={index} variant="secondary" className="text-xs">
                      {tag}
                    </Badge>
                  ))}
                </div>
              )}
            </div>
          )}

          {/* Price & Actions */}
          <div className="flex items-center justify-between gap-3 pt-2">
            <div className={textAlignClass}>
              <p className="text-2xl font-bold">
                {course.price.amount > 0 
                  ? `${course.price.amount} ${course.price.currency}`
                  : t("courses:detail.free")
                }
              </p>
            </div>
            
            <div className={`flex items-center gap-2 ${isRTL ? "flex-row-reverse" : ""}`}>
              <CourseImageUpload courseId={course.id} links={course.links} />
              <CourseActions courseId={course.id} links={course.links} />
            </div>
          </div>

          {/* Action Buttons */}
          <div className="flex gap-3 pt-2">
            <motion.div
              className="flex-1"
              whileHover={{ scale: 1.02 }}
              whileTap={{ scale: 0.98 }}
            >
              <Button className="w-full gap-2" size="lg">
                <CreditCard className="h-4 w-4" />
                {t("courses:detail.buyNow")}
              </Button>
            </motion.div>
            <motion.div
              className="flex-1"
              whileHover={{ scale: 1.02 }}
              whileTap={{ scale: 0.98 }}
            >
              <Button variant="outline" className="w-full gap-2" size="lg">
                <ShoppingCart className="h-4 w-4" />
                {t("courses:detail.addToCart")}
              </Button>
            </motion.div>
          </div>
        </div>
      </div>
    </Card>
  );
}
