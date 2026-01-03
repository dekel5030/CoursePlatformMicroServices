import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import { Edit, Trash2 } from "lucide-react";
import { Authorized, ActionType, ResourceType, ResourceId } from "@/features/auth";
import { toast } from "sonner";

interface CourseActionsProps {
  courseId: string;
}

export function CourseActions({ courseId }: CourseActionsProps) {
  const { t } = useTranslation(['courses', 'translation']);

  const handleEdit = () => {
    toast.info(t('courses:actions.editNotImplemented'));
  };

  const handleDelete = () => {
    toast.info(t('courses:actions.deleteNotImplemented'));
  };

  return (
    <div className="flex gap-2">
      <Authorized 
        action={ActionType.Update} 
        resource={ResourceType.Course}
        resourceId={ResourceId.create(courseId)}
      >
        <Button 
          variant="outline" 
          size="sm" 
          className="gap-2"
          onClick={handleEdit}
        >
          <Edit className="h-4 w-4" />
          {t('common.edit')}
        </Button>
      </Authorized>
      <Authorized 
        action={ActionType.Delete} 
        resource={ResourceType.Course}
        resourceId={ResourceId.create(courseId)}
      >
        <Button 
          variant="destructive" 
          size="sm" 
          className="gap-2"
          onClick={handleDelete}
        >
          <Trash2 className="h-4 w-4" />
          {t('courses:detail.delete')}
        </Button>
      </Authorized>
    </div>
  );
}
