import { useNavigate } from "react-router-dom";
import { Button } from "@/shared/ui";
import type { LinkRecord } from "@/shared/types/LinkRecord";

export interface LinkButtonsProps {
  /** Strongly-typed links record (rel -> LinkRecord) */
  links: Record<string, LinkRecord | undefined> | undefined;
  /** Label for each rel key (e.g. { partialUpdate: "Edit", delete: "Delete" }) */
  labelByRel: Record<string, string>;
  /** Called for non-GET actions (POST/PATCH/DELETE/PUT); for GET can be used to navigate */
  onAction?: (rel: string, link: LinkRecord) => void | Promise<void>;
  /** Rels to skip (e.g. ["self"]) */
  excludeRels?: string[];
  /** Optional: resolve API href to app route for GET links (client-side nav) */
  getRouteForHref?: (href: string) => string | null;
  /** Optional: state to pass when navigating (e.g. lessonSelfLink for lesson manage/self) */
  getStateForHref?: (href: string, rel: string) => object | undefined;
  variant?: "default" | "destructive" | "outline" | "secondary" | "ghost" | "link";
  size?: "default" | "sm" | "lg" | "icon";
  className?: string;
}

/**
 * Renders one button per available link. GET links become navigation (if getRouteForHref provided) or trigger onAction; others call onAction(rel, link).
 */
export function LinkButtons({
  links,
  labelByRel,
  onAction,
  excludeRels = ["self"],
  getRouteForHref,
  getStateForHref,
  variant = "outline",
  size = "sm",
  className,
}: LinkButtonsProps) {
  const navigate = useNavigate();

  if (!links || typeof links !== "object") return null;

  const entries = Object.entries(links).filter(
    ([rel, link]) =>
      !excludeRels.includes(rel) &&
      link?.href &&
      labelByRel[rel] != null
  );

  if (entries.length === 0) return null;

  const handleClick = (rel: string, link: LinkRecord) => {
    const method = (link.method ?? "GET").toUpperCase();
    if (method === "GET" && link.href) {
      const route = getRouteForHref?.(link.href);
      if (route != null) {
        const state = getStateForHref?.(link.href, rel);
        navigate(route, state ? { state } : undefined);
        return;
      }
    }
    onAction?.(rel, link);
  };

  return (
    <div className={`flex flex-wrap gap-2 ${className ?? ""}`}>
      {entries.map(([rel, link]) => {
        if (!link?.href) return null;
        const label = labelByRel[rel];
        const method = (link.method ?? "GET").toUpperCase();
        const isGet = method === "GET";
        const route = isGet ? getRouteForHref?.(link.href) : null;

        if (isGet && route != null) {
          return (
            <Button
              key={rel}
              variant={variant}
              size={size}
              onClick={() => {
                const state = getStateForHref?.(link!.href!, rel);
                navigate(route, state ? { state } : undefined);
              }}
              className="gap-2"
            >
              {label}
            </Button>
          );
        }

        return (
          <Button
            key={rel}
            variant={rel === "delete" ? "destructive" : variant}
            size={size}
            onClick={() => handleClick(rel, link)}
            className="gap-2"
          >
            {label}
          </Button>
        );
      })}
    </div>
  );
}
