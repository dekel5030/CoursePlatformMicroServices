import { Link, useLocation } from "react-router-dom";
import { useTranslation } from "react-i18next";
import type { TFunction } from "i18next";
import {
  Breadcrumb as BreadcrumbRoot,
  BreadcrumbList,
  BreadcrumbItem as BreadcrumbItemUI,
  BreadcrumbLink,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import React from "react";

export interface BreadcrumbItem {
  label: string;
  path?: string;
}

interface BreadcrumbProps {
  items?: BreadcrumbItem[];
}

export default function Breadcrumb({ items }: BreadcrumbProps) {
  const location = useLocation();
  const { t } = useTranslation();

  // If no items are provided, auto-generate from URL path
  const breadcrumbItems = items || generateBreadcrumbsFromPath(location.pathname, t);

  if (breadcrumbItems.length === 0) return null;

  return (
    <div className="bg-background border-b border-border py-3 px-8">
      <div className="max-w-7xl mx-auto">
        <BreadcrumbRoot>
          <BreadcrumbList>
            {breadcrumbItems.map((item, index) => {
              const isLast = index === breadcrumbItems.length - 1;

              return (
                <React.Fragment key={index}>
                  <BreadcrumbItemUI>
                    {!isLast && item.path ? (
                      <BreadcrumbLink asChild>
                        <Link to={item.path}>{item.label}</Link>
                      </BreadcrumbLink>
                    ) : (
                      <BreadcrumbPage>{item.label}</BreadcrumbPage>
                    )}
                  </BreadcrumbItemUI>
                  {!isLast && <BreadcrumbSeparator />}
                </React.Fragment>
              );
            })}
          </BreadcrumbList>
        </BreadcrumbRoot>
      </div>
    </div>
  );
}

function generateBreadcrumbsFromPath(pathname: string, t: TFunction): BreadcrumbItem[] {
  const items: BreadcrumbItem[] = [{ label: t("breadcrumbs.home"), path: "/" }];

  const pathSegments = pathname.split("/").filter(Boolean);

  let currentPath = "";
  pathSegments.forEach((segment, index) => {
    currentPath += `/${segment}`;

    // Skip if it's a GUID (lesson or course ID)
    const isGuid = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(segment);

    if (!isGuid) {
      // Try to find translation for the segment, otherwise capitalize it
      const translationKey = `breadcrumbs.${segment.toLowerCase()}`;
      const label = t(translationKey, { defaultValue: capitalizeFirst(segment) });
      
      items.push({
        label,
        path: index < pathSegments.length - 1 ? currentPath : undefined,
      });
    }
  });

  return items;
}

function capitalizeFirst(str: string): string {
  return str.charAt(0).toUpperCase() + str.slice(1);
}