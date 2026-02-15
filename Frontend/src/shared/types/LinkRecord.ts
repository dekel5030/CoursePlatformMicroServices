/**
 * HATEOAS link record (OpenAPI strongly-typed links).
 * Rel is the object key; each value has href and method.
 */
export interface LinkRecord {
  href?: string | null;
  method?: string | null;
}

/**
 * Generic links record: rel (camelCase) -> LinkRecord
 */
export type LinksRecord = Record<string, LinkRecord | undefined>;
