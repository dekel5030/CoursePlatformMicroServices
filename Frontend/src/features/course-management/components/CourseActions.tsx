import { useState, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { LinkButtons } from "@/shared/components";
import ConfirmationModal from "@/components/ui/ConfirmationModal";
import { useDeleteCourse } from "@/domain/courses";
import { toast } from "sonner";
import { getLinkFromRecord } from "@/shared/utils";
import type { LinksRecord } from "@/shared/types/LinkRecord";

interface CourseActionsProps {
  courseId: string;
  links?: LinksRecord;
}

export function CourseActions({ links }: CourseActionsProps) {
  const { t } = useTranslation(["course-management", "translation"]);
  const navigate = useNavigate();
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const deleteCourse = useDeleteCourse();

  const deleteLink = getLinkFromRecord(links, "delete");

  const handleAction = useCallback(
    async (rel: string, _link: { href?: string | null; method?: string | null }) => {
      if (rel === "delete") {
        setIsDeleteDialogOpen(true);
        return;
      }
      if (rel === "partialUpdate") {
        toast.info(t("course-management:actions.editNotImplemented"));
      }
    },
    [t]
  );

  const handleConfirmDelete = async () => {
    if (!deleteLink?.href) {
      console.error("No delete link found for this course");
      return;
    }
    try {
      await deleteCourse.mutateAsync(deleteLink.href);
      toast.success(t("course-management:actions.deleteSuccess"));
      navigate("/courses");
    } catch (error) {
      toast.error(t("course-management:actions.deleteFailed"));
      console.error("Failed to delete course:", error);
    } finally {
      setIsDeleteDialogOpen(false);
    }
  };

  const hasAnyAction = links && (links.partialUpdate?.href || links.delete?.href);
  if (!hasAnyAction) return null;

  return (
    <>
      <LinkButtons
        links={links}
        labelByRel={{
          partialUpdate: t("common.edit"),
          delete: t("course-management:detail.delete"),
        }}
        onAction={handleAction}
        excludeRels={["self", "coursePage", "analytics", "publish", "generateImageUploadUrl", "createModule", "changePosition"]}
        variant="outline"
        size="sm"
      />

      <ConfirmationModal
        open={isDeleteDialogOpen}
        onOpenChange={setIsDeleteDialogOpen}
        onConfirm={handleConfirmDelete}
        title={t("course-management:actions.deleteConfirmTitle")}
        message={t("course-management:actions.deleteConfirmMessage")}
        confirmText={t("common.delete")}
        cancelText={t("common.cancel")}
        isLoading={deleteCourse.isPending}
      />
    </>
  );
}
