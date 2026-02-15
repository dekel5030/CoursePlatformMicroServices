import { useState, useMemo } from "react";
import { useTranslation } from "react-i18next";
import { BreadcrumbNav } from "@/components/layout";
import { CatalogHeader } from "./CatalogHeader";
import { CategoryFilter } from "./CategoryFilter";
import { CourseGrid } from "./CourseGrid";
import { Pagination } from "./Pagination";
import type { CourseModel } from "@/domain/courses";
import type { LinkDto } from "@/shared/types";
import type { LinksRecord } from "@/shared/types/LinkRecord";

interface PaginatedCourseListProps {
  courses: CourseModel[];
  isLoading: boolean;
  error: Error | null;
  /** Collection links (legacy array or strongly-typed record with create, next, prev) */
  links?: LinkDto[] | LinksRecord;
  onNavigate?: (url: string) => void;
  showBreadcrumbs?: boolean;
  showHeader?: boolean;
  showCategoryFilter?: boolean;
  breadcrumbItems?: Array<{ label: string; path?: string }>;
}

/**
 * Generic paginated course list component that can be reused across the app
 * Supports optional breadcrumbs, header, and category filtering
 */
export function PaginatedCourseList({
  courses,
  isLoading,
  error,
  links,
  onNavigate,
  showBreadcrumbs = true,
  showHeader = true,
  showCategoryFilter = true,
  breadcrumbItems,
}: PaginatedCourseListProps) {
  const [selectedCategory, setSelectedCategory] = useState<string | null>(null);
  const { t } = useTranslation(["course-catalog", "translation"]);

  const categories = useMemo(() => {
    const set = new Set<string>();
    courses.forEach((c) => c.title && set.add(c.title));
    return Array.from(set).sort();
  }, [courses]);

  const filtered = selectedCategory
    ? courses.filter((c) => c.title === selectedCategory)
    : courses;

  const defaultBreadcrumbItems = [
    { label: t("breadcrumbs.home"), path: "/" },
    { label: t("breadcrumbs.courses") },
  ];

  return (
    <div className="flex flex-col">
      {showBreadcrumbs && (
        <BreadcrumbNav items={breadcrumbItems || defaultBreadcrumbItems} />
      )}
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12 space-y-12 w-full">
        {showHeader && <CatalogHeader collectionLinks={links as LinkDto[] | LinksRecord | undefined} />}

        <div className="space-y-8">
          {showCategoryFilter && categories.length > 0 && (
            <CategoryFilter
              categories={categories}
              selectedCategory={selectedCategory}
              onSelectCategory={setSelectedCategory}
            />
          )}

          <main>
            <CourseGrid
              courses={filtered}
              isLoading={isLoading}
              error={error}
            />
            {onNavigate && links && (
              <Pagination
                links={Array.isArray(links) ? links : undefined}
                collectionLinks={
                  !Array.isArray(links) && links ? links : undefined
                }
                onNavigate={onNavigate}
                isLoading={isLoading}
              />
            )}
          </main>
        </div>
      </div>
    </div>
  );
}
