import { useTranslation } from "react-i18next";
import { useAllCourses } from "@/features/courses";
import { BreadcrumbNav } from "@/components";
import { CatalogHeader } from "../components/CatalogHeader";
import { CourseGrid } from "../components/CourseGrid";

export default function AllCoursesPage() {
  const { t } = useTranslation(["courses", "translation"]);
  const { data, isLoading, error } = useAllCourses();
  
  const courses = data?.courses || [];
  const links = data?.links || [];

  const breadcrumbItems = [
    { label: t("breadcrumbs.home"), path: "/" },
    { label: t("breadcrumbs.allCourses") },
  ];

  return (
    <div className="flex flex-col">
      <BreadcrumbNav items={breadcrumbItems} />
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12 space-y-12 w-full">
        <CatalogHeader collectionLinks={links} />

        <main>
          <CourseGrid
            courses={courses}
            isLoading={isLoading}
            error={error}
          />
        </main>
      </div>
    </div>
  );
}
