import { useTranslation } from "react-i18next";
import { Button, Card, InlineEditableText } from "@/components";
import { ShoppingCart, CreditCard } from "lucide-react";
import { motion } from "framer-motion";
import { CourseActions } from "./CourseActions";
import { usePatchCourse } from "../hooks/use-courses";
import { toast } from "sonner";
import { Authorized, ActionType, ResourceType, ResourceId } from "@/features/auth";
import type { Course } from "../types";

interface CourseHeaderProps {
  course: Course;
}

export function CourseHeader({ course }: CourseHeaderProps) {
  const { t } = useTranslation(['courses', 'translation']);
  const patchCourse = usePatchCourse(course.id);

  const handleTitleUpdate = async (newTitle: string) => {
    try {
      await patchCourse.mutateAsync({ title: newTitle });
      toast.success(t('courses:detail.titleUpdated'));
    } catch (error) {
      toast.error(t('courses:detail.titleUpdateFailed'));
      throw error;
    }
  };

  return (
    <Card className="overflow-hidden">
      <div className="grid md:grid-cols-2 gap-6 p-6">
        {course.imageUrl && (
          <div className="relative h-64 md:h-full overflow-hidden rounded-lg">
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
        <div className="space-y-4">
          <div className="space-y-2">
            <div className="flex justify-between items-start gap-2">
              <Authorized
                action={ActionType.Update}
                resource={ResourceType.Course}
                resourceId={ResourceId.create(course.id)}
                fallback={
                  <h1 className="text-3xl font-bold" dir="auto">
                    {course.title}
                  </h1>
                }
              >
                <InlineEditableText
                  value={course.title}
                  onSave={handleTitleUpdate}
                  displayClassName="text-3xl font-bold"
                  inputClassName="text-3xl font-bold"
                  placeholder={t('courses:detail.enterTitle')}
                  maxLength={200}
                />
              </Authorized>
              <CourseActions courseId={course.id} />
            </div>
            <p className="text-muted-foreground">
              {t('courses:detail.instructor')}:{" "}
              <span dir="auto">
                {course.instructorUserId ?? t('common.unknown')}
              </span>
            </p>
          </div>
          <div className="flex gap-3">
            <motion.div
              className="flex-1"
              whileHover={{ scale: 1.02 }}
              whileTap={{ scale: 0.98 }}
            >
              <Button className="w-full gap-2">
                <CreditCard className="h-4 w-4" />
                {t('courses:detail.buyNow')}
              </Button>
            </motion.div>
            <motion.div
              className="flex-1"
              whileHover={{ scale: 1.02 }}
              whileTap={{ scale: 0.98 }}
            >
              <Button variant="outline" className="w-full gap-2">
                <ShoppingCart className="h-4 w-4" />
                {t('courses:detail.addToCart')}
              </Button>
            </motion.div>
          </div>
        </div>
      </div>
    </Card>
  );
}
