import { useRef } from "react";
import { useTranslation } from "react-i18next";
import { Button } from "@/shared/ui";
import { Upload, Loader2 } from "lucide-react";
import { toast } from "sonner";
import { useLessonVideoUpload } from "../hooks/useLessonVideoUpload";
import { getLinkFromRecord } from "@/shared/utils";
import type { LinksRecord } from "@/shared/types/LinkRecord";

interface LessonVideoUploadProps {
  courseId: string;
  lessonId: string;
  links?: LinksRecord;
}

const ALLOWED_VIDEO_TYPES = [
  "video/mp4",
  "video/mpeg",
  "video/quicktime",
  "video/x-msvideo",
  "video/webm",
] as const;

const ALLOWED_VIDEO_EXTENSIONS = ALLOWED_VIDEO_TYPES.join(",");

export function LessonVideoUpload({
  courseId,
  lessonId,
  links,
}: LessonVideoUploadProps) {
  const { t } = useTranslation(["lesson-viewer", "translation"]);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const { uploadVideo, isUploading, reset } = useLessonVideoUpload(
    courseId,
    lessonId,
  );

  const uploadLink =
    getLinkFromRecord(links, "generateVideoUploadUrl") ??
    getLinkFromRecord(links, "uploadVideoUrl");
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
    event: React.ChangeEvent<HTMLInputElement>,
  ) => {
    const file = event.target.files?.[0];
    if (!file) return;

    // Validate file type
    if (!ALLOWED_VIDEO_TYPES.includes(file.type as typeof ALLOWED_VIDEO_TYPES[number])) {
      toast.error(t("lesson-viewer:detail.invalidVideoType"));
      resetFileInput();
      return;
    }

    try {
      reset();
      await uploadVideo(file, uploadLink.href!);
      toast.success(t("lesson-viewer:detail.videoUploadSuccess"));
      resetFileInput();
    } catch (err) {
      toast.error(t("lesson-viewer:detail.videoUploadFailed"));
      console.error("Failed to upload video:", err);
      resetFileInput();
    }
  };

  return (
    <div>
      <input
        ref={fileInputRef}
        type="file"
        accept={ALLOWED_VIDEO_EXTENSIONS}
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
            {t("lesson-viewer:detail.uploadingVideo")}
          </>
        ) : (
          <>
            <Upload className="h-4 w-4" />
            {t("lesson-viewer:detail.uploadVideo")}
          </>
        )}
      </Button>
    </div>
  );
}
