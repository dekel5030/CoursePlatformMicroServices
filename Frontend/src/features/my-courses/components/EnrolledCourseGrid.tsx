import { useTranslation } from "react-i18next";
import { EnrolledCourseCard } from "./EnrolledCourseCard";
import { Skeleton } from "@/shared/ui";
import { motion } from "framer-motion";
import type { EnrolledCourseDto } from "@/domain/enrollments";

interface EnrolledCourseGridProps {
  courses: EnrolledCourseDto[];
  isLoading: boolean;
  error: Error | null;
}

export function EnrolledCourseGrid({
  courses,
  isLoading,
  error,
}: EnrolledCourseGridProps) {
  const { t } = useTranslation("translation");

  const container = {
    hidden: { opacity: 0 },
    show: {
      opacity: 1,
      transition: {
        staggerChildren: 0.08,
      },
    },
  };

  const item = {
    hidden: { opacity: 0, y: 20 },
    show: { opacity: 1, y: 0, transition: { duration: 0.4 } },
  };

  if (isLoading) {
    return (
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {[1, 2, 3, 4, 5, 6].map((i) => (
          <Skeleton key={i} className="h-[220px] rounded-lg" />
        ))}
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-destructive/10 border border-destructive/20 text-destructive px-6 py-4 rounded-lg">
        {t("common.error", { message: error.message })}
      </div>
    );
  }

  if (courses.length === 0) {
    return (
      <div className="py-16 text-center">
        <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-muted mb-4">
          <svg
            className="w-8 h-8 text-muted-foreground"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253"
            />
          </svg>
        </div>
        <p className="text-lg font-medium text-muted-foreground">
          {t("myCourses.noEnrollments")}
        </p>
      </div>
    );
  }

  return (
    <motion.div
      className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6"
      variants={container}
      initial="hidden"
      animate="show"
    >
      {courses.map((course) => (
        <motion.div key={course.enrollmentId} variants={item}>
          <EnrolledCourseCard course={course} />
        </motion.div>
      ))}
    </motion.div>
  );
}
