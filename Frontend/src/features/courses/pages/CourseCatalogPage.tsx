import { useMemo, useState } from "react";
import { CourseCard, useCourses } from "@/features/courses";
import { Button, Skeleton, BreadcrumbNav } from "@/components";
import { motion } from "framer-motion";
import { useTranslation } from "react-i18next";
import { SlidersHorizontal, Plus } from "lucide-react";

export default function HomePage() {
  const [selectedCategory, setSelectedCategory] = useState<string | null>(null);
  const { t } = useTranslation();
  const { data: courses = [], isLoading, error } = useCourses();

  const canAddCourse = true; //hasPermission(permissions, "Create", "Course", "*");

  const categories = useMemo(() => {
    const set = new Set<string>();
    courses.forEach((c) => c.title && set.add(c.title));
    return Array.from(set).sort();
  }, [courses]);

  const filtered = selectedCategory
    ? courses.filter((c) => c.title === selectedCategory)
    : courses;

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

  const breadcrumbItems = [
    { label: t("breadcrumbs.home"), path: "/" },
    { label: t("breadcrumbs.courses") },
  ];

  return (
    <div className="flex flex-col">
      <BreadcrumbNav items={breadcrumbItems} />
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12 space-y-12 w-full">
        {/* Header */}
        <header className="flex flex-col md:flex-row items-start md:items-end justify-between gap-6 border-b border-border pb-8">
          <div className="space-y-2 max-w-2xl">
            <h1 className="text-4xl font-bold tracking-tight text-foreground">
              {t("navbar.catalog")}
            </h1>
            <p className="text-lg text-muted-foreground">
              Explore our curated collection of architectural and engineering
              courses. From BIM mastery to computational design.
            </p>
          </div>
          <div className="flex items-center gap-2">
            <Button
              variant="outline"
              size="sm"
              className="hidden md:flex gap-2"
            >
              <SlidersHorizontal className="h-4 w-4" />
              Filters
            </Button>
            {canAddCourse && (
              <Button size="sm" className="gap-2">
                <Plus className="h-4 w-4" />
                Add Course
              </Button>
            )}
          </div>
        </header>

        <div className="space-y-8">
          {categories.length > 0 && (
            <div
              className="flex overflow-x-auto pb-4 gap-2 scrollbar-none"
              aria-label="Categories"
            >
              <Button
                variant={!selectedCategory ? "default" : "secondary"}
                size="sm"
                onClick={() => setSelectedCategory(null)}
                className="rounded-full px-6 transition-all"
              >
                All
              </Button>
              {categories.map((cat) => (
                <Button
                  key={cat}
                  variant={selectedCategory === cat ? "default" : "secondary"}
                  size="sm"
                  onClick={() => setSelectedCategory(cat)}
                  className="rounded-full px-6 whitespace-nowrap transition-all"
                >
                  {cat}
                </Button>
              ))}
            </div>
          )}

          <main>
            {isLoading && (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-8">
                {[1, 2, 3, 4].map((i) => (
                  <Skeleton key={i} className="h-[340px] rounded-md" />
                ))}
              </div>
            )}

            {error && (
              <div className="bg-destructive/10 border border-destructive/20 text-destructive px-6 py-4 rounded-md">
                Error loading courses: {error.message}
              </div>
            )}

            {!isLoading &&
              !error &&
              (filtered.length === 0 ? (
                <div className="py-12 text-center text-muted-foreground">
                  No courses found in this category.
                </div>
              ) : (
                <motion.div
                  className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-8"
                  variants={container}
                  initial="hidden"
                  animate="show"
                >
                  {filtered.map((course) => (
                    <motion.div key={course.id} variants={item}>
                      <CourseCard course={course} />
                    </motion.div>
                  ))}
                </motion.div>
              ))}
          </main>
        </div>
      </div>
    </div>
  );
}
