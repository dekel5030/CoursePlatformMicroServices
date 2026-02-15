import { useRef } from "react";
import { useTranslation } from "react-i18next";
import { Button } from "@/shared/ui";
import { Upload, Loader2 } from "lucide-react";
import { toast } from "sonner";
import { useCourseImageUpload } from "../hooks/useCourseImageUpload";
import { getLinkFromRecord } from "@/shared/utils";
import type { LinksRecord } from "@/shared/types/LinkRecord";

interface CourseImageUploadProps {
  courseId: string;
  links?: LinksRecord;
}

const ALLOWED_IMAGE_TYPES = [
  "image/jpeg",
  "image/png",
  "image/gif",
  "image/webp",
];

export function CourseImageUpload({ courseId, links }: CourseImageUploadProps) {
  const { t } = useTranslation(["course-management", "translation"]);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const { uploadImage, isUploading, reset } = useCourseImageUpload(courseId);

  const uploadLink = getLinkFromRecord(links, "generateImageUploadUrl");
  if (!uploadLink?.href) return null;

  const handleButtonClick = () => {
    fileInputRef.current?.click();
  };

  const resetFileInput = () => {
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  const handleFileChange = async (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    const file = event.target.files?.[0];
    if (!file) return;

    // Validate file type
    if (!ALLOWED_IMAGE_TYPES.includes(file.type)) {
      toast.error(t("course-management:detail.invalidFileType"));
      resetFileInput();
      return;
    }

    try {
      reset();
      await uploadImage(file, uploadLink.href!);
      toast.success(t("course-management:detail.uploadSuccess"));
      resetFileInput();
    } catch (err) {
      toast.error(t("course-management:detail.uploadFailed"));
      console.error("Failed to upload image:", err);
      resetFileInput();
    }
  };

  return (
    <div>
      <input
        ref={fileInputRef}
        type="file"
        accept="image/jpeg,image/png,image/gif,image/webp"
        onChange={handleFileChange}
        className="hidden"
        disabled={isUploading}
      />
      <Button
        variant="outline"
        size="sm"
        className="gap-2"
        onClick={handleButtonClick}
        disabled={isUploading}
      >
        {isUploading ? (
          <>
            <Loader2 className="h-4 w-4 animate-spin" />
            {t("course-management:detail.uploading")}
          </>
        ) : (
          <>
            <Upload className="h-4 w-4" />
            {t("course-management:detail.uploadImage")}
          </>
        )}
      </Button>
    </div>
  );
}
