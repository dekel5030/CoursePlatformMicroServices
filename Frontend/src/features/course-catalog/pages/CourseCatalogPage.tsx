import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useAllCourses } from "@/domain/courses";
import { PaginatedCourseList } from "../components/PaginatedCourseList";

export default function CourseCatalogPage() {
  const { t } = useTranslation(["course-catalog", "translation"]);
  const [currentUrl, setCurrentUrl] = useState<string | undefined>(undefined);
  const { data, isLoading, error } = useAllCourses(currentUrl);

  const courses = data?.courses ?? [];
  const links = data?.links;

  const handleNavigate = (url: string) => {
    setCurrentUrl(url);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const breadcrumbItems = [
    { label: t("breadcrumbs.home"), path: "/" },
    { label: t("breadcrumbs.courses") },
  ];

  return (
    <PaginatedCourseList
      courses={courses}
      isLoading={isLoading}
      error={error}
      links={links}
      onNavigate={handleNavigate}
      breadcrumbItems={breadcrumbItems}
      showBreadcrumbs={true}
      showHeader={true}
      showCategoryFilter={true}
    />
  );
}
