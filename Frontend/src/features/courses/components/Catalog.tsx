import { Button } from "@/components";
import { Plus } from "lucide-react";
import { useTranslation } from "react-i18next";

export default function Catalog() {
  const { t } = useTranslation(['courses', 'translation']);

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-bold tracking-tight">
          {t("navbar.catalog")}
        </h2>
        <Button>
          <Plus className="mr-2 h-4 w-4" />
          {t('courses:catalog.addCourse')}
        </Button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div className="p-12 border border-dashed rounded-lg text-center text-muted-foreground">
          {t('courses:catalog.noCourses')}
        </div>
      </div>
    </div>
  );
}
