import { useParams } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useCourse, usePatchCourse } from "@/domain/courses";
import { CourseDetailContent } from "../components/CourseDetailContent";
import { toast } from "sonner";
import { getLinkFromRecord } from "@/shared/utils";

export default function CoursePage() {
  const { id } = useParams<{ id: string }>();
  const { data: course, isLoading, error } = useCourse(id);
  const patchCourse = usePatchCourse(id!);
  const { t } = useTranslation(["course-management", "translation"]);

  const handleDescriptionUpdate = async (newDescription: string) => {
    const updateLink = getLinkFromRecord(course?.links, "partialUpdate");
    if (!updateLink?.href) {
      console.error("No update link found for this course");
      return;
    }

    try {
      await patchCourse.mutateAsync({
        url: updateLink.href,
        request: { description: newDescription },
      });
      toast.success(t("course-management:detail.descriptionUpdated"));
    } catch (err) {
      toast.error(t("course-management:detail.descriptionUpdateFailed"));
      throw err;
    }
  };

  const breadcrumbItems = [
    { label: t("breadcrumbs.home"), path: "/" },
    { label: t("breadcrumbs.courses"), path: "/catalog" },
    { label: course?.title ?? "" },
  ];

  return (
    <CourseDetailContent
      course={course}
      isLoading={isLoading}
      error={error}
      onDescriptionUpdate={handleDescriptionUpdate}
      breadcrumbItems={breadcrumbItems}
    />
  );
}
