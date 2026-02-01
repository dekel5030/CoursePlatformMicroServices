import { axiosClient } from "@/app/axios";
import type {
  CourseRatingCollectionDto,
  CreateCourseRatingRequest,
  UpdateCourseRatingRequest,
} from "../types";

/**
 * Build URL with pagination query params
 */
function buildRatingsUrl(
  baseUrl: string,
  pageNumber: number,
  pageSize: number
): string {
  try {
    const url = new URL(baseUrl);
    url.searchParams.set("pageNumber", String(pageNumber));
    url.searchParams.set("pageSize", String(pageSize));
    return url.toString();
  } catch {
    return `${baseUrl}?pageNumber=${pageNumber}&pageSize=${pageSize}`;
  }
}

/**
 * Fetch course ratings - uses link href or constructed URL with pagination
 */
export async function fetchCourseRatings(
  url: string,
  pageNumber = 1,
  pageSize = 10
): Promise<CourseRatingCollectionDto> {
  const fetchUrl =
    pageNumber === 1 && pageSize === 10
      ? url
      : buildRatingsUrl(url, pageNumber, pageSize);
  const response = await axiosClient.get<CourseRatingCollectionDto>(fetchUrl);
  return response.data;
}

/**
 * Fetch course ratings by full URL (for pagination links: next-page, previous-page)
 */
export async function fetchCourseRatingsByUrl(
  url: string
): Promise<CourseRatingCollectionDto> {
  const response = await axiosClient.get<CourseRatingCollectionDto>(url);
  return response.data;
}

/**
 * Create a new rating (POST to create-rating link)
 */
export async function createCourseRating(
  url: string,
  request: CreateCourseRatingRequest
): Promise<void> {
  await axiosClient.post(url, request);
}

/**
 * Update a rating (PATCH to partial-update link)
 */
export async function patchCourseRating(
  url: string,
  request: UpdateCourseRatingRequest
): Promise<void> {
  await axiosClient.patch(url, request);
}

/**
 * Delete a rating (DELETE to delete link)
 */
export async function deleteCourseRating(url: string): Promise<void> {
  await axiosClient.delete(url);
}
