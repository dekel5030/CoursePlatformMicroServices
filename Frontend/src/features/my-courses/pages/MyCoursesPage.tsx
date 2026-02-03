import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useMyEnrollments } from "@/domain/enrollments";
import { BreadcrumbNav } from "@/components/layout";
import { EnrolledCourseGrid } from "../components/EnrolledCourseGrid";
import { PageNumberPagination } from "../components/PageNumberPagination";

export default function MyCoursesPage() {
  const { t } = useTranslation("translation");
  const [pageNumber, setPageNumber] = useState(1);
  const pageSize = 10;

  const { data, isLoading, error } = useMyEnrollments(pageNumber, pageSize);

  const courses = data?.items ?? [];
  const totalPages =
    data?.totalPages ?? (Math.ceil((data?.totalItems ?? 0) / pageSize) || 1);

  const handlePageChange = (page: number) => {
    setPageNumber(page);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const breadcrumbItems = [
    { label: t("breadcrumbs.home"), path: "/" },
    { label: t("breadcrumbs.myCourses") },
  ];

  return (
    <div className="flex flex-col">
      <BreadcrumbNav items={breadcrumbItems} />
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12 space-y-12 w-full">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">
            {t("myCourses.title")}
          </h1>
          <p className="text-muted-foreground mt-1">
            {t("myCourses.subtitle")}
          </p>
        </div>

        <main>
          <EnrolledCourseGrid
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
