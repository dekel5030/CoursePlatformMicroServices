import { useMutation, useQueryClient } from "@tanstack/react-query";
import { generateImageUploadUrl, uploadImageToStorage } from "../api";
import type { GenerateUploadUrlRequest } from "../api";
import { coursesQueryKeys } from "./use-courses";

interface UploadImageParams {
  uploadUrl: string;
  file: File;
}

export function useCourseImageUpload(courseId: string) {
  const queryClient = useQueryClient();

  const generateUrlMutation = useMutation({
    mutationFn: ({
      uploadUrl,
      request,
    }: {
      uploadUrl: string;
      request: GenerateUploadUrlRequest;
    }) => generateImageUploadUrl(uploadUrl, request),
  });

  const uploadMutation = useMutation({
    mutationFn: ({ uploadUrl, file }: UploadImageParams) =>
      uploadImageToStorage(uploadUrl, file),
    onSuccess: () => {
      // Invalidate course queries to refresh the image
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.detail(courseId) });
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.featured() });
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.allCourses() });
    },
  });

  const uploadImage = async (file: File, uploadEndpoint: string) => {
    // Step 1: Request upload URL
    const response = await generateUrlMutation.mutateAsync({
      uploadUrl: uploadEndpoint,
      request: {
        fileName: file.name,
        contentType: file.type,
      },
    });

    // Step 2: Upload file to the presigned URL
    await uploadMutation.mutateAsync({
      uploadUrl: response.uploadUrl,
      file,
    });
  };

  return {
    uploadImage,
    isUploading: generateUrlMutation.isPending || uploadMutation.isPending,
    error: generateUrlMutation.error || uploadMutation.error,
    reset: () => {
      generateUrlMutation.reset();
      uploadMutation.reset();
    },
  };
}
