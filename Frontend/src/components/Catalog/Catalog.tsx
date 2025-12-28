import { usePermissions } from "@/hooks/usePermissions";
import { hasPermission } from "@/utils/permissionEvaluation";
import { Button } from "@/components/ui/button";
import { Plus } from "lucide-react";
import { useTranslation } from "react-i18next";

export default function Catalog() {
  const permissions = usePermissions();
  const { t } = useTranslation();

  const canAddCourse = hasPermission(
    permissions,
    "create",
    "course",
    "*"
  );

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-bold tracking-tight">{t('navbar.catalog')}</h2>
        {canAddCourse && (
          <Button>
            <Plus className="mr-2 h-4 w-4" />
            Add Course
          </Button>
        )}
      </div>
      
      {/* Catalog content would go here */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div className="p-12 border border-dashed rounded-lg text-center text-muted-foreground">
          Courses will appear here
        </div>
      </div>
    </div>
  );
}
