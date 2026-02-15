import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  fetchCourseById,
  fetchManagedCourseById,
  createCourse,
  patchCourse,
  deleteCourse,
  publishCourse,
  fetchAllCourses,
  createModule,
  patchModule,
  deleteModule,
  reorderModules,
  reorderLessons,
  type FetchAllCoursesResult,
  type CreateModuleRequest,
  type ReorderModulesRequest,
  type ReorderLessonsRequest,
} from "../api";
import type {
  CourseModel,
  CreateCourseRequestDto,
  UpdateCourseRequestDto,
} from "../types";
import { coursesQueryKeys } from "../query-keys";
import { API_ENDPOINTS } from "@/app/axios";
import { toast } from "sonner";

export function useAllCourses(url?: string) {
  return useQuery<FetchAllCoursesResult, Error>({
    queryKey: [...coursesQueryKeys.allCourses(), url || API_ENDPOINTS.COURSES],
    queryFn: () => fetchAllCourses(url),
  });
}

export function useCourse(id: string | undefined) {
  return useQuery<CourseModel, Error>({
    queryKey: id ? coursesQueryKeys.detail(id) : ["courses", "undefined"],
    queryFn: () => fetchCourseById(id!),
    enabled: !!id,
  });
}

export function useManagedCourse(id: string | undefined) {
  return useQuery<CourseModel, Error>({
    queryKey: id ? coursesQueryKeys.managedDetail(id) : ["courses", "managed", "undefined"],
    queryFn: () => fetchManagedCourseById(id!),
    enabled: !!id,
  });
}

export function useCreateCourse() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateCourseRequestDto) => createCourse(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.all });
    },
  });
}

export function usePatchCourse(id: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      url,
      request,
    }: {
      url: string;
      request: UpdateCourseRequestDto;
    }) => patchCourse(url, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.detail(id) });
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.managedDetail(id) });
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.allCourses(),
      });
    },
  });
}

export function useDeleteCourse() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (url: string) => deleteCourse(url),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: coursesQueryKeys.all });
    },
  });
}

export function usePublishCourse(courseId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (url: string) => publishCourse(url),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.detail(courseId),
      });
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.managedDetail(courseId),
      });
      toast.success("Course published successfully");
    },
    onError: () => {
      toast.error("Failed to publish course");
    },
  });
}

export function useCreateModule(courseId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      url,
      request,
    }: {
      url: string;
      request?: CreateModuleRequest;
    }) => createModule(url, request),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.detail(courseId),
      });
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.managedDetail(courseId),
      });
      toast.success("Module created successfully");
    },
    onError: () => {
      toast.error("Failed to create module");
    },
  });
}

export function usePatchModule(courseId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      url,
      request,
    }: {
      url: string;
      request: { title?: string };
    }) => patchModule(url, request),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.detail(courseId),
      });
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.managedDetail(courseId),
      });
    },
  });
}

export function useDeleteModule(courseId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (deleteUrl: string) => deleteModule(deleteUrl),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.detail(courseId),
      });
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.managedDetail(courseId),
      });
      toast.success("Module deleted successfully");
    },
    onError: () => {
      toast.error("Failed to delete module");
    },
  });
}

export function useReorderModules(courseId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: ReorderModulesRequest) =>
      reorderModules(courseId, request),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.detail(courseId),
      });
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.managedDetail(courseId),
      });
      toast.success("Modules reordered successfully");
    },
    onError: () => {
      toast.error("Failed to reorder modules");
    },
  });
}

export function useReorderLessons(courseId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      moduleId,
      lessonIds,
    }: ReorderLessonsRequest & { moduleId: string }) =>
      reorderLessons(moduleId, { lessonIds }),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.detail(courseId),
      });
      queryClient.invalidateQueries({
        queryKey: coursesQueryKeys.managedDetail(courseId),
      });
      toast.success("Lessons reordered successfully");
    },
    onError: () => {
      toast.error("Failed to reorder lessons");
    },
  });
}
