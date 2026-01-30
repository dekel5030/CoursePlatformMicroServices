import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { Button } from "@/shared/ui";
import ConfirmationModal from "@/components/ui/ConfirmationModal";
import { Edit, Trash2 } from "lucide-react";
import { useDeleteCourse } from "@/domain/courses";
import { toast } from "sonner";
import { hasLink, getLink } from "@/shared/utils";
import { CourseRels } from "@/domain/courses";
import type { LinkDto } from "@/shared/types";

interface CourseActionsProps {
  courseId: string;
  links?: LinkDto[];
}

export function CourseActions({ links }: CourseActionsProps) {
  const { t } = useTranslation(['courses', 'translation']);
  const navigate = useNavigate();
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const deleteCourse = useDeleteCourse();
  
  // Determine available actions based on HATEOAS links
  const canUpdate = hasLink(links, CourseRels.PARTIAL_UPDATE);
  const canDelete = hasLink(links, CourseRels.DELETE);
  const deleteLink = getLink(links, CourseRels.DELETE);

  const handleEdit = () => {
    toast.info(t('courses:actions.editNotImplemented'));
  };

  const handleDeleteClick = () => {
    setIsDeleteDialogOpen(true);
  };

  const handleConfirmDelete = async () => {
    if (!deleteLink) {
      console.error("No delete link found for this course");
      return;
    }
    
    try {
      await deleteCourse.mutateAsync(deleteLink.href);
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

  // If no actions available, don't render anything
  if (!canUpdate && !canDelete) {
    return null;
  }

  return (
    <>
      <div className="flex gap-2">
        {canUpdate && (
          <Button 
            variant="outline" 
            size="sm" 
            className="gap-2"
            onClick={handleEdit}
          >
            <Edit className="h-4 w-4" />
            {t('common.edit')}
          </Button>
        )}
        {canDelete && (
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
        )}
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
