import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Button } from "@/shared/ui";
import { SlidersHorizontal, Plus } from "lucide-react";
import { AddCourseDialog } from "@/features/course-management";
import { hasLink, getLinkFromRecord } from "@/shared/utils";
import { CourseRels } from "@/domain/courses";
import type { LinkDto } from "@/shared/types";
import type { LinksRecord } from "@/shared/types/LinkRecord";

interface CatalogHeaderProps {
  /** Legacy array or strongly-typed collection links (create, next, prev) */
  collectionLinks?: LinkDto[] | LinksRecord;
}

export function CatalogHeader({ collectionLinks }: CatalogHeaderProps) {
  const { t } = useTranslation(["course-catalog", "translation"]);
  const [isAddCourseOpen, setIsAddCourseOpen] = useState(false);

  const canCreateCourse = Array.isArray(collectionLinks)
    ? hasLink(collectionLinks, CourseRels.CREATE)
    : !!getLinkFromRecord(collectionLinks, "create")?.href;

  return (
    <>
      <header className="flex flex-col md:flex-row items-start md:items-end justify-between gap-6 border-b border-border pb-8">
        <div className="space-y-2 max-w-2xl">
          <h1 className="text-4xl font-bold tracking-tight text-foreground">
            {t("navbar.catalog")}
          </h1>
          <p className="text-lg text-muted-foreground">
            {t('course-catalog:catalog.subtitle')}
          </p>
        </div>

        <div className="flex items-center gap-2">
          <Button
            variant="outline"
            size="sm"
            className="hidden md:flex gap-2"
          >
            <SlidersHorizontal className="h-4 w-4" />
            {t('course-catalog:catalog.filters')}
          </Button>

          {canCreateCourse && (
            <Button 
              size="sm" 
              className="gap-2"
              onClick={() => setIsAddCourseOpen(true)}
            >
              <Plus className="h-4 w-4" />
              {t('course-catalog:catalog.addCourse')}
            </Button>
          )}
        </div>
      </header>

      <AddCourseDialog 
        open={isAddCourseOpen} 
        onOpenChange={setIsAddCourseOpen} 
      />
    </>
  );
}
