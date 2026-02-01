import { useMutation, useQueryClient } from "@tanstack/react-query";
import { generateImageUploadUrl, uploadImageToStorage } from "@/domain/courses/api";
import type { GenerateUploadUrlRequest } from "@/domain/courses/api";
import { coursesQueryKeys } from "@/domain/courses";

interface UploadImageParams {
  uploadUrl: string;
  file: File;
}

/**
 * Custom hook for handling course image upload flow
 * Manages the two-step upload process: requesting presigned URL and uploading the file
 */
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
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.detail(courseId) });
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.featured() });
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.allCourses() });
    },
  });

  const uploadImage = async (file: File, uploadEndpoint: string) => {
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
    uploadImage,
    isUploading: generateUrlMutation.isPending || uploadMutation.isPending,
    error: generateUrlMutation.error || uploadMutation.error,
    reset: () => {
      generateUrlMutation.reset();
      uploadMutation.reset();
    },
  };
}
