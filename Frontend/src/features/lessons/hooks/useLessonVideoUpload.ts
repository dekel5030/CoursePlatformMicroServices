import { useMutation, useQueryClient } from "@tanstack/react-query";
import { generateVideoUploadUrl, uploadVideoToStorage } from "../api";
import type { GenerateVideoUploadUrlRequest } from "../api";
import { lessonsQueryKeys } from "./use-lessons";

interface UploadVideoParams {
  uploadUrl: string;
  file: File;
}

/**
 * Custom hook for handling lesson video upload flow
 * Manages the two-step upload process: requesting presigned URL and uploading the file
 * @param courseId - The ID of the course
 * @param lessonId - The ID of the lesson to upload the video for
 * @returns Object with uploadVideo function, loading state, error state, and reset function
 */
export function useLessonVideoUpload(courseId: string, lessonId: string) {
  const queryClient = useQueryClient();

  const generateUrlMutation = useMutation({
    mutationFn: ({
      uploadUrl,
      request,
    }: {
      uploadUrl: string;
      request: GenerateVideoUploadUrlRequest;
    }) => generateVideoUploadUrl(uploadUrl, request),
  });

  const uploadMutation = useMutation({
    mutationFn: ({ uploadUrl, file }: UploadVideoParams) =>
      uploadVideoToStorage(uploadUrl, file),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: lessonsQueryKeys.detail(courseId, lessonId) });
      queryClient.invalidateQueries({ queryKey: lessonsQueryKeys.all(courseId) });
    },
  });

  const uploadVideo = async (file: File, uploadEndpoint: string) => {
    const response = await generateUrlMutation.mutateAsync({
      uploadUrl: uploadEndpoint,
      request: {
        fileName: file.name,
        contentType: file.type,
      },
    });

    await uploadMutation.mutateAsync({
      uploadUrl: response.uploadUrl,
      file,
    });
  };

  return {
    uploadVideo,
    isUploading: generateUrlMutation.isPending || uploadMutation.isPending,
    error: generateUrlMutation.error || uploadMutation.error,
    reset: () => {
      generateUrlMutation.reset();
      uploadMutation.reset();
    },
  };
}
