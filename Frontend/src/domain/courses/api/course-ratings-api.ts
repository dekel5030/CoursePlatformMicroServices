import { axiosClient } from "@/app/axios";
import type {
  CourseRatingCollectionDto,
  CourseRatingDto,
  CreateCourseRatingRequest,
  UpdateCourseRatingRequest,
} from "../types";

/** Raw item from GET course ratings when backend returns { data, links } */
interface CourseRatingItemDtoApi {
  data: {
    id: string;
    courseId: string;
    user: { id: string; firstName: string; lastName: string; avatarUrl: string | null };
    rating: number;
    comment: string;
    createdAt: string;
    updatedAt: string | null;
  };
  links: { update?: { href?: string; method?: string }; delete?: { href?: string; method?: string } };
}

function isRatingItemWithData(item: unknown): item is CourseRatingItemDtoApi {
  return (
    item != null &&
    typeof item === "object" &&
    "data" in item &&
    typeof (item as CourseRatingItemDtoApi).data === "object"
  );
}

function normalizeRatingsResponse(raw: {
  items: unknown[];
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages?: number;
  links?: unknown;
}): CourseRatingCollectionDto {
  const items: CourseRatingDto[] = (raw.items ?? []).map((item) => {
    if (isRatingItemWithData(item)) {
      return {
        id: item.data.id,
        courseId: item.data.courseId,
        user: item.data.user,
        rating: item.data.rating,
        comment: item.data.comment,
        createdAt: item.data.createdAt,
        updatedAt: item.data.updatedAt,
        links: item.links,
      };
    }
    return item as CourseRatingDto;
  });
  return {
    pageNumber: raw.pageNumber,
    pageSize: raw.pageSize,
    totalItems: raw.totalItems,
    totalPages: raw.totalPages,
    items,
    links: (raw.links ?? []) as CourseRatingCollectionDto["links"],
  };
}

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
 * Fetch course ratings - uses link href or constructed URL with pagination.
 * Normalizes backend shape when items are { data, links }.
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
  return normalizeRatingsResponse(response.data as Parameters<typeof normalizeRatingsResponse>[0]);
}

/**
 * Fetch course ratings by full URL (for pagination links).
 * Normalizes backend shape when items are { data, links }.
 */
export async function fetchCourseRatingsByUrl(
  url: string
): Promise<CourseRatingCollectionDto> {
  const response = await axiosClient.get<CourseRatingCollectionDto>(url);
  return normalizeRatingsResponse(response.data as Parameters<typeof normalizeRatingsResponse>[0]);
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
