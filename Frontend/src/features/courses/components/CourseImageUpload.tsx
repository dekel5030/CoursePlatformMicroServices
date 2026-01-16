import { useRef } from "react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import { Upload, Loader2 } from "lucide-react";
import { toast } from "sonner";
import { useCourseImageUpload } from "../hooks/useCourseImageUpload";
import { getLink, CourseRels } from "@/utils/linkHelpers";
import type { LinkDto } from "@/types/LinkDto";

interface CourseImageUploadProps {
  courseId: string;
  links?: LinkDto[];
}

const ALLOWED_IMAGE_TYPES = ["image/jpeg", "image/png", "image/gif", "image/webp"];

export function CourseImageUpload({ courseId, links }: CourseImageUploadProps) {
  const { t } = useTranslation(["courses"]);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const { uploadImage, isUploading, reset } = useCourseImageUpload(courseId);

  // Check if user has permission to upload image
  const uploadLink = getLink(links, CourseRels.GENERATE_IMAGE_UPLOAD_URL);

  // If no permission, don't render the component
  if (!uploadLink) {
    return null;
  }

  const handleButtonClick = () => {
    fileInputRef.current?.click();
  };

  const handleFileChange = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    // Validate file type
    if (!ALLOWED_IMAGE_TYPES.includes(file.type)) {
      toast.error(t("courses:detail.invalidFileType"));
      // Reset the input
      if (fileInputRef.current) {
        fileInputRef.current.value = "";
      }
      return;
    }

    try {
      reset();
      await uploadImage(file, uploadLink.href);
      toast.success(t("courses:detail.uploadSuccess"));
      // Reset the input
      if (fileInputRef.current) {
        fileInputRef.current.value = "";
      }
    } catch (err) {
      toast.error(t("courses:detail.uploadFailed"));
      console.error("Failed to upload image:", err);
      // Reset the input
      if (fileInputRef.current) {
        fileInputRef.current.value = "";
      }
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
            {t("courses:detail.uploading")}
          </>
        ) : (
          <>
            <Upload className="h-4 w-4" />
            {t("courses:detail.uploadThumbnail")}
          </>
        )}
      </Button>
    </div>
  );
}
