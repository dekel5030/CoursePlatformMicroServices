import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useManagedCourses } from "@/domain/courses";
import { BreadcrumbNav } from "@/components/layout";
import { ManagedCourseGrid } from "../components/ManagedCourseGrid";
import { PageNumberPagination } from "@/features/my-courses/components/PageNumberPagination";

export default function ManagedCoursesPage() {
  const { t } = useTranslation("translation");
  const [pageNumber, setPageNumber] = useState(1);
  const pageSize = 10;

  const { data, isLoading, error } = useManagedCourses(pageNumber, pageSize);

  const courses = data?.items ?? [];
  const totalPages =
    data?.totalPages ?? (Math.ceil((data?.totalItems ?? 0) / pageSize) || 1);

  const handlePageChange = (page: number) => {
    setPageNumber(page);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const breadcrumbItems = [
    { label: t("breadcrumbs.home"), path: "/" },
    { label: t("managedCourses.title") },
  ];

  return (
    <div className="flex flex-col">
      <BreadcrumbNav items={breadcrumbItems} />
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12 space-y-12 w-full">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">
            {t("managedCourses.title")}
          </h1>
          <p className="text-muted-foreground mt-1">
            {t("managedCourses.subtitle")}
          </p>
        </div>

        <main>
          <ManagedCourseGrid
            courses={courses}
            isLoading={isLoading}
            error={error}
          />
          <PageNumberPagination
            pageNumber={pageNumber}
            totalPages={totalPages}
            totalItems={data?.totalItems ?? 0}
            onPageChange={handlePageChange}
            isLoading={isLoading}
          />
        </main>
      </div>
    </div>
  );
}
