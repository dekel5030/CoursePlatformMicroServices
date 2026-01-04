import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import ConfirmationModal from "@/components/ui/ConfirmationModal";
import { Edit, Trash2 } from "lucide-react";
import { Authorized, ActionType, ResourceType, ResourceId } from "@/features/auth";
import { useDeleteCourse } from "../hooks/use-courses";
import { toast } from "sonner";

interface CourseActionsProps {
  courseId: string;
}

export function CourseActions({ courseId }: CourseActionsProps) {
  const { t } = useTranslation(['courses', 'translation']);
  const navigate = useNavigate();
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const deleteCourse = useDeleteCourse();

  const handleEdit = () => {
    toast.info(t('courses:actions.editNotImplemented'));
  };

  const handleDeleteClick = () => {
    setIsDeleteDialogOpen(true);
  };

  const handleConfirmDelete = async () => {
    try {
      await deleteCourse.mutateAsync(courseId);
      toast.success(t('courses:actions.deleteSuccess'));
      // Navigate back to all courses page after successful deletion
      navigate('/courses');
    } catch (error) {
      toast.error(t('courses:actions.deleteFailed'));
      console.error('Failed to delete course:', error);
    } finally {
      setIsDeleteDialogOpen(false);
    }
  };

  return (
    <>
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
            onClick={handleDeleteClick}
            disabled={deleteCourse.isPending}
          >
            <Trash2 className="h-4 w-4" />
            {t('courses:detail.delete')}
          </Button>
        </Authorized>
      </div>

      <ConfirmationModal
        open={isDeleteDialogOpen}
        onOpenChange={setIsDeleteDialogOpen}
        onConfirm={handleConfirmDelete}
        title={t('courses:actions.deleteConfirmTitle')}
        message={t('courses:actions.deleteConfirmMessage')}
        confirmText={t('common.delete')}
        cancelText={t('common.cancel')}
        isLoading={deleteCourse.isPending}
      />
    </>
  );
}
