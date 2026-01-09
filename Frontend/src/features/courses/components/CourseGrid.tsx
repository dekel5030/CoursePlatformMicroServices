import { useTranslation } from "react-i18next";
import { CourseCard } from "@/features/courses";
import { Skeleton } from "@/components/ui/skeleton";
import { motion } from "framer-motion";
import type { CourseModel } from "../types";

interface CourseGridProps {
  courses: CourseModel[];
  isLoading: boolean;
  error: Error | null;
}

export function CourseGrid({ courses, isLoading, error }: CourseGridProps) {
  const { t } = useTranslation(['courses', 'translation']);

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

  if (isLoading) {
    return (
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-8">
        {[1, 2, 3, 4].map((i) => (
          <Skeleton key={i} className="h-[340px] rounded-md" />
        ))}
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-destructive/10 border border-destructive/20 text-destructive px-6 py-4 rounded-md">
        {t('courses:catalog.errorLoading', { message: error.message })}
      </div>
    );
  }

  if (courses.length === 0) {
    return (
      <div className="py-12 text-center text-muted-foreground">
        {t('courses:catalog.noCourses')}
      </div>
    );
  }

  return (
    <motion.div
      className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-8"
      variants={container}
      initial="hidden"
      animate="show"
    >
      {courses.map((course) => (
        <motion.div key={course.id} variants={item}>
          <CourseCard course={course} />
        </motion.div>
      ))}
    </motion.div>
  );
}
