import { useState } from "react";
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useMyEnrollments } from "@/domain/enrollments";
import { BreadcrumbNav } from "@/components/layout";
import { EnrolledCourseGrid } from "../components/EnrolledCourseGrid";
import { PageNumberPagination } from "../components/PageNumberPagination";
import { Button } from "@/shared/ui";
import { getLinkFromRecord, linkDtoArrayToRecord, apiHrefToAppRoute } from "@/shared/utils";
import type { LinksRecord } from "@/shared/types/LinkRecord";
import { BookOpen } from "lucide-react";

function getCollectionLinks(links: unknown): LinksRecord | undefined {
  if (!links || typeof links !== "object") return undefined;
  if (Array.isArray(links)) return linkDtoArrayToRecord(links);
  return links as LinksRecord;
}

export default function MyCoursesPage() {
  const { t } = useTranslation("translation");
  const [pageNumber, setPageNumber] = useState(1);
  const pageSize = 10;

  const { data, isLoading, error } = useMyEnrollments(pageNumber, pageSize);

  const courses = data?.items ?? [];
  const totalPages =
    data?.totalPages ?? (Math.ceil((data?.totalItems ?? 0) / pageSize) || 1);
  const collectionLinks = getCollectionLinks(data?.links);
  const browseCatalogLink = collectionLinks
    ? getLinkFromRecord(collectionLinks, "browseCatalog")
    : undefined;
  const browseCatalogRoute = browseCatalogLink?.href
    ? apiHrefToAppRoute(browseCatalogLink.href)
    : null;

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
        <div className="flex flex-wrap items-start justify-between gap-4">
          <div>
            <h1 className="text-2xl font-bold tracking-tight">
              {t("myCourses.title")}
            </h1>
            <p className="text-muted-foreground mt-1">
              {t("myCourses.subtitle")}
            </p>
          </div>
          {browseCatalogRoute && (
            <Button variant="outline" size="sm" className="gap-2 shrink-0" asChild>
              <Link to={browseCatalogRoute}>
                <BookOpen className="h-4 w-4" />
                {t("myCourses.browseCatalog", { defaultValue: "Browse catalog" })}
              </Link>
            </Button>
          )}
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
