/**
 * HATEOAS link structure returned by the API
 * Represents hypermedia controls for self-discoverable REST APIs
 */
export interface LinkDto {
  href: string;
  rel: string;
  method: string;
}

/**
 * Interface for responses that include HATEOAS links
 */
export interface ILinksResponse {
  links: LinkDto[];
}

/**
 * Generic paged response with HATEOAS links
 */
export interface PagedResponse<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  links: LinkDto[];
}
