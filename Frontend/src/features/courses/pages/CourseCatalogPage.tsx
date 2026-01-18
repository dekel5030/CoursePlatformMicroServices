import { useTranslation } from "react-i18next";
import { useFeaturedCourses } from "@/features/courses";
import { PaginatedCourseList } from "../components/PaginatedCourseList";

export default function CourseCatalogPage() {
  const { t } = useTranslation(["courses", "translation"]);
  const { data: courses = [], isLoading, error } = useFeaturedCourses();

  const breadcrumbItems = [
    { label: t("breadcrumbs.home"), path: "/" },
    { label: t("breadcrumbs.courses") },
  ];

  return (
    <PaginatedCourseList
      courses={courses}
      isLoading={isLoading}
      error={error}
      breadcrumbItems={breadcrumbItems}
      showBreadcrumbs={true}
      showHeader={true}
      showCategoryFilter={true}
    />
  );
}
