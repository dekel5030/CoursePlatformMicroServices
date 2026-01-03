import { useMemo, useState } from "react";
import { useTranslation } from "react-i18next";
import { useCourses } from "@/features/courses";
import { BreadcrumbNav } from "@/components";
import { CatalogHeader } from "../components/CatalogHeader";
import { CategoryFilter } from "../components/CategoryFilter";
import { CourseGrid } from "../components/CourseGrid";

export default function HomePage() {
  const [selectedCategory, setSelectedCategory] = useState<string | null>(null);
  const { t } = useTranslation(['courses', 'translation']);
  const { data: courses = [], isLoading, error } = useCourses();

  const categories = useMemo(() => {
    const set = new Set<string>();
    courses.forEach((c) => c.title && set.add(c.title));
    return Array.from(set).sort();
  }, [courses]);

  const filtered = selectedCategory
    ? courses.filter((c) => c.title === selectedCategory)
    : courses;

  const breadcrumbItems = [
    { label: t("breadcrumbs.home"), path: "/" },
    { label: t("breadcrumbs.courses") },
  ];

  return (
    <div className="flex flex-col">
      <BreadcrumbNav items={breadcrumbItems} />
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12 space-y-12 w-full">
        <CatalogHeader />

        <div className="space-y-8">
          <CategoryFilter 
            categories={categories}
            selectedCategory={selectedCategory}
            onSelectCategory={setSelectedCategory}
          />

          <main>
            <CourseGrid courses={filtered} isLoading={isLoading} error={error} />
          </main>
        </div>
      </div>
    </div>
  );
}
