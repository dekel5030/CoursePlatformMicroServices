import { useRef } from "react";
import { useTranslation } from "react-i18next";
import { Button } from "@/components/ui/button";
import { Upload, Loader2 } from "lucide-react";
import { toast } from "sonner";
import { useLessonVideoUpload } from "../hooks/useLessonVideoUpload";
import { getLink, LessonRels } from "@/utils/linkHelpers";
import type { LinkDto } from "@/types/LinkDto";

interface LessonVideoUploadProps {
  courseId: string;
  lessonId: string;
  links?: LinkDto[];
}

const ALLOWED_VIDEO_TYPES = [
  "video/mp4",
  "video/mpeg",
  "video/quicktime",
  "video/x-msvideo",
  "video/webm",
];

export function LessonVideoUpload({ courseId, lessonId, links }: LessonVideoUploadProps) {
  const { t } = useTranslation(["lessons"]);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const { uploadVideo, isUploading, reset } = useLessonVideoUpload(courseId, lessonId);

  // Check if user has permission to upload video
  const uploadLink = getLink(links, LessonRels.GENERATE_VIDEO_UPLOAD_URL);

  // If no permission, don't render the component
  if (!uploadLink) {
    return null;
  }

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
    if (!ALLOWED_VIDEO_TYPES.includes(file.type)) {
      toast.error(t("lessons:detail.invalidVideoType"));
      resetFileInput();
      return;
    }

    try {
      reset();
      await uploadVideo(file, uploadLink.href);
      toast.success(t("lessons:detail.videoUploadSuccess"));
      resetFileInput();
    } catch (err) {
      toast.error(t("lessons:detail.videoUploadFailed"));
      console.error("Failed to upload video:", err);
      resetFileInput();
    }
  };

  return (
    <div>
      <input
        ref={fileInputRef}
        type="file"
        accept="video/mp4,video/mpeg,video/quicktime,video/x-msvideo,video/webm"
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
            {t("lessons:detail.uploadingVideo")}
          </>
        ) : (
          <>
            <Upload className="h-4 w-4" />
            {t("lessons:detail.uploadVideo")}
          </>
        )}
      </Button>
    </div>
  );
}
