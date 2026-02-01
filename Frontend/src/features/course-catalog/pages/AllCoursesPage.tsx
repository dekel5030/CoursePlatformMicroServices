import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useAllCourses } from "@/domain/courses";
import { BreadcrumbNav } from "@/components/layout";
import { CatalogHeader } from "../components/CatalogHeader";
import { CourseGrid } from "../components/CourseGrid";
import { Pagination } from "../components/Pagination";

export default function AllCoursesPage() {
  const { t } = useTranslation(["course-catalog", "translation"]);
  const [currentUrl, setCurrentUrl] = useState<string | undefined>(undefined);
  const { data, isLoading, error } = useAllCourses(currentUrl);
  
  const courses = data?.courses || [];
  const links = data?.links || [];

  const handleNavigate = (url: string) => {
    setCurrentUrl(url);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

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
          <Pagination
            links={links}
            onNavigate={handleNavigate}
            isLoading={isLoading}
          />
        </main>
      </div>
    </div>
  );
}
