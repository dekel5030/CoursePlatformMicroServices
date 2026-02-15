import { useState, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { LinkButtons } from "@/shared/components";
import ConfirmationModal from "@/components/ui/ConfirmationModal";
import { useDeleteCourse, usePublishCourse } from "@/domain/courses";
import { toast } from "sonner";
import { getLinkFromRecord, apiHrefToAppRoute, LINK_LABELS } from "@/shared/utils";
import type { LinksRecord } from "@/shared/types/LinkRecord";

interface CourseActionsProps {
  courseId: string;
  links?: LinksRecord;
}

export function CourseActions({ courseId, links }: CourseActionsProps) {
  const { t } = useTranslation(["course-management", "translation"]);
  const navigate = useNavigate();
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const deleteCourse = useDeleteCourse();
  const publishCourse = usePublishCourse(courseId);

  const deleteLink = getLinkFromRecord(links, "delete");

  const handleAction = useCallback(
    async (rel: string, link: { href?: string | null; method?: string | null }) => {
      if (rel === "delete") {
        setIsDeleteDialogOpen(true);
        return;
      }
      if (rel === "publish" && link?.href) {
        try {
          await publishCourse.mutateAsync(link.href);
        } catch {
          // toast handled in hook
        }
        return;
      }
      if (rel === "partialUpdate") {
        toast.info(t("course-management:actions.editNotImplemented"));
      }
    },
    [t, publishCourse]
  );

  const handleConfirmDelete = async () => {
    if (!deleteLink?.href) return;
    try {
      await deleteCourse.mutateAsync(deleteLink.href);
      toast.success(t("course-management:actions.deleteSuccess"));
      navigate("/manage/courses");
    } catch (error) {
      toast.error(t("course-management:actions.deleteFailed"));
    } finally {
      setIsDeleteDialogOpen(false);
    }
  };

  const labelByRel: Record<string, string> = {
    coursePage: t("course-management:detail.viewCourse", { defaultValue: LINK_LABELS.coursePage }),
    analytics: t("course-management:detail.analytics", { defaultValue: LINK_LABELS.analytics }),
    partialUpdate: t("common.edit"),
    delete: t("course-management:detail.delete"),
    publish: t("course-management:detail.publish", { defaultValue: LINK_LABELS.publish }),
    generateImageUploadUrl: t("course-management:detail.uploadImage", { defaultValue: LINK_LABELS.generateImageUploadUrl }),
    manage: t("course-management:detail.manageCourse", { defaultValue: LINK_LABELS.manage }),
    ratings: t("course-management:ratings.sectionTitle", { defaultValue: LINK_LABELS.ratings }),
  };

  const hasAnyLink = links && Object.values(links).some((l) => l?.href);
  if (!hasAnyLink) return null;

  return (
    <>
      <LinkButtons
        links={links}
        labelByRel={labelByRel}
        onAction={handleAction}
        excludeRels={["self", "createModule", "changePosition"]}
        getRouteForHref={apiHrefToAppRoute}
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
