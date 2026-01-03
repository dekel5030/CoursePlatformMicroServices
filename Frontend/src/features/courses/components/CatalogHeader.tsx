import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import { SlidersHorizontal, Plus } from "lucide-react";
import { Authorized, ActionType, ResourceType, ResourceId } from "@/features/auth";
import { AddCourseDialog } from "./AddCourseDialog";

export function CatalogHeader() {
  const { t } = useTranslation(['courses', 'translation']);
  const [isAddCourseOpen, setIsAddCourseOpen] = useState(false);

  return (
    <>
      <header className="flex flex-col md:flex-row items-start md:items-end justify-between gap-6 border-b border-border pb-8">
        <div className="space-y-2 max-w-2xl">
          <h1 className="text-4xl font-bold tracking-tight text-foreground">
            {t("navbar.catalog")}
          </h1>
          <p className="text-lg text-muted-foreground">
            {t('courses:catalog.subtitle')}
          </p>
        </div>

        <div className="flex items-center gap-2">
          <Button
            variant="outline"
            size="sm"
            className="hidden md:flex gap-2"
          >
            <SlidersHorizontal className="h-4 w-4" />
            {t('courses:catalog.filters')}
          </Button>

          <Authorized 
            action={ActionType.Create} 
            resource={ResourceType.Course}
            resourceId={ResourceId.Wildcard}
          >
            <Button 
              size="sm" 
              className="gap-2"
              onClick={() => setIsAddCourseOpen(true)}
            >
              <Plus className="h-4 w-4" />
              {t('courses:catalog.addCourse')}
            </Button>
          </Authorized>
        </div>
      </header>

      <AddCourseDialog 
        open={isAddCourseOpen} 
        onOpenChange={setIsAddCourseOpen} 
      />
    </>
  );
}
